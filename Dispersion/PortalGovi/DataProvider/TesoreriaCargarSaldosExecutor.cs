using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortalGovi.DataProvider
{
    /// <summary>
    /// Ejecuta los lotes SQL de tesorería (DISPERSION + OPENQUERY HANA) para el cuadro de inversión.
    /// </summary>
    public static class TesoreriaCargarSaldosExecutor
    {
        /// <summary>
        /// Fecha PET: día siguiente a la fecha de referencia; si la referencia es viernes, el lunes siguiente.
        /// </summary>
        public static DateTime ComputeFechaPet(DateTime fechaReferencia)
        {
            var d = fechaReferencia.Date;
            return d.DayOfWeek == DayOfWeek.Friday ? d.AddDays(3) : d.AddDays(1);
        }

        public static string ToYyyymmdd(DateTime d) => d.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        public static async Task ExecuteAllAsync(SqlConnection connection, DateTime saldoDiario, DateTime transDesde, DateTime transHasta, DateTime depDesde, DateTime depHasta, CancellationToken cancellationToken = default)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var fechaPetSaldo = ComputeFechaPet(saldoDiario);
            var fechaPetTrans = ComputeFechaPet(transHasta);
            var fechaPetDep = ComputeFechaPet(depHasta);
            var fechaPetHistorico = ComputeFechaPet(saldoDiario);

            await RunSaldoDiarioAsync(connection, saldoDiario, fechaPetSaldo, cancellationToken).ConfigureAwait(false);
            await RunTransferenciasAsync(connection, transDesde, transHasta, fechaPetTrans, cancellationToken).ConfigureAwait(false);
            await RunDepositosAsync(connection, depDesde, depHasta, fechaPetDep, cancellationToken).ConfigureAwait(false);
            await RunHistoricoAnalisisAsync(connection, saldoDiario, fechaPetHistorico, cancellationToken).ConfigureAwait(false);
        }

        private static async Task RunSaldoDiarioAsync(SqlConnection connection, DateTime saldoDiario, DateTime fechaPet, CancellationToken ct)
        {
            var ymd = ToYyyymmdd(saldoDiario);
            var sql = new StringBuilder();
            sql.AppendLine("SET NOCOUNT ON;");
            sql.AppendLine("DECLARE @FechaAAAAMMDD varchar(8) = @pFechaYmd;");
            sql.AppendLine("DECLARE @FechaPET      date       = @pFechaPet;");
            sql.AppendLine(@"IF @FechaAAAAMMDD NOT LIKE '[12][0-9][0-9][0-9][01][0-9][0-3][0-9]'
   OR TRY_CONVERT(date, STUFF(STUFF(@FechaAAAAMMDD,5,0,'-'),8,0,'-')) IS NULL
BEGIN
    THROW 50000, 'La fecha debe venir como AAAAMMDD y ser válida.', 1;
END;");
            sql.AppendLine(@"DELETE FROM [DISPERSION].[dbo].[Tesoreria_SaldosBancarios]
WHERE Fecha = CONVERT(date, STUFF(STUFF(@FechaAAAAMMDD,5,0,'-'),8,0,'-'));");
            sql.AppendLine("DECLARE @FechaISO varchar(10) = STUFF(STUFF(@FechaAAAAMMDD,5,0,'-'),8,0,'-');");
            sql.AppendLine("DECLARE @HanaSql nvarchar(max); DECLARE @Tsql nvarchar(max);");
            sql.AppendLine(@"SET @HanaSql = N'
SELECT
    ""BD""          AS ""BD"",
    ""Cuenta""      AS ""Cuenta"",
    ""NombreCuenta"" AS ""NombreCuenta"",
    ""SALDO DIARIO"" AS ""SaldoDiario"",
    ""Fecha""       AS ""Fecha""
FROM ""STS_GROB"".""CV_SALDO_DIARIO_V2""
WHERE ""Fecha"" = ''' + @FechaISO + N'''
';");
            sql.AppendLine(@"SET @Tsql = N'
INSERT INTO [DISPERSION].[dbo].[Tesoreria_SaldosBancarios]
    (Empresa, Cuenta, Banco, Saldo, Fecha, FechaPET)
SELECT
    Q.""NombreCuenta""              AS Empresa,
    Q.""Cuenta""                    AS Cuenta,
    Q.""NombreCuenta""              AS Banco,
    Q.""SaldoDiario""               AS Saldo,
    CONVERT(date, Q.""Fecha"")      AS Fecha,
    @FechaPET                     AS FechaPET
FROM OPENQUERY(HANA, ''' + REPLACE(@HanaSql, '''', '''''') + N''') AS Q;
';");
            sql.AppendLine("EXEC sp_executesql @Tsql, N'@FechaPET date', @FechaPET = @FechaPET;");

            using var cmd = new SqlCommand(sql.ToString(), connection) { CommandTimeout = 600 };
            cmd.Parameters.Add("@pFechaYmd", SqlDbType.Char, 8).Value = ymd;
            cmd.Parameters.Add("@pFechaPet", SqlDbType.Date).Value = fechaPet;
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        private static async Task RunTransferenciasAsync(SqlConnection connection, DateTime desde, DateTime hasta, DateTime fechaPet, CancellationToken ct)
        {
            var dateI = ToYyyymmdd(desde);
            var dateF = ToYyyymmdd(hasta);
            var dateIIso = desde.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var dateFIso = hasta.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var hanaInner = new StringBuilder();
            hanaInner.AppendLine(@"SELECT 
  X.""Empresa"",
  X.""Sucursal"",
  X.""Account"",
  IFNULL(L.""Transfers"", 0) AS ""Transfers"",
  IFNULL(SUM(""Importe Sin Domiciliados""), 0) AS ""Importe Sin Domiciliados"",
  IFNULL(SUM(""Importe Domiciliado""), 0) AS ""Importe Domiciliado"",
  IFNULL(O.""Domiciliados"", 0) AS ""Domiciliados EDC (CFE,DOMICILIACION)"",
  IFNULL(E.""TotalComisiones"", 0) AS ""Comisiones EDC (IVA,COMISION)"",
  IFNULL(O.""Domiciliados"", 0) + IFNULL(E.""TotalComisiones"", 0) + IFNULL(SUM(""Total""), 0) AS ""Total"",
  CAST('" + dateIIso + @"' AS DATE) AS ""FechaInicial"",
  CAST('" + dateFIso + @"' AS DATE) AS ""FechaFinal""
FROM ""_SYS_BIC"".""ProductivaGOVI/CUENTAS_BANCARIAS"" X
LEFT JOIN (
  SELECT
    L.""Empresa"",
    L.""Sucursal"",
    L.""Cuenta"",
    COUNT(*) AS ""Transfers"",
    SUM(CAST(CASE WHEN L.""U_Domiciliado"" = 'Si' THEN 0.00 ELSE L.""Importe"" END AS NUMERIC(19,2))) AS ""Importe Sin Domiciliados"",
    SUM(CAST(CASE WHEN L.""U_Domiciliado"" = 'No' THEN 0.00 ELSE L.""Importe"" END AS NUMERIC(19,2))) AS ""Importe Domiciliado"",
    SUM(CAST(CASE WHEN L.""U_Domiciliado"" = 'Si' THEN 0.00 ELSE L.""Importe"" END AS NUMERIC(19,2))) AS ""Total""
  FROM ""_SYS_BIC"".""ProductivaGOVI.ReporteTransferencias/TransferenciasSBOGOVI"" L
  WHERE L.""FEmision"" BETWEEN CAST('" + dateIIso + @"' AS DATE) AND CAST('" + dateFIso + @"' AS DATE)
  GROUP BY L.""Empresa"", L.""Sucursal"", L.""Cuenta""
) L
  ON L.""Cuenta"" = X.""Account""
LEFT JOIN (
  SELECT 
    ""Account"",
    CAST(IFNULL(SUM(""TotalComisiones""), 0) AS NUMERIC(19,2)) AS ""TotalComisiones""
  FROM ""_SYS_BIC"".""Az/COMISIONES_EC""
  WHERE ""DueDate"" BETWEEN CAST('" + dateIIso + @"' AS DATE) AND CAST('" + dateFIso + @"' AS DATE)
  GROUP BY ""Account""
) E
  ON E.""Account"" = X.""Account""
LEFT JOIN (
  SELECT 
    ""Account"",
    CAST(IFNULL(SUM(""Domiciliados""), 0) AS NUMERIC(19,2)) AS ""Domiciliados""
  FROM ""_SYS_BIC"".""Az/DOMICILIADO_EC""
  WHERE ""DueDate"" BETWEEN CAST('" + dateIIso + @"' AS DATE) AND CAST('" + dateFIso + @"' AS DATE)
  GROUP BY ""Account""
) O
  ON O.""Account"" = X.""Account""
WHERE X.""Sucursal"" NOT LIKE '%BAJA%'
GROUP BY
  X.""Empresa"",
  X.""Sucursal"",
  X.""Account"",
  IFNULL(L.""Transfers"",0),
  E.""TotalComisiones"",
  O.""Domiciliados""
ORDER BY
  X.""Empresa"",
  X.""Sucursal"",
  X.""Account""");

            var hanaSql = hanaInner.ToString();
            var hanaEscaped = hanaSql.Replace("'", "''");

            var batch = new StringBuilder();
            batch.AppendLine("SET NOCOUNT ON;");
            batch.AppendLine("DECLARE @FechaPet DATE = @pFechaPet;");
            batch.AppendLine("DECLARE @DateI  varchar(8) = @pDateI;");
            batch.AppendLine("DECLARE @DateF  varchar(8) = @pDateF;");
            batch.AppendLine(@"IF @DateI NOT LIKE '[12][0-9][0-9][0-9][01][0-9][0-3][0-9]'
 OR @DateF NOT LIKE '[12][0-9][0-9][0-9][01][0-9][0-3][0-9]'
 OR TRY_CONVERT(date, STUFF(STUFF(@DateI,5,0,'-'),8,0,'-')) IS NULL
 OR TRY_CONVERT(date, STUFF(STUFF(@DateF,5,0,'-'),8,0,'-')) IS NULL
BEGIN
  THROW 50000, 'Las fechas deben venir como AAAAMMDD y ser válidas.', 1;
END;");
            batch.AppendLine("DECLARE @FechaInicial date = CONVERT(date, STUFF(STUFF(@DateI,5,0,'-'),8,0,'-'));");
            batch.AppendLine("DECLARE @FechaFinal   date = CONVERT(date, STUFF(STUFF(@DateF,5,0,'-'),8,0,'-'));");
            batch.AppendLine("DECLARE @HanaSql nvarchar(max) = N'" + hanaEscaped + "';");
            batch.AppendLine(@"IF OBJECT_ID('tempdb..#Transf') IS NOT NULL DROP TABLE #Transf;
CREATE TABLE #Transf(
  Empresa                 nvarchar(255),
  Sucursal                nvarchar(255),
  Cuenta                  nvarchar(255),
  Transfers               int,
  ImporteSinDomiciliados  decimal(19,2),
  ImporteDomiciliados     decimal(19,2),
  DomiciliadosEDC         decimal(19,2),
  ComisionesEDC           decimal(19,2),
  Total                   decimal(19,2),
  FechaInicial            date,
  FechaFinal              date
);");
            batch.AppendLine(@"DECLARE @Tsql nvarchar(max) =
N'
INSERT INTO #Transf(
  Empresa,Sucursal,Cuenta,Transfers,
  ImporteSinDomiciliados,ImporteDomiciliados,
  DomiciliadosEDC,ComisionesEDC,Total,
  FechaInicial,FechaFinal
)
SELECT 
  Q.""Empresa"",
  Q.""Sucursal"",
  Q.""Account"",
  Q.""Transfers"",
  Q.""Importe Sin Domiciliados"",
  Q.""Importe Domiciliado"",
  Q.""Domiciliados EDC (CFE,DOMICILIACION)"",
  Q.""Comisiones EDC (IVA,COMISION)"",
  Q.""Total"",
  Q.""FechaInicial"",
  Q.""FechaFinal""
FROM OPENQUERY(HANA, ''' + REPLACE(@HanaSql, '''', '''''') + N''') AS Q;
';");
            batch.AppendLine("EXEC sys.sp_executesql @Tsql;");
            batch.AppendLine(@"BEGIN TRY
  BEGIN TRAN;
  DELETE FROM [DISPERSION].[dbo].[Tesoreria_SaldosTransferencias]
  WHERE [FechaInicial] = @FechaInicial AND [FechaFinal] = @FechaFinal;
  INSERT INTO [DISPERSION].[dbo].[Tesoreria_SaldosTransferencias](
    [Empresa],[Sucursal],[Cuenta],[Transfers],[ImporteSinDomiciliados],[ImporteDomiciliados],
    [DomiciliadosEDC],[ComisionesEDC],[Total],[FechaInicial],[FechaFinal],[FechaPet]
  )
  SELECT Empresa,Sucursal,Cuenta,Transfers,ImporteSinDomiciliados,ImporteDomiciliados,
    DomiciliadosEDC,ComisionesEDC,Total,FechaInicial,FechaFinal,@FechaPet FROM #Transf;
  COMMIT TRAN;
END TRY
BEGIN CATCH
  IF XACT_STATE() <> 0 ROLLBACK TRAN;
  THROW;
END CATCH;");

            using var cmd = new SqlCommand(batch.ToString(), connection) { CommandTimeout = 600 };
            cmd.Parameters.Add("@pFechaPet", SqlDbType.Date).Value = fechaPet;
            cmd.Parameters.Add("@pDateI", SqlDbType.Char, 8).Value = dateI;
            cmd.Parameters.Add("@pDateF", SqlDbType.Char, 8).Value = dateF;
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        private static async Task RunDepositosAsync(SqlConnection connection, DateTime desde, DateTime hasta, DateTime fechaPet, CancellationToken ct)
        {
            var fechaYmd = ToYyyymmdd(desde);
            var fechaFYmd = ToYyyymmdd(hasta);
            var fechaDate = desde.Date;
            var fechaFDate = hasta.Date;
            var fechaIso = desde.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaFIso = hasta.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var castFecha = "CAST('" + fechaIso + "' AS DATE)";
            var castFechaF = "CAST('" + fechaFIso + "' AS DATE)";

            const string template = @"
SELECT
    (SELECT ""CompnyName"" FROM ""SBOCHAPULTEPEC"".""OADM"") AS ""Empresa"",
    CASE WHEN L.""GLAccount"" = '105-024' THEN 'APERTURA' ELSE L.""Block"" END AS ""Sucursal"",
    E.""BankName"" AS ""Banco"",
    L.""GLAccount"" AS ""Contable"",
    L.""Account"" AS ""Cuenta"",
    CASE
        WHEN L.""County"" = 'USD' THEN CAST(""STS_GROB"".""SBOCHAPULTEPEC_DEPOSITOS_SUMA""(L.""GLAccount"", :L_Fecha, :L_FechaF, 1) AS DECIMAL(19, 2))
        ELSE CAST(""STS_GROB"".""SBOCHAPULTEPEC_DEPOSITOS_SUMA""(L.""GLAccount"", :L_Fecha, :L_FechaF, 0) AS DECIMAL(19, 2))
    END AS ""Depositos"",
    CASE
        WHEN L.""County"" = 'USD' THEN CAST(""STS_GROB"".""SBOCHAPULTEPEC_DEP_CTA_PROPIA_SUMA""(L.""GLAccount"", :L_Fecha, :L_FechaF, 1) AS DECIMAL(19, 2))
        ELSE CAST(""STS_GROB"".""SBOCHAPULTEPEC_DEP_CTA_PROPIA_SUMA""(L.""GLAccount"", :L_Fecha, :L_FechaF, 0) AS DECIMAL(19, 2))
    END AS ""Depositos Cuenta Propia"",
    CAST(""STS_GROB"".""CHEQUES_DEVUELTOS""(L.""GLAccount"", :L_Fecha, :L_FechaF, 'SBOCHAPULTEPEC') AS DECIMAL(19, 2)) AS ""Cheques Devolucion"",
    IFNULL(SUM(O.""DebAmount""),0) AS ""EgresosCuentaPropia"",
    TO_VARCHAR(IFNULL(N.""Rate"", 0)) AS ""Rate"",
    CAST(:L_Fecha  AS DATE) AS ""FechaInicial"",
    CAST(:L_FechaF AS DATE) AS ""FechaFinal""
FROM ""SBOCHAPULTEPEC"".""DSC1"" L
INNER JOIN ""SBOCHAPULTEPEC"".""ODSC"" E ON L.""BankCode"" = E.""BankCode""
LEFT JOIN  ""SBOCHAPULTEPEC"".""OBNK"" O ON O.""AcctCode"" = L.""GLAccount"" AND O.""DueDate"" = :L_Fecha
LEFT JOIN (
    SELECT ""Rate"" FROM ""SBOCHAPULTEPEC"".""ORTT"" WHERE ""RateDate"" = ADD_DAYS(:L_FechaF, 1) AND ""Currency"" = 'USD'
) N ON 1 = 1
WHERE L.""GLAccount"" IS NOT NULL
GROUP BY L.""Block"", E.""BankName"", L.""GLAccount"", L.""Account"", N.""Rate"", L.""County""";

            var empresas = new[]
            {
                "SBOGOVI", "SBOLAFE", "SBOSANNICOLAS", "SBOAUTOMOTIVE", "SBOGOVISUSPEN", "SBOGROBSUSP",
                "SBOGOVI2", "SBOSERVINMB", "SBOSUSPGR", "SBOREFAGR", "SBOCHAPULTEPEC"
            };

            var validateSql = @"
IF @pFecha  NOT LIKE '[12][0-9][0-9][0-9][01][0-9][0-3][0-9]'
 OR @pFechaF NOT LIKE '[12][0-9][0-9][0-9][01][0-9][0-3][0-9]'
 OR TRY_CONVERT(date, STUFF(STUFF(@pFecha ,5,0,'-'),8,0,'-')) IS NULL
 OR TRY_CONVERT(date, STUFF(STUFF(@pFechaF,5,0,'-'),8,0,'-')) IS NULL
BEGIN
  THROW 50000, 'Las fechas deben venir como AAAAMMDD y ser válidas.', 1;
END;";
            using (var v = new SqlCommand(validateSql, connection) { CommandTimeout = 60 })
            {
                v.Parameters.Add("@pFecha", SqlDbType.Char, 8).Value = fechaYmd;
                v.Parameters.Add("@pFechaF", SqlDbType.Char, 8).Value = fechaFYmd;
                await v.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
            }

            const string dropCreate = @"
IF OBJECT_ID('tempdb..#Depositos') IS NOT NULL DROP TABLE #Depositos;
CREATE TABLE #Depositos (
  Empresa             nvarchar(200),
  Sucursal            nvarchar(200),
  Banco               nvarchar(200),
  Contable            nvarchar(50),
  Cuenta              nvarchar(100),
  Depositos           decimal(19,2),
  DepositosProp       decimal(19,2),
  ChequesDev          decimal(19,2),
  EgresosCuentaPropia decimal(19,2),
  Rate                nvarchar(100),
  FechaInicial        date,
  FechaFinal          date
);";
            using (var c0 = new SqlCommand(dropCreate, connection) { CommandTimeout = 60 })
                await c0.ExecuteNonQueryAsync(ct).ConfigureAwait(false);

            foreach (var empresa in empresas)
            {
                var hanaSql = template;
                if (empresa == "SBOGOVI")
                {
                    hanaSql = hanaSql.Replace("\"STS_GROB\".\"SBOCHAPULTEPEC_DEPOSITOS_SUMA\"", "\"STS_GROB\".\"DEPOSITOS_SUMA\"", StringComparison.Ordinal)
                        .Replace("\"STS_GROB\".\"SBOCHAPULTEPEC_DEP_CTA_PROPIA_SUMA\"", "\"STS_GROB\".\"DEP_CTA_PROPIA_SUMA\"", StringComparison.Ordinal)
                        .Replace("\"STS_GROB\".\"SBOCHAPULTEPEC_TESORERIA_SUMA\"", "\"STS_GROB\".\"SBOGOVI_TESORERIA_SUMA\"", StringComparison.Ordinal);
                }
                else
                {
                    hanaSql = hanaSql.Replace("\"STS_GROB\".\"SBOCHAPULTEPEC_DEPOSITOS_SUMA\"", "\"STS_GROB\".\"" + empresa + "_DEPOSITOS_SUMA\"", StringComparison.Ordinal)
                        .Replace("\"STS_GROB\".\"SBOCHAPULTEPEC_DEP_CTA_PROPIA_SUMA\"", "\"STS_GROB\".\"" + empresa + "_DEP_CTA_PROPIA_SUMA\"", StringComparison.Ordinal)
                        .Replace("\"STS_GROB\".\"SBOCHAPULTEPEC_TESORERIA_SUMA\"", "\"STS_GROB\".\"" + empresa + "_TESORERIA_SUMA\"", StringComparison.Ordinal);
                }

                hanaSql = hanaSql.Replace("SBOCHAPULTEPEC", empresa, StringComparison.Ordinal)
                    .Replace(":L_FechaF", castFechaF, StringComparison.Ordinal)
                    .Replace(":L_Fecha", castFecha, StringComparison.Ordinal);

                var hanaEscaped = hanaSql.Replace("'", "''");
                var insertSql = "INSERT INTO #Depositos (Empresa, Sucursal, Banco, Contable, Cuenta, Depositos, DepositosProp, ChequesDev, EgresosCuentaPropia, Rate, FechaInicial, FechaFinal) " +
                    "SELECT Empresa, Sucursal, Banco, Contable, Cuenta, [Depositos], [Depositos Cuenta Propia], [Cheques Devolucion], [EgresosCuentaPropia], [Rate], [FechaInicial], [FechaFinal] " +
                    "FROM OPENQUERY(HANA, '" + hanaEscaped + "')";

                try
                {
                    using var cIns = new SqlCommand(insertSql, connection) { CommandTimeout = 600 };
                    await cIns.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
                }
                catch
                {
                    /* por empresa: continuar como en el script original (PRINT y siguiente) */
                }
            }

            const string mergeSql = @"
BEGIN TRY
  BEGIN TRAN;
  DELETE FROM [DISPERSION].[dbo].[Tesoreria_SaldosDepositos]
  WHERE [FechaInicial] = @FechaDate AND [FechaFinal] = @FechaFDate;
  INSERT INTO [DISPERSION].[dbo].[Tesoreria_SaldosDepositos]
  (Empresa, Sucursal, Banco, Contable, Cuenta, Depositos, DepositosProp, ChequesDev, EgresosCuentaPropia, Rate, FechaInicial, FechaFinal, FechaPet)
  SELECT Empresa, Sucursal, Banco, Contable, Cuenta, Depositos, DepositosProp, ChequesDev, EgresosCuentaPropia, Rate, FechaInicial, FechaFinal, @FechaPet FROM #Depositos;
  COMMIT TRAN;
END TRY
BEGIN CATCH
  IF XACT_STATE() <> 0 ROLLBACK TRAN;
  THROW;
END CATCH;";
            using (var cFin = new SqlCommand(mergeSql, connection) { CommandTimeout = 600 })
            {
                cFin.Parameters.Add("@FechaDate", SqlDbType.Date).Value = fechaDate;
                cFin.Parameters.Add("@FechaFDate", SqlDbType.Date).Value = fechaFDate;
                cFin.Parameters.Add("@FechaPet", SqlDbType.Date).Value = fechaPet;
                await cFin.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
            }
        }

        private static async Task RunHistoricoAnalisisAsync(SqlConnection connection, DateTime saldoDiario, DateTime fechaPet, CancellationToken ct)
        {
            var ymd = ToYyyymmdd(saldoDiario);
            var sql = new StringBuilder();
            sql.AppendLine("SET NOCOUNT ON;");
            sql.AppendLine("DECLARE @Fecha varchar(8) = @pFechaYmd;");
            sql.AppendLine("DECLARE @FechaPet date = @pFechaPet;");
            sql.AppendLine("DECLARE @FechaDate date = CONVERT(date, STUFF(STUFF(@Fecha,5,0,'-'),8,0,'-'));");
            sql.AppendLine("DELETE FROM [DISPERSION].[dbo].[Tesoreria_HistoricoAnalisisSaldoDiario] WHERE FechaSD = @FechaPet;");
            sql.AppendLine(@"INSERT INTO [DISPERSION].[dbo].[Tesoreria_HistoricoAnalisisSaldoDiario] (
[FolioSD],[Almacen],[Cuentas],[RendiminetoDiario],[SaldoDiario],[Compra],[Venta],[ActualChequera],[SaldoInversion],[TotalBanorte],[Depositos],[DepositosVenta],[Transferencias],[DepositosMes],[ChequeMes],[DolaresTCDia],[DolaresTCDiario],[GranTotal],[Banorte],[Banamex],[Santander],[FechaSD],[Orden])
SELECT
       1 AS [FolioSD]
      ,L1.Almacen
      ,L1.Cuenta AS [Cuentas]
      ,0 AS [RendiminetoDiario]
      ,ISNULL(L2.Saldo,0) AS [SaldoDiario]
      ,ISNULL(L12.Compra,0) AS [Compra]
      ,ISNULL(l12.Venta,0) AS [Venta]
      ,ISNULL((ISNULL(L2.Saldo,0) - (L12.Compra) + (L12.Venta)),0) AS [ActualChequera]
      ,ISNULL(L12.SaldoInversion,0) AS [SaldoInversion]
      ,ISNULL(((ISNULL(L2.Saldo,0) - (L12.Compra) + (L12.Venta)) + (L12.SaldoInversion)),0) AS [TotalBanorte]
      ,ISNULL((L3.Depositos - L3.ChequesDev),0) AS [Depositos]
      ,0 AS [DepositosVenta]
      ,ISNULL(L4.Total,0) AS [Transferencias]
      ,0 AS [DepositosMes]
      ,0 AS [ChequeMes]
      ,0 AS [DolaresTCDia]
      ,0 AS [DolaresTCDiario]
      ,0 AS [GranTotal]
      ,0 AS [Banorte]
      ,0 AS [Banamex]
      ,0 AS [Santander]
      ,ISNULL(L2.FechaPet,@FechaPet) AS [FechaSD]
      ,L1.Orden
FROM [DISPERSION].[dbo].[Tesoreria_OrdendelosDatos] L1
LEFT JOIN [DISPERSION].[dbo].[Tesoreria_SaldosBancarios] L2
  ON TRY_CONVERT(bigint, L2.Cuenta) = TRY_CONVERT(bigint, L1.Cuenta)
 AND L2.Fecha = @FechaDate
LEFT JOIN [DISPERSION].[dbo].[Tesoreria_SaldosDepositos] L3
  ON TRY_CONVERT(bigint, L3.Cuenta) = TRY_CONVERT(bigint, L1.Cuenta)
 AND L3.FechaFinal = @FechaDate
LEFT JOIN [DISPERSION].[dbo].[Tesoreria_SaldosTransferencias] L4
  ON TRY_CONVERT(bigint, L4.Cuenta) = TRY_CONVERT(bigint, L1.Cuenta)
 AND L4.FechaFinal = @FechaDate
LEFT JOIN [DISPERSION].[dbo].[Tesoreria_SaldosCompraVenta] L12 ON CAST(L12.Cuenta as bigint) = CAST(L1.Cuenta AS bigint)
AND L12.FechaPet = @FechaPet
ORDER BY L1.Orden ASC;");

            using var cmd = new SqlCommand(sql.ToString(), connection) { CommandTimeout = 600 };
            cmd.Parameters.Add("@pFechaYmd", SqlDbType.Char, 8).Value = ymd;
            cmd.Parameters.Add("@pFechaPet", SqlDbType.Date).Value = fechaPet;
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }
    }
}

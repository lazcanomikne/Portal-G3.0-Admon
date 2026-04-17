using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PortalGovi.Models;
using PortalGovi.Models.Credito;
using Sap.Data.Hana;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.DataProvider
{
    public class CreditManager
    {
        private readonly IConfiguration _configuration;
        public Company oCompany;
        public HanaConnection connH;
        public string ConnectionString;// = _configuration.GetConnectionString("Sap");//"Server=192.168.1.30:30015;UserID=SYSTEM;Password=Pa$$w0rd!";
        public string SqlConectionString;
        public string SqlConectionStringMigrarTxt;
        public string SqlConectionStringAjustes;

        public CreditManager(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("Sap");
            SqlConectionString = _configuration.GetConnectionString("SQL");
            SqlConectionStringMigrarTxt = _configuration.GetConnectionString("MigrarTxt");
            SqlConectionStringAjustes = _configuration.GetConnectionString("Ajustes");
        }

        public dynamic GetCustomers(string sociedad, string sucursal)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("SELECT  \"CardCode\", \"CardName\" FROM \"_SYS_BIC\".\"ProductivaGOVI.PortalAdministrativo/GLOBAL_CLIENTES\" WHERE \"Sucursal\" = '{0}' AND  \"Sociedad\" = '{1}'", sucursal, sociedad);

                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataAdapter dataAdapter = new HanaDataAdapter(cmdHSAP))
                        {
                            using (DataTable data = new DataTable())
                            {
                                try
                                {
                                    dataAdapter.Fill(data);
                                    var list = data.AsEnumerable().Select(row =>
                                        new
                                        {
                                            CardCode = (string)row["CardCode"],
                                            CardName = (string)row["CardName"].ToString(),
                                        }).ToList();

                                    return list;
                                }
                                catch (Exception ex)
                                {
                                    return new List<dynamic>();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable GetPagoCta(string sociedad, string cuenta)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("SELECT  * FROM \"_SYS_BIC\".\"ProductivaGOVI.PortalAdministrativo/GLOBAL_EDO_CTA\" WHERE \"AcctCode\" = '{0}' AND  \"Sociedad\" = '{1}'", cuenta, sociedad);

                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataAdapter dataAdapter = new HanaDataAdapter(cmdHSAP))
                        {
                            using (DataTable data = new DataTable())
                            {
                                try
                                {
                                    dataAdapter.Fill(data);

                                    return data;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    return new DataTable();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable GetPendingBill(string sociedad, string cliente)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("SELECT * FROM \"_SYS_BIC\".\"ProductivaGOVI.PortalAdministrativo/GLOBAL_SALDO_PENDIENTE\" WHERE  \"Sociedad\" = '{0}' AND  \"CardCode\" = '{1}'", sociedad, cliente);

                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataAdapter dataAdapter = new HanaDataAdapter(cmdHSAP))
                        {
                            using (DataTable data = new DataTable())
                            {
                                try
                                {
                                    dataAdapter.Fill(data);

                                    return data;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    return new DataTable();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable GetTypeDiscount(string sociedad)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("SELECT * FROM \"_SYS_BIC\".\"ProductivaGOVI.PortalAdministrativo/GLOBAL_DESCUENTOS\" WHERE \"Sociedad\" = '{0}'", sociedad);

                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataAdapter dataAdapter = new HanaDataAdapter(cmdHSAP))
                        {
                            using (DataTable data = new DataTable())
                            {
                                try
                                {
                                    dataAdapter.Fill(data);

                                    return data;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    return new DataTable();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public decimal GetSaldoFavor(string cliente)
        {
            using (var connection = new SqlConnection(SqlConectionString))
            {
                connection.Open();
                string sConsulta = "usp_GetSaldoaFavor";
                try
                {
                    var saldo = connection.ExecuteScalar<decimal>(sConsulta, new { CardCode = cliente }, commandType: CommandType.StoredProcedure);
                    return saldo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }
            }
        }

        public async Task<int> InsertarPagoAsync(CYC_PagosEncabezado pago)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Insertar encabezado
                var sqlEncabezado = @"
                INSERT INTO CYC_PagosEncabezado (
                    Fecha, Sociedad, Sucursal, IdCuenta, Cuenta,
                    CardCode, CardName, Monto, Referencia,
                    Descuento1, PorcDesc1, Descuento2, PorcDesc2,
                    Descuento3, PorcDesc3, Descuento4, PorcDesc4,
                    TotalAPagar, Estatus, Comentarios, FechaPago, BPLId, idEdoCta, TipoOp, usuario, FidValue, PagoaCuenta, SaldoaFavor)
                VALUES (
                    @Fecha, @Sociedad, @Sucursal, @IdCuenta, @Cuenta,
                    @CardCode, @CardName, @Monto, @Referencia,
                    @Descuento1, @PorcDesc1, @Descuento2, @PorcDesc2,
                    @Descuento3, @PorcDesc3, @Descuento4, @PorcDesc4,
                    @TotalAPagar, @Estatus, @Comentarios, @FechaPago, @BPLId, @IdEdoCta, @TipoOp, @Usuario, @FidValue, @PagoaCuenta, @SaldoaFavor);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var folioPago = await connection.ExecuteScalarAsync<int>(
                    sqlEncabezado, pago, transaction);

                // Insertar detalles
                var sqlDetalle = @"
                INSERT INTO CYC_PagosDetalle (
                    FolioPago, DocEntry, DocNum, SaldoVencido, RebjDev,
                    MontoDcto1, TDcto1, MontoDcto2, TDcto2,
                    MontoDcto3, TDcto3, MontoDcto4, TDcto4,
                    TotalPago, Manual, Tipo, Estatus, Comentarios, UUID, TransId)
                VALUES (
                    @FolioPago, @DocEntry, @DocNum, @SaldoVencido, @RebjDev,
                    @MontoDcto1, @TDcto1, @MontoDcto2, @TDcto2,
                    @MontoDcto3, @TDcto3, @MontoDcto4, @TDcto4,
                    @TotalPago, @Manual, @Tipo, @Estatus, @Comentarios, @UUID, @TransId);";

                foreach (var detalle in pago.Detalles)
                {
                    detalle.FolioPago = folioPago;
                    await connection.ExecuteAsync(sqlDetalle, detalle, transaction);
                }

                transaction.Commit();
                return folioPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        internal async Task<DataTable> GetResporDetails(string folio)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = string.Format("SELECT * FROM [DISPERSION].[dbo].[Detalle_InfOperaciones] WHERE FolioPago = '{0}'", folio);

                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.Text;
                new SqlDataAdapter(sqlCommand).Fill(table);

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task<DataTable> GetResporHeader(string fecha)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = string.Format("SELECT * FROM [DISPERSION].[dbo].[Encabezado_InfOperaciones] WHERE Fecha = '{0}'", fecha);
                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.Text;
                new SqlDataAdapter(sqlCommand).Fill(table);

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }
        internal async Task<DataTable> GetResportHeaderCuadroInversion(string fecha)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = string.Format("SELECT [Almacen] ,[Cuentas] ,[RendiminetoDiario] ,[SaldoDiario] ,[Compra] ,[Venta] ,[ActualChequera] ,[SaldoInversion] ,[TotalBanorte] ,[Depositos] ,[DepositosVenta] ,[Transferencias] ,[DepositosMes] ,[ChequeMes] ,[DolaresTCDia] ,[DolaresTCDiario] ,[GranTotal] ,[Banorte] ,[Banamex] ,[Santander] FROM [DISPERSION].[dbo].[Tesoreria_HistoricoAnalisisSaldoDiario] WHERE [FechaSD] = '{0}' ORDER BY [Orden] ASC", fecha);
                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.Text;
                new SqlDataAdapter(sqlCommand).Fill(table);

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }
        internal async Task<DataTable> GetOperationHeader()
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = string.Format("SELECT * FROM [DISPERSION].[dbo].[Borrador_Encabezado_InfOperaciones]");
                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.Text;
                new SqlDataAdapter(sqlCommand).Fill(table);

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task UpdateOperationHeader(string folio)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                string sConsulta = string.Format("UPDATE [DISPERSION].[dbo].[CYC_PagosEncabezado] SET TipoOp = 2 WHERE FolioPago = {0}", folio);
                await connection.ExecuteScalarAsync(sConsulta, transaction: transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<CYC_PagosEncabezado> GetPagoByIdAsync(int id)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();

            // Consultar encabezado
            var sqlEncabezado = @"SELECT * FROM CYC_PagosEncabezado WHERE FolioPago = @Id";
            var encabezado = await connection.QueryFirstOrDefaultAsync<CYC_PagosEncabezado>(sqlEncabezado, new { Id = id });

            if (encabezado == null)
                return null;

            // Consultar detalles
            var sqlDetalle = @"SELECT * FROM CYC_PagosDetalle WHERE FolioPago = @Id";
            var detalles = (await connection.QueryAsync<CYC_PagosDetalle>(sqlDetalle, new { Id = id })).ToList();

            encabezado.Detalles = detalles;
            return encabezado;
        }
        public async Task<bool> DeletePagoByIdAsync(int id)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Eliminar detalles primero
                var sqlDeleteDetalles = "DELETE FROM CYC_PagosDetalle WHERE FolioPago = @Id";
                await connection.ExecuteAsync(sqlDeleteDetalles, new { Id = id }, transaction);

                // Eliminar encabezado
                var sqlDeleteEncabezado = "DELETE FROM CYC_PagosEncabezado WHERE FolioPago = @Id";
                int rows = await connection.ExecuteAsync(sqlDeleteEncabezado, new { Id = id }, transaction);

                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Ejecuta saldo diario, transferencias, depósitos e histórico de análisis (OPENQUERY HANA + tablas DISPERSION).
        /// </summary>
        public async Task ExecuteCargarSaldosCuadroInversionAsync(CargarSaldosCuadroRequest request)
        {
            static DateTime ParseIso(string s, string name)
            {
                if (string.IsNullOrWhiteSpace(s) ||
                    !DateTime.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                    throw new ArgumentException($"La fecha '{name}' no es válida (use yyyy-MM-dd).");
                return d.Date;
            }

            var saldoDiario = ParseIso(request.SaldoDiario, nameof(request.SaldoDiario));
            var transDesde = ParseIso(request.TransferenciasDesde, nameof(request.TransferenciasDesde));
            var transHasta = ParseIso(request.TransferenciasHasta, nameof(request.TransferenciasHasta));
            var depDesde = ParseIso(request.DepositosDesde, nameof(request.DepositosDesde));
            var depHasta = ParseIso(request.DepositosHasta, nameof(request.DepositosHasta));

            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            await TesoreriaCargarSaldosExecutor.ExecuteAllAsync(connection, saldoDiario, transDesde, transHasta, depDesde, depHasta).ConfigureAwait(false);
        }

    }
}

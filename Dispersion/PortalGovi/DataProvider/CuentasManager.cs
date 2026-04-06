using Dapper;
using Microsoft.Extensions.Configuration;
using PortalGovi.Models.Configuracion;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.DataProvider
{
    public class CuentasManager
    {
        private string _connString;

        public CuentasManager(IConfiguration config)
        {
            _connString = config.GetConnectionString("SQL");
        }

        // 1. Obtener Cuentas Principales
        public async Task<IEnumerable<CuentaPrincipalModel>> GetPrincipales()
        {
            using var db = new SqlConnection(_connString);
            var sql = @"SELECT [Almacen], [RazonSocial], [Cuenta], [Orden] 
                        FROM [DISPERSION].[dbo].[Tesoreria_OrdendelosDatos] 
                        ORDER BY Orden DESC"; // Ordenamos por último creado
            return await db.QueryAsync<CuentaPrincipalModel>(sql);
        }

        // 2. Crear Cuenta Principal (Calculando Orden)
        public async Task<int> AddPrincipal(CuentaPrincipalModel model)
        {
            using var db = new SqlConnection(_connString);
            await db.OpenAsync();
            
            // Calculamos el siguiente ID
            var nextId = await db.ExecuteScalarAsync<int>(
                "SELECT ISNULL(MAX(Orden), 0) + 1 FROM [DISPERSION].[dbo].[Tesoreria_OrdendelosDatos]");
            
            model.Orden = nextId;

            var sql = @"INSERT INTO [DISPERSION].[dbo].[Tesoreria_OrdendelosDatos] 
                        (Almacen, RazonSocial, Cuenta, Orden)
                        VALUES (@Almacen, @RazonSocial, @Cuenta, @Orden)";
            
            await db.ExecuteAsync(sql, model);
            return nextId; // Retornamos el ID generado
        }

        // 3. Obtener Dependencias de una Cuenta (Por OrdenID)
        public async Task<DependenciasViewModel> GetDependencias(int ordenId)
        {
            using var db = new SqlConnection(_connString);
            var result = new DependenciasViewModel();

            // Cuentas USD
            var sqlUSD = @"SELECT IdUSD as Id, OrdenID, Almacen, RazonSocial, CuentaUSD as Cuenta 
                           FROM [DISPERSION].[dbo].[Tesoreria_CuentasUSD] 
                           WHERE OrdenID = @OrdenID";
            result.CuentasUSD = (await db.QueryAsync<CuentaDependienteModel>(sqlUSD, new { OrdenID = ordenId })).ToList();

            // Cuentas Referenciadas
            var sqlRef = @"SELECT IdReferencia as Id, OrdenID, Almacen, RazonSocial, CuentaReferenciada as Cuenta 
                           FROM [DISPERSION].[dbo].[Tesoreria_CuentasReferenciadas] 
                           WHERE OrdenID = @OrdenID";
            result.CuentasRef = (await db.QueryAsync<CuentaDependienteModel>(sqlRef, new { OrdenID = ordenId })).ToList();

            return result;
        }

        // 4. Agregar Dependencia USD
        public async Task AddUSD(CuentaDependienteModel model)
        {
            using var db = new SqlConnection(_connString);
            var sql = @"INSERT INTO [DISPERSION].[dbo].[Tesoreria_CuentasUSD] 
                        (OrdenID, Almacen, RazonSocial, CuentaUSD) 
                        VALUES (@OrdenID, @Almacen, @RazonSocial, @Cuenta)";
            await db.ExecuteAsync(sql, model);
        }

        // 5. Agregar Dependencia Referenciada
        public async Task AddReferenciada(CuentaDependienteModel model)
        {
            using var db = new SqlConnection(_connString);
            var sql = @"INSERT INTO [DISPERSION].[dbo].[Tesoreria_CuentasReferenciadas] 
                        (OrdenID, Almacen, RazonSocial, CuentaReferenciada) 
                        VALUES (@OrdenID, @Almacen, @RazonSocial, @Cuenta)";
            await db.ExecuteAsync(sql, model);
        }
    }
}

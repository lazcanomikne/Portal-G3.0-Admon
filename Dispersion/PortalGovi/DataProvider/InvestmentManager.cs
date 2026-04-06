using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PortalGovi.Models.Investment;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.DataProvider
{
    public class InvestmentManager
    {
        public string SqlConectionString;
        public InvestmentManager(IConfiguration configuration)
        {
            SqlConectionString = configuration.GetConnectionString("SQL");
        }

        public async Task<DataTable> GetSaldoDisponible(string fecha)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = string.Format("SELECT * FROM [DISPERSION].[dbo].[Tesoreria_SaldoDisponible_Detalle] WHERE Fecha = '{0}'", fecha);

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

        public async Task<int> InsertSaldoDisponible(SaldoDisponibleModel request)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                string sConsulta = "usp_InserSaldoDisponible";
                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@json", JsonConvert.SerializeObject(request));
                int result = await sqlCommand.ExecuteNonQueryAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteSaldoDisponible(int id)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                string sConsulta = string.Format("DELETE Tesoreria_SaldoDisponible_Detalle WHERE ID_Encabezado = {0};" +
                    "DELETE Tesoreria_SaldoDisponible_Encabezado WHERE Id = {0};", id);
                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.Text;                
                int result = await sqlCommand.ExecuteNonQueryAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTable> GetSaldoFijo()
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = "SELECT [ID] AS [id], [Titular] AS [titular], [Cuenta] AS [cuenta], [SaldoFijo] AS [saldoFijo] FROM [DISPERSION].[dbo].[Tesoreria_SaldoFijo]";
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
        public async Task<int> InsertSaldoFijo(SaldoFijoModel request)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Insertar encabezado
                var sqlEncabezado = @"
                INSERT INTO Tesoreria_SaldoFijo (
                    Titular, Cuenta, SaldoFijo)
                VALUES (
                    @Titular, @Cuenta, @SaldoFijo);";

                var folioPago = await connection.ExecuteScalarAsync<int>(
                    sqlEncabezado, request, transaction);                

                transaction.Commit();
                return folioPago;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> UpdateSaldoFijo(int id, SaldoFijoModel request)
        {
            request.Id = id;
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Actualizar registro
                var sqlEncabezado = @"
                UPDATE [DISPERSION].[dbo].[Tesoreria_SaldoFijo] 
                SET [Titular] = @Titular, 
                    [Cuenta] = @Cuenta, 
                    [SaldoFijo] = @SaldoFijo
                WHERE [ID] = @Id;";

                var affectedRows = await connection.ExecuteAsync(
                    sqlEncabezado, request, transaction);

                transaction.Commit();
                return affectedRows;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<int> DeleteSaldoFijo(int id)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                string sConsulta = string.Format("DELETE Tesoreria_SaldoFijo WHERE Id = {0};", id);
                SqlCommand sqlCommand = new SqlCommand(sConsulta, connection);
                sqlCommand.CommandType = CommandType.Text;
                int result = await sqlCommand.ExecuteNonQueryAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<DataTable> GetApartados(string fecha)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            DataTable table = new DataTable();
            try
            {
                string sConsulta = string.Format("SELECT [ID],[Cuenta],[MontoApartar],[Fecha] FROM [DISPERSION].[dbo].[Tesoreria_Apartados] WHERE CAST(Fecha AS DATE) = '{0}'", fecha);
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

        public async Task<int> InsertApartado(ApartadoModel request)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                var sql = @"
                INSERT INTO [DISPERSION].[dbo].[Tesoreria_Apartados] (Cuenta, MontoApartar, Fecha)
                VALUES (@Cuenta, @MontoApartar, @Fecha)";
                
                return await connection.ExecuteAsync(sql, request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> InsertApartadosList(System.Collections.Generic.List<ApartadoModel> request)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var sql = @"
                INSERT INTO [DISPERSION].[dbo].[Tesoreria_Apartados] (Cuenta, MontoApartar, Fecha)
                VALUES (@Cuenta, @MontoApartar, @Fecha)";
                
                var result = await connection.ExecuteAsync(sql, request, transaction);
                transaction.Commit();
                return result;
            }
            catch(Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<int> UpdateApartado(ApartadoModel request)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                var sql = @"
                UPDATE [DISPERSION].[dbo].[Tesoreria_Apartados] 
                SET Cuenta = @Cuenta, 
                    MontoApartar = @MontoApartar, 
                    Fecha = @Fecha
                WHERE ID = @ID";
                
                return await connection.ExecuteAsync(sql, request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteApartado(int id)
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                var sql = "DELETE FROM [DISPERSION].[dbo].[Tesoreria_Apartados] WHERE ID = @ID";
                return await connection.ExecuteAsync(sql, new { ID = id });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<System.Collections.Generic.List<CuentaOrdenModel>> GetCuentasOrden()
        {
            using var connection = new SqlConnection(SqlConectionString);
            await connection.OpenAsync();
            try
            {
                string sConsulta = "SELECT [Almacen], [Cuenta] FROM [DISPERSION].[dbo].[Tesoreria_OrdendelosDatos]";
                var result = await connection.QueryAsync<CuentaOrdenModel>(sConsulta);
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

using Microsoft.Extensions.Configuration;
using PortalGovi.Models.Cancelacion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;
using PortalGovi.Models;
using System.Net;
using System.Text;

namespace PortalGovi.DataProvider
{
    public class CancelacionManager
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString;


        public CancelacionManager(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("Cancelacion");
        }

        public async Task<IEnumerable<CancelacionMasiva>> GetMasics(string Fecha)
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                await con.OpenAsync();
                var sql = $"SELECT * FROM [dbo].[CancelacionMasivaUUID] WHERE CONVERT(date, Fecha) = '{Fecha}'";
                var result = await con.QueryAsync<CancelacionMasiva>(sql);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> CargarData(List<CancelacionMasiva> data)
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                con.Open();
                foreach (var item in data)
                {
                    SqlCommand sqlCommand2 = new SqlCommand(@"INSERT INTO [dbo].[CancelacionMasivaUUID] ([RFC_Emisor], [RFC_Receptor],[Total_UUID],[UUID],[Motivo],[Usuario])
                                                                                        VALUES (@RFC_Emisor, @RFC_Receptor, @Total_UUID, @UUID, @Motivo, @Usuario)", con);
                    sqlCommand2.Parameters.AddWithValue("@RFC_Emisor", item.RFC_Emisor);
                    sqlCommand2.Parameters.AddWithValue("@RFC_Receptor", item.RFC_Receptor);
                    sqlCommand2.Parameters.AddWithValue("@Total_UUID", item.Total_UUID);
                    sqlCommand2.Parameters.AddWithValue("@UUID", item.UUID);
                    sqlCommand2.Parameters.AddWithValue("@Motivo", item.Motivo);
                    sqlCommand2.Parameters.AddWithValue("@Usuario", item.Usuario);
                    sqlCommand2.ExecuteNonQuery();
                }
                await ProcesarData(data);
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task ProcesarData(List<CancelacionMasiva> data)
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                await con.OpenAsync();
                var listUUID = "'" + string.Join("','", data.Select(u => u.UUID).ToList()) + "'";
                var sql = $"SELECT * FROM [dbo].[CancelacionMasivaUUID] WHERE UUID IN ({listUUID})";
                var result = await con.QueryAsync<CancelacionMasiva>(sql);
                if (result.Count() > 0)
                {
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                    HttpClient httpClient = new HttpClient(handler);
                    foreach (var item in result)
                    {
                        var cred = con.Query<Credencial>($"SELECT  id, id_Empresa, token, Certificado FROM  Credenciales WHERE RFC_Emisor = '{item.RFC_Emisor}'").FirstOrDefault();

                        var input = new CancelationRequest()
                        {
                            credentials = new Credentials()
                            {
                                id = cred.id_Empresa,
                                token = cred.token
                            },
                            issuer = new Issuer()
                            {
                                rfc = item.RFC_Emisor
                            },
                            document = new Document()
                            {
                                certificatenumber = cred.Certificado,
                                rfc_receptor = item.RFC_Receptor,
                                total_cfdi = item.Total_UUID,
                                motive = item.Motivo
                            }
                        };

                        var content = new StringContent(
                        JsonConvert.SerializeObject(input),
                        Encoding.UTF8,
                        "application/json");
                        var response2 = await httpClient.PutAsync($"https://servicios.diverza.com/api/v1/documents/{item.UUID}/cancel", content);
                        Console.WriteLine("Result {0}", response2.Content.ReadAsStringAsync().Result);

                    }
                    await UpdateData(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to processing data: {ex.Message}");
            }
        }

        private async Task UpdateData(IEnumerable<CancelacionMasiva> result)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                HttpClient httpClient = new HttpClient(handler);
                using var con = new SqlConnection(ConnectionString);
                await con.OpenAsync();
                foreach (var item in result)
                {
                    var cred = con.Query<Credencial>($"SELECT  id, id_Empresa, token, Certificado FROM  Credenciales WHERE RFC_Emisor = '{item.RFC_Emisor}'").FirstOrDefault();
                    if (cred == null) continue;
                    var input = new
                    {
                        credentials = new Credentials()
                        {
                            id = cred.id_Empresa,
                            token = cred.token
                        },
                    };
                    var content = new StringContent(
                            JsonConvert.SerializeObject(input),
                            Encoding.UTF8,
                            "application/json");
                    var response2 = await httpClient.PutAsync($"https://servicios.diverza.com/api/v1/documents/{item.UUID}/sat_cfdi_enquiry", content);
                    var resultRequest = JsonConvert.DeserializeObject<CancelationStatus>(response2.Content.ReadAsStringAsync().Result);
                    string sql = $"UPDATE [CancelacionMasivaUUID] SET Estatus = '{resultRequest.estado}' WHERE Id = {item.id}";
                    SqlCommand sqlCmd2 = new SqlCommand(sql, con);
                    sqlCmd2.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update data: {ex.Message}");
            }
        }

        public async Task UpdatePending()
        {
            try
            {
                using var con = new SqlConnection(ConnectionString);
                await con.OpenAsync();
                var sql = @"SELECT * FROM [dbo].[CancelacionMasivaUUID] WHERE Estatus IS NULL ORDER BY 1 DESC";
                var result = await con.QueryAsync<CancelacionMasiva>(sql);
                if (result.Count() > 0)
                {
                    await UpdateData(result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

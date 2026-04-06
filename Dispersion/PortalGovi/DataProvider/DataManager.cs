using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
//using RestSharp;
using Sap.Data.Hana;
using PortalGovi.Models;
using System.IO;
using System.Text;
using System.Text.Json;
using SAPbobsCOM;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Dynamic;

namespace PortalGovi.DataProvider
{
    public class DataManager
    {
        private readonly IConfiguration _configuration;
        public Company oCompany;
        public HanaConnection connH;
        public string ConnectionString;// = _configuration.GetConnectionString("Sap");//"Server=192.168.1.30:30015;UserID=SYSTEM;Password=Pa$$w0rd!";
        public string SqlConectionString;
        public string SqlConectionStringMigrarTxt;
        public string SqlConectionStringAjustes;

        public DataManager(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("Sap");
            SqlConectionString = _configuration.GetConnectionString("SQL");
            SqlConectionStringMigrarTxt = _configuration.GetConnectionString("MigrarTxt");
            SqlConectionStringAjustes = _configuration.GetConnectionString("Ajustes");
        }

        //Conexion a Hana
        private void SetConnectionContext()
        {
            try
            {
                connH = new HanaConnection("");
                connH.Open();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /*Dispersion pagos*/
        public List<Sociedad> ObtenerSociedades()
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " SELECT \"Code\",\"U_RevOffice\",\"U_CompnyName\", \"U_DB\" FROM \"SBOGOVI\".\"@SOCIEDADES\" ";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                var list = data.AsEnumerable().Select(row =>
                                new Sociedad
                                {
                                    Code = (string)row["Code"],
                                    U_RevOffice = (string)row["U_RevOffice"],
                                    U_CompnyName = (string)row["U_CompnyName"],
                                    U_DB = (string)row["U_DB"],
                                }).ToList();

                                return list;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ObtenerSociedades: {ex.Message}", ex);
            }
        }
        public List<Sucursal> ObtenerSucursales(string sociedad)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " Select L0.\"BPLId\", L0.\"BPLName\", L0.\"BPLFrName\", L1.\"Name\" From \"" + sociedad + "\".\"OBPL\" L0 LEFT JOIN \"" + sociedad + "\".\"OCST\" L1 ON L1.\"Code\" = L0.\"State\" LEFT JOIN \"" + sociedad + "\".\"OCRY\" L2 ON L2.\"Code\" = L0.\"Country\" WHERE L0.\"BPLFrName\" IS NOT NULL ";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                var list = data.AsEnumerable().Select(row =>
                                new Sucursal
                                {
                                    BPLId = (int)row["BPLId"],
                                    BPLName = (string)row["BPLName"],
                                    BPLFrName = (string)row["BPLFrName"],
                                    Name = (string)row["Name"],
                                }).ToList();

                                return list;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ObtenerSucursales('{sociedad}'): {ex.Message}", ex);
            }
        }
        public List<Transferencia> ObtenerTransferencias(string sociedad, string sucursal, string cuenta, string operacion, int year)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format(Properties.Resources.SQL_TRANSFERENCIAS, sociedad == null ? "" : "WHERE \"BankCtlKey\" = '" + operacion + "' AND \"Sociedad\" = '" + sociedad + "' AND \"BPLName\" = '" + sucursal + "' AND \"GLAccount\" = '" + cuenta + "' AND YEAR(\"DocDate\") = "+ year + ""); ;

                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        // if (sociedad == null)
                        // {
                        using (HanaDataAdapter dataAdapter = new HanaDataAdapter(cmdHSAP))
                        {
                            using (DataTable data = new DataTable())
                            {
                                try
                                {
                                    dataAdapter.Fill(data);
                                    var list = data.AsEnumerable().Select(row =>
                                        new Transferencia
                                        {
                                            Sociedad = (string)row["Sociedad"],
                                            VATRegNum = (string)row["VATRegNum"],
                                            BankCtlKey = (string)row["BankCtlKey"],
                                            MandateID = (string)row["MandateID"],
                                            Account = (string)row["Account"],
                                            DflAccount = (string)row["DflAccount"],
                                            DocTotal = decimal.Parse(row["DocTotal"].ToString()),
                                            DocNum = int.Parse(row["DocNum"].ToString()),
                                            JrnlMemo = (string)row["JrnlMemo"],
                                            County = (string)row["County"],
                                            LicTradNum = (string)row["LicTradNum"],
                                            IVA = (string)row["IVA"],
                                            E_Mail = (string)row["E_Mail"],
                                            DocDate = (DateTime)row["DocDate"],
                                            CardName = (string)row["CardName"],
                                            DocEntry = int.Parse(row["DocEntry"].ToString())
                                        }).ToList();

                                    return list;
                                }
                                catch (Exception)
                                {
                                    throw;
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
        public dynamic GetPasivo(string sociedad, string sucursal, string cuenta)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    string sConsulta = string.Format("CALL \"STS_GROB\".\"GET_Pasivo_Portal_Dispersion_xSucursal\" ('{0}','{1}','{2}')", sociedad, sucursal, cuenta);
                    connH.Open();
                    if (string.IsNullOrEmpty(sucursal) || string.IsNullOrEmpty(cuenta)) sConsulta = string.Format("CALL \"STS_GROB\".\"GET_Pasivo_Portal_Dispersion\" ('{0}')", sociedad);

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
                                            Sociedad = (string)row["Sociedad"],
                                            VATRegNum = (string)row["VATRegNum"],
                                            AcctCode = (string)row["AcctCode"],
                                            AcctName = (string)row["AcctName"],
                                            SeriesName = (string)row["SeriesName"],
                                            Series = int.Parse(row["Series"].ToString()),
                                            SaldoPendiente = decimal.Parse(row["SaldoPendiente"].ToString()),
                                            DocTotal = decimal.Parse(row["DocTotal"].ToString()),
                                            PaidToDate = decimal.Parse(row["PaidToDate"].ToString()),
                                            TransferSum = decimal.Parse(row["TransferSum"].ToString()),
                                            DocNum = int.Parse(row["DocNum"].ToString()),
                                            CardCode = (string)row["CardCode"],
                                            CardName = (string)row["CardName"],
                                            DocEntry = int.Parse(row["DocEntry"].ToString()),
                                            BPLId = row["BPLId"],
                                            BPLName = row["BPLName"],
                                            JournalRemarks = row["JournalRemarks"],
                                            Comments = row["Comments"],
                                        }).ToList();

                                    return list;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    return new List() { };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<string> { };
                throw;
            }
        }
        public dynamic GetTransferencias(string sociedad, string fecha1, string fecha2)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("CALL \"STS_GROB\".\"Global_Transfer_Dispersion\" ('{1}','{2}','{0}')", sociedad, fecha1, fecha2);

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
                                            Sociedad = (string)row["Sociedad"],
                                            VATRegNum = (string)row["VATRegNum"],
                                            BankCtlKey = (string)row["BankCtlKey"],
                                            MandateID = (string)row["MandateID"],
                                            Account = (string)row["Account"],
                                            DflAccount = (string)row["DflAccount"],
                                            DocTotal = decimal.Parse(row["DocTotal"].ToString()),
                                            DocNum = int.Parse(row["DocNum"].ToString()),
                                            JrnlMemo = (string)row["JrnlMemo"],
                                            County = (string)row["County"],
                                            LicTradNum = (string)row["LicTradNum"],
                                            IVA = (string)row["IVA"],
                                            E_Mail = (string)row["E_Mail"],
                                            DocDate = (DateTime)row["DocDate"],
                                            CardName = (string)row["CardName"],
                                            DocEntry = int.Parse(row["DocEntry"].ToString()),
                                            DocNum2 = row["DocNum2"],
                                            BPLName = row["BPLName"],
                                            GLAccount = row["GLAccount"],
                                            U_dispersion = row["U_dispersion"].ToString().Equals("si")
                                        }).ToList();

                                    return list;
                                }
                                catch (Exception ex)
                                {
                                    throw;
                                    return new List() { };
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
        public DataTable GetCuadroInversion(string[] fechas)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("CALL \"STS_GROB\".\"CuadrodeInversion\" ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", fechas[0], fechas[1], fechas[2], fechas[3], fechas[4], fechas[5], fechas[6]);

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new DataTable();
            }
        }
        public async Task UpdateDispersion(string sociedad, string usuario, string pass, JsonDocument transf)
        {
            try
            {
                IEnumerable<string> cookies = null;
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                HttpClient httpClient = new HttpClient(handler);
                var d = new LoginApiData
                {
                    CompanyDB = sociedad,
                    UserName = usuario,
                    Password = pass
                };
                var datalogin = new StringContent(
                    JsonConvert.SerializeObject(d),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                if (response.IsSuccessStatusCode)
                {
                    cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                }
                foreach (JsonElement item in transf.RootElement.EnumerateArray())
                {
                    //Actualizar, Aqui es donde se puede estar errando
                    var jsonobj = new { U_dispersion = item.GetProperty("u_dispersion").ToString() };
                    var udispersion = new StringContent(
                        JsonConvert.SerializeObject(jsonobj),
                        Encoding.UTF8,
                        "application/json");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments({item.GetProperty("docNum").ToString()})", udispersion);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<Cuenta> ObtenerCuentas(string sociedad, string sucursal)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " Select \"GLAccount\", \"Account\", \"AcctName\" From \"" + sociedad + "\".\"DSC1\" WHERE \"Branch\" = '" + sucursal + "' ";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                var list = data.AsEnumerable().Select(row =>
                                 new Cuenta
                                 {
                                     GLAccount = (string)row["GLAccount"],
                                     Account = (string)row["Account"],
                                     AcctName = (string)row["AcctName"],
                                 }).ToList();

                                return list;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public string GenerarArchivo(List<Transferencia> transferencias, string sociedad, string sucursal, string tipo, string u, string p)
        {
            string sResult = GenerarTxt(transferencias, sociedad, sucursal, tipo, u, p).Result;

            return sResult;
        }
        private async Task<string> GenerarTxt(List<Transferencia> transferencias, string sociedad, string sucursal, string tipo, string u, string p)
        {
            string pdf = "";
            try
            {
                IEnumerable<string> cookies = null;
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                HttpClient httpClient = new HttpClient(handler);
                var d = new LoginApiData
                {
                    CompanyDB = sociedad,
                    UserName = u,
                    Password = p
                };
                var datalogin = new StringContent(
                    JsonConvert.SerializeObject(d),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                if (response.IsSuccessStatusCode)
                {
                    cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                }

                Stream stream = null;
                StreamWriter streamWriter = null;
                DateTime date = DateTime.Now;
                string dia = date.Day.ToString().PadLeft(2, '0');
                string mes = date.Month.ToString().PadLeft(2, '0');
                string anio = date.Year.ToString().Substring(2);
                string tipoCta = (tipo == "04" || tipo == "4") ? "SPEI" : "PAGTER";
                string nombreArchivo = "";
                /**/
                int folioArchivo = 0;
                StreamReader reader = new StreamReader("folio.txt");
                folioArchivo = int.Parse(reader.ReadLine()) + 1;
                reader.Close();
                StreamWriter writer = new StreamWriter("folio.txt");
                writer.Write(folioArchivo);
                writer.Close();
                /**/
                string U_RutaArchivo;
                string U_RutaReporte;
                string U_RutaImagenes;
                string U_CompnyName;
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " SELECT \"U_CompnyName\", \"U_RutaArchivo\", \"U_RutaReporte\", \"U_RutaImagenes\" FROM \"SBOGOVI\".\"@SOCIEDADES\" WHERE  \"U_DB\" = '" + sociedad + "' ";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                U_RutaArchivo = (string)data.Rows[0]["U_RutaArchivo"];
                                U_RutaReporte = (string)data.Rows[0]["U_RutaReporte"];
                                U_RutaImagenes = (string)data.Rows[0]["U_RutaImagenes"];
                                U_CompnyName = (string)data.Rows[0]["U_CompnyName"];
                            }
                        }
                    }
                }
                string nombreSucursal;
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " Select L0.\"BPLFrName\" From \"" + sociedad + "\".\"OBPL\" L0 LEFT JOIN \"" + sociedad + "\".\"OCST\" L1 ON L1.\"Code\" = L0.\"State\" LEFT JOIN \"" + sociedad + "\".\"OCRY\" L2 ON L2.\"Code\" = L0.\"Country\" WHERE L0.\"BPLFrName\" IS NOT NULL AND LENGTH(L0.\"BPLName\") <= 3 AND L0.\"BPLName\" = '" + sucursal + "'";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                nombreSucursal = (string)data.Rows[0]["BPLFrName"];
                            }
                        }
                    }
                }

                nombreArchivo = tipoCta + "-" + dia + mes + anio + "-" + nombreSucursal + "-FOLIO" + folioArchivo;
                string pathArchivo = U_RutaArchivo + "\\" + nombreArchivo + ".txt";
                if (!Directory.Exists(U_RutaArchivo))
                {
                    Directory.CreateDirectory(U_RutaArchivo);
                }
                if (File.Exists(pathArchivo))
                {
                    File.Delete(pathArchivo);
                    stream = File.Create(pathArchivo);
                }
                else
                {
                    stream = File.Create(pathArchivo);
                }
                //stream = File.Create(Path.Combine("Files", nombreArchivo + ".txt"));
                streamWriter = new StreamWriter(stream, Encoding.Default);
                StringBuilder renglon = new StringBuilder();
                foreach (Transferencia transferencia in transferencias)
                {
                    renglon.Append(transferencia.BankCtlKey.ToString().PadLeft(2, '0'));
                    renglon.Append(transferencia.MandateID.ToString().PadRight(13));
                    renglon.Append(transferencia.Account.ToString().PadLeft(20, '0'));
                    renglon.Append(transferencia.DflAccount.ToString().PadLeft(20, '0'));
                    string importe = String.Format("{0:00.00}", transferencia.DocTotal).Replace(".", "").PadLeft(14, '0');
                    renglon.Append(importe);
                    renglon.Append(transferencia.DocNum.ToString().PadLeft(10, '0'));
                    renglon.Append(transferencia.JrnlMemo.Length > 30 ? transferencia.JrnlMemo.Substring(0, 30) : transferencia.JrnlMemo.ToString().PadRight(30));
                    renglon.Append(transferencia.County == "MXP" ? "1" : "2");
                    renglon.Append(transferencia.County == "MXP" ? "1" : "2");
                    renglon.Append(transferencia.LicTradNum.Length < 13 ? transferencia.LicTradNum.PadRight(13) : transferencia.LicTradNum.ToString());
                    renglon.Append(transferencia.IVA.ToString().PadLeft(14, '0'));
                    renglon.Append(transferencia.E_Mail.Length > 39 ? transferencia.E_Mail.Substring(0, 38) : transferencia.E_Mail.ToString().PadRight(39));
                    renglon.Append(transferencia.DocDate.ToString("ddMMyyyy"));
                    string instruccion = transferencia.CardName.Length > 70 ? transferencia.CardName.Substring(0, 69) : transferencia.CardName.ToString().PadRight(69);
                    renglon.Append(instruccion);
                    renglon.Append(".");
                    renglon.AppendLine((string)null);
                    //Actualizar, Aqui es donde se puede estar errando
                    var jsonobj = new { U_dispersion = "si" };
                    var udispersion = new StringContent(
                        JsonConvert.SerializeObject(jsonobj),
                        Encoding.UTF8,
                        "application/json");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments({transferencia.DocEntry})", udispersion);

                    if (!(response2.StatusCode == HttpStatusCode.NoContent))
                    {
                        streamWriter.Close();
                        return response2.Content.ReadAsStringAsync().Result;//
                    }
                    GuardarDispersion(transferencia, 0, u, nombreArchivo);
                }
                //Escribir
                streamWriter.Write(renglon);
                streamWriter.Close();
                //Reporte
                //string pathReporte = Path.Combine("Files", nombreArchivo + ".pdf");
                string pathReporte = U_RutaReporte + "\\" + nombreArchivo + ".pdf";
                //Crear pdf
                if (GeneraPDF(pathReporte, U_RutaImagenes, tipoCta, folioArchivo.ToString(), nombreSucursal, transferencias, U_CompnyName))
                {
                    if (File.Exists(pathReporte))
                    {
                        //File.Copy(pathReporte, Path.Combine(dest, nombreArchivo + ".pdf"));
                    }
                }
                pdf = nombreArchivo + ".pdf";
            }
            catch (Exception ex)
            {
                pdf = ex.Message.Replace(".pdf", "");
            }
            return pdf;
        }
        public List<string> GenerarArchivoByone(List<Transferencia> transferencias, string u, string p)
        {
            List<string> sResult = GenerarTxtByone(transferencias, u, p).Result;

            return sResult;
        }
        public string GenerarArchivoByone_(List<Transferencia> transferencias, string u, string p)
        {
            string sResult = GenerarTxtByone_(transferencias, u, p).Result;

            return sResult;
        }
        private async Task<List<string>> GenerarTxtByone(List<Transferencia> transferencias, string u, string p)
        {
            List<string> pdf = new List<string>();
            try
            {
                string sociedad, sucursal;
                foreach (Transferencia transferencia in transferencias)
                {
                    sociedad = transferencia.Sociedad;
                    sucursal = transferencia.VATRegNum;
                    IEnumerable<string> cookies = null;
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                    HttpClient httpClient = new HttpClient(handler);
                    var d = new LoginApiData
                    {
                        CompanyDB = sociedad,
                        UserName = u,
                        Password = p
                    };
                    var datalogin = new StringContent(
                        JsonConvert.SerializeObject(d),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                    if (response.IsSuccessStatusCode)
                    {
                        cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                    }

                    Stream stream = null;
                    StreamWriter streamWriter = null;
                    DateTime date = DateTime.Now;
                    string dia = date.Day.ToString().PadLeft(2, '0');
                    string mes = date.Month.ToString().PadLeft(2, '0');
                    string anio = date.Year.ToString().Substring(2);
                    string tipoCta = (transferencia.BankCtlKey == "04" || transferencia.BankCtlKey == "4") ? "SPEI" : "PAGTER";
                    string nombreArchivo = "";
                    string U_RutaArchivo;
                    string U_RutaReporte;
                    string U_RutaImagenes;
                    string U_CompnyName;
                    using (HanaConnection connH = new HanaConnection(ConnectionString))
                    {
                        connH.Open();
                        string sConsulta = " SELECT \"U_CompnyName\", \"U_RutaArchivo\", \"U_RutaReporte\", \"U_RutaImagenes\" FROM \"SBOGOVI\".\"@SOCIEDADES\" WHERE  \"U_DB\" = '" + sociedad + "' ";
                        using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                        {
                            using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                            {
                                using (DataTable data = new DataTable())
                                {
                                    data.Load(dataReader);
                                    U_RutaArchivo = (string)data.Rows[0]["U_RutaArchivo"];
                                    U_RutaReporte = (string)data.Rows[0]["U_RutaReporte"];
                                    U_RutaImagenes = (string)data.Rows[0]["U_RutaImagenes"];
                                    U_CompnyName = (string)data.Rows[0]["U_CompnyName"];
                                }
                            }
                        }
                    }
                    string nombreSucursal = sucursal;

                    if (!Directory.Exists(U_RutaArchivo))
                    {
                        Directory.CreateDirectory(U_RutaArchivo);
                    }
                    //pdf = pathReporte;
                    if (!Directory.Exists(U_RutaReporte))
                    {
                        Directory.CreateDirectory(U_RutaReporte);
                    }

                    /**/
                    int folioArchivo = 0;
                    StreamReader reader = new StreamReader("folio.txt");
                    folioArchivo = int.Parse(reader.ReadLine()) + 1;
                    reader.Close();
                    StreamWriter writer = new StreamWriter("folio.txt");
                    writer.Write(folioArchivo);
                    writer.Close();
                    /**/
                    nombreArchivo = tipoCta + "-" + dia + mes + anio + "-" + nombreSucursal + "-FOLIO" + folioArchivo;
                    string pathArchivo = U_RutaArchivo + "\\" + nombreArchivo + ".txt";
                    if (File.Exists(pathArchivo))
                    {
                        File.Delete(pathArchivo);
                        stream = File.Create(pathArchivo);
                    }
                    else
                    {
                        stream = File.Create(pathArchivo);
                    }
                    streamWriter = new StreamWriter(stream, Encoding.Default);
                    StringBuilder renglon = new StringBuilder();
                    renglon.Append(transferencia.BankCtlKey.ToString().PadLeft(2, '0'));
                    renglon.Append(transferencia.MandateID.ToString().PadRight(13));
                    renglon.Append(transferencia.Account.ToString().PadLeft(20, '0'));
                    renglon.Append(transferencia.DflAccount.ToString().PadLeft(20, '0'));
                    string importe = String.Format("{0:00.00}", transferencia.DocTotal).Replace(".", "").PadLeft(14, '0');
                    renglon.Append(importe);
                    renglon.Append(transferencia.DocNum.ToString().PadLeft(10, '0'));
                    renglon.Append(transferencia.JrnlMemo.Length > 30 ? transferencia.JrnlMemo.Substring(0, 30) : transferencia.JrnlMemo.ToString().PadRight(30));
                    renglon.Append(transferencia.County == "MXP" ? "1" : "2");
                    renglon.Append(transferencia.County == "MXP" ? "1" : "2");
                    renglon.Append(transferencia.LicTradNum.Length < 13 ? transferencia.LicTradNum.PadRight(13) : transferencia.LicTradNum.ToString());
                    renglon.Append(transferencia.IVA.ToString().PadLeft(14, '0'));
                    renglon.Append(transferencia.E_Mail.Length > 39 ? transferencia.E_Mail.Substring(0, 38) : transferencia.E_Mail.ToString().PadRight(39));
                    renglon.Append(transferencia.DocDate.ToString("ddMMyyyy"));
                    string instruccion = transferencia.CardName.Length > 70 ? transferencia.CardName.Substring(0, 69) : transferencia.CardName.ToString().PadRight(69);
                    renglon.Append(instruccion);
                    renglon.Append(".");
                    renglon.AppendLine((string)null);
                    //Actualizar
                    var jsonobj = new { U_dispersion = "si" };
                    var udispersion = new StringContent(
                        JsonConvert.SerializeObject(jsonobj),
                        Encoding.UTF8,
                        "application/json");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments({transferencia.DocEntry})", udispersion);

                    if (!(response2.StatusCode == HttpStatusCode.NoContent))
                    {
                        streamWriter.Close();
                        throw new Exception();
                    }

                    //Escribir
                    streamWriter.Write(renglon);
                    streamWriter.Close();
                    //Reporte
                    //string pathReporte = Path.Combine("Files", nombreArchivo + ".pdf");
                    string pathReporte = U_RutaReporte + "\\" + nombreArchivo + ".pdf";
                    //Crear pdf
                    if (GeneraPDF(pathReporte, U_RutaImagenes, tipoCta, folioArchivo.ToString(), nombreSucursal, transferencia, U_CompnyName))
                    {

                        if (File.Exists(pathReporte))
                        {
                            //File.Copy(pathReporte, Path.Combine(dest, nombreArchivo + ".pdf"));
                        }
                    }
                    pdf.Add(nombreArchivo + ".pdf");
                    GuardarDispersion(transferencia, 1, u, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return pdf;
        }
        private async Task<string> GenerarTxtByone_(List<Transferencia> transferencias, string u, string p)
        {
            string pdf = "Dispersion realizada correctamente";
            try
            {
                string sociedad, sucursal;
                foreach (Transferencia transferencia in transferencias)
                {
                    sociedad = transferencia.Sociedad;
                    sucursal = transferencia.VATRegNum;
                    IEnumerable<string> cookies = null;
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                    HttpClient httpClient = new HttpClient(handler);
                    var d = new LoginApiData
                    {
                        CompanyDB = sociedad,
                        UserName = u,
                        Password = p
                    };
                    var datalogin = new StringContent(
                        JsonConvert.SerializeObject(d),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                    if (response.IsSuccessStatusCode)
                    {
                        cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                    }
                    //Actualizar
                    var jsonobj = new { U_dispersion = "si" };
                    var udispersion = new StringContent(
                        JsonConvert.SerializeObject(jsonobj),
                        Encoding.UTF8,
                        "application/json");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments({transferencia.DocEntry})", udispersion);

                    if (!(response2.StatusCode == HttpStatusCode.NoContent))
                    {
                        return response2.Content.ReadAsStringAsync().Result;//
                    }

                    GuardarDispersion(transferencia, 1, u, "generado sin archivo");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return pdf;
        }
        private void GuardarDispersion(Transferencia transferencia, int tipo, string u, string nArchivo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlConectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(@"INSERT INTO dbo.FoliosDispersion (Sociedad, VATRegNum, BankCtlKey, MandateID, Account, DflAccount, DocTotal, DocNum, JrnlMemo, County, LicTradNum, IVA, E_Mail, DocDate, CardName, DocNum2, DocEntry, nArchivo, Usuario, FechaGen, Tipo)
                                                                                           VALUES (@sociedad, @vatregnum, @bankctlkey, @mandateid, @account, @dflaccount, @doctotal, @docnum, @jrnlmemo, @county, @lictradnum, @iva, @e_mail, @docdate, @cardname, @docnum2, @docentry, @narchivo, @usuario, @fechagen, @tipogen)", connection))
                    {
                        command.Parameters.AddWithValue("@sociedad", transferencia.Sociedad);
                        command.Parameters.AddWithValue("@vatregnum", transferencia.VATRegNum);
                        command.Parameters.AddWithValue("@bankctlkey", transferencia.BankCtlKey);
                        command.Parameters.AddWithValue("@mandateid", transferencia.MandateID);
                        command.Parameters.AddWithValue("@account", transferencia.Account);
                        command.Parameters.AddWithValue("@dflaccount", transferencia.DflAccount);
                        command.Parameters.AddWithValue("@doctotal", transferencia.DocTotal);
                        command.Parameters.AddWithValue("@docnum", transferencia.DocNum);
                        command.Parameters.AddWithValue("@jrnlmemo", transferencia.JrnlMemo);
                        command.Parameters.AddWithValue("@county", transferencia.County);
                        command.Parameters.AddWithValue("@lictradnum", transferencia.LicTradNum);
                        command.Parameters.AddWithValue("@iva", transferencia.IVA);
                        command.Parameters.AddWithValue("@e_mail", transferencia.E_Mail);
                        command.Parameters.AddWithValue("@docdate", transferencia.DocDate);
                        command.Parameters.AddWithValue("@cardname", transferencia.CardName);
                        command.Parameters.AddWithValue("@docnum2", transferencia.DocNum);
                        command.Parameters.AddWithValue("@docentry", transferencia.DocEntry);
                        command.Parameters.AddWithValue("@narchivo", nArchivo);
                        command.Parameters.AddWithValue("@usuario", u);
                        command.Parameters.AddWithValue("@fechagen", DateTime.Now);
                        command.Parameters.AddWithValue("@tipogen", tipo);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool GeneraPDF(string rutaReporte, string rutaImagenes, string tipoCta, string folio, string suc, List<Transferencia> transferencias, string nomSociedad)
        {
            bool b = true;
            try
            {
                int thisDay = DateTime.Now.Day;
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                string mesCadena = string.Empty;
                string sTransferencia;
                string sBeneficiario;
                string sCuentaOrigen;
                string sCuentaDestino;
                string sImporte;
                string sFecha;
                decimal dTotal = 0m;
                switch (thisMonth)
                {
                    case 1:
                        mesCadena = "Enero";
                        break;
                    case 2:
                        mesCadena = "Febrero";
                        break;
                    case 3:
                        mesCadena = "Marzo";
                        break;
                    case 4:
                        mesCadena = "Abril";
                        break;
                    case 5:
                        mesCadena = "Mayo";
                        break;
                    case 6:
                        mesCadena = "Junio";
                        break;
                    case 7:
                        mesCadena = "Julio";
                        break;
                    case 8:
                        mesCadena = "Agosto";
                        break;
                    case 9:
                        mesCadena = "Septiembre";
                        break;
                    case 10:
                        mesCadena = "Octubre";
                        break;
                    case 11:
                        mesCadena = "Noviembre";
                        break;
                    case 12:
                        mesCadena = "Diciembre";
                        break;
                }

                using (var document = new iTextSharp.text.Document(PageSize.LETTER))
                {
                    // se configura la hoja
                    document.PageSize.Rotate();
                    // se configuran las propiedades
                    document.AddAuthor("Corponet");
                    document.AddTitle("Crear pdf");
                    document.SetMargins(10, 10, 80, 10);
                    document.NewPage();
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(rutaReporte, FileMode.Create));
                    writer.ViewerPreferences = PdfWriter.PageLayoutSinglePage;
                    // se abre el document
                    document.Open();
                    // Imagen
                    Image l = Image.GetInstance(rutaImagenes + "/logo1.jpg");
                    l.SetAbsolutePosition(document.Left + 5, document.Top - 5);
                    l.ScaleToFit(85.0f, 85.0f);
                    document.Add(l);
                    // Titulo del reporte
                    var titulo = new Paragraph("REPORTE DE ARCHIVOS TXT GENERADOS", new Font((Font.FontFamily)Font.BOLD, 14.0f));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);
                    // Fecha del reporte
                    var fecha = new Paragraph("Fecha: " + thisDay.ToString() + " de " + mesCadena + " del " + thisYear.ToString(), new Font((Font.FontFamily)Font.BOLD, 8.0f));
                    fecha.Alignment = Element.ALIGN_RIGHT;
                    document.Add(fecha);
                    // primera tabla
                    var table = new PdfPTable(2);
                    table.TotalWidth = 200.0f;
                    table.LockedWidth = true;
                    // table.DefaultCell.Border = Rectangle.NO_BORDER
                    var widths = new float[] { 1.0f, 2.0f };
                    table.SetWidths(widths);
                    table.HorizontalAlignment = (int)30.0f;
                    table.SpacingBefore = 20.0f;
                    table.SpacingAfter = 30.0f;
                    var cell1 = new PdfPCell(new Phrase("FOLIO: ", new Font((Font.FontFamily)Font.BOLD, 12.0f)));
                    var cell2 = new PdfPCell(new Phrase(folio, new Font((Font.FontFamily)Font.BOLD, 12.0f)));
                    cell1.BorderColor = BaseColor.YELLOW;
                    cell2.BorderColor = BaseColor.YELLOW;
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    document.Add(table);
                    var pagosEfectuados = new Paragraph("Pagos efectuados - Dispersión de pagos", new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    pagosEfectuados.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(pagosEfectuados);
                    // Tipo de operacion
                    var tipoOperacion = new Paragraph("Tipo de Cuenta: " + tipoCta, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    tipoOperacion.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(tipoOperacion);
                    // sociedad
                    var sociedad = new Paragraph("Sociedad: " + nomSociedad, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    sociedad.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(sociedad);
                    // sucursal
                    var sucursal = new Paragraph("Sucursal: " + suc, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    sucursal.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(sucursal);
                    // tabla 2
                    var table2 = new PdfPTable(6);
                    table2.TotalWidth = 580.0f;
                    table2.LockedWidth = true;
                    var Widths2 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table2.SetWidths(Widths2);
                    table2.HorizontalAlignment = (int)30.0f;
                    table2.SpacingBefore = 20.0f;
                    // table2.SpacingAfter = 30.0F
                    var T2_cell1 = new PdfPCell(new Phrase("# Transferencia ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell2 = new PdfPCell(new Phrase("Beneficiario ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell3 = new PdfPCell(new Phrase("Cuenta Origen: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell4 = new PdfPCell(new Phrase("Cuenta Destino: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell5 = new PdfPCell(new Phrase("Importe: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell6 = new PdfPCell(new Phrase("Fecha de Aplicación: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    T2_cell1.BorderColor = BaseColor.BLACK;
                    T2_cell2.BorderColor = BaseColor.BLACK;
                    T2_cell3.BorderColor = BaseColor.BLACK;
                    T2_cell4.BorderColor = BaseColor.BLACK;
                    T2_cell5.BorderColor = BaseColor.BLACK;
                    T2_cell6.BorderColor = BaseColor.BLACK;
                    T2_cell1.BackgroundColor = BaseColor.YELLOW;
                    T2_cell2.BackgroundColor = BaseColor.YELLOW;
                    T2_cell3.BackgroundColor = BaseColor.YELLOW;
                    T2_cell4.BackgroundColor = BaseColor.YELLOW;
                    T2_cell5.BackgroundColor = BaseColor.YELLOW;
                    T2_cell6.BackgroundColor = BaseColor.YELLOW;
                    table2.AddCell(T2_cell1);
                    table2.AddCell(T2_cell2);
                    table2.AddCell(T2_cell3);
                    table2.AddCell(T2_cell4);
                    table2.AddCell(T2_cell5);
                    table2.AddCell(T2_cell6);
                    document.Add(table2);
                    var table3 = new PdfPTable(6);
                    table3.TotalWidth = 580.0f;
                    table3.LockedWidth = true;
                    var Widths3 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table3.SetWidths(Widths3);
                    table3.HorizontalAlignment = (int)30.0f;

                    foreach (Transferencia transferencia in transferencias)
                    {
                        sTransferencia = transferencia.DocNum.ToString();
                        sBeneficiario = transferencia.CardName.ToString();
                        sCuentaOrigen = transferencia.Account.ToString();
                        sCuentaDestino = transferencia.DflAccount.ToString();
                        sImporte = transferencia.DocTotal.ToString("C");
                        sFecha = transferencia.DocDate.ToString("dd/MM/yyyy");
                        //dTotal = Conversions.ToDecimal(Strings.Format(dTotal + eImporte.String));
                        var T3_cell1 = new PdfPCell(new Phrase(sTransferencia, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                        var T3_cell2 = new PdfPCell(new Phrase(sBeneficiario, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                        var T3_cell3 = new PdfPCell(new Phrase(sCuentaOrigen, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                        var T3_cell4 = new PdfPCell(new Phrase(sCuentaDestino, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                        var T3_cell5 = new PdfPCell(new Phrase(sImporte, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                        var T3_cell6 = new PdfPCell(new Phrase(sFecha, new Font((Font.FontFamily)Font.BOLD, 8.0f)));

                        table3.AddCell(T3_cell1);
                        table3.AddCell(T3_cell2);
                        table3.AddCell(T3_cell3);
                        table3.AddCell(T3_cell4);
                        table3.AddCell(T3_cell5);
                        table3.AddCell(T3_cell6);
                    }
                    dTotal = transferencias.Sum(d => d.DocTotal);
                    document.Add(table3);
                    var table4 = new PdfPTable(6);
                    table4.TotalWidth = 580.0f;
                    table4.LockedWidth = true;

                    var widths4 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table4.SetWidths(widths4);
                    table4.HorizontalAlignment = (int)30.0f;
                    table4.SpacingBefore = 20.0f;
                    table4.SpacingAfter = 30.0f;
                    var t4_cell01 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell11 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell21 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell31 = new PdfPCell(new Phrase("# de tranferencias: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell41 = new PdfPCell(new Phrase(transferencias.Count.ToString(), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell51 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell0 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell1 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell2 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell3 = new PdfPCell(new Phrase("Total Proveedor: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell4 = new PdfPCell(new Phrase(dTotal.ToString("C"), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell5 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_0 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_1 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_2 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_3 = new PdfPCell(new Phrase("Total Folio: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_4 = new PdfPCell(new Phrase(dTotal.ToString("c"), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_5 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    t4_cell01.BorderColor = BaseColor.WHITE;
                    t4_cell11.BorderColor = BaseColor.WHITE;
                    t4_cell21.BorderColor = BaseColor.WHITE;
                    t4_cell31.BorderColor = BaseColor.BLACK;
                    t4_cell41.BorderColor = BaseColor.BLACK;
                    t4_cell51.BorderColor = BaseColor.BLACK;
                    t4_cell0.BorderColor = BaseColor.WHITE;
                    t4_cell1.BorderColor = BaseColor.WHITE;
                    t4_cell2.BorderColor = BaseColor.WHITE;
                    t4_cell3.BorderColor = BaseColor.BLACK;
                    t4_cell4.BorderColor = BaseColor.BLACK;
                    t4_cell5.BorderColor = BaseColor.WHITE;
                    t4_cell_0.BorderColor = BaseColor.WHITE;
                    t4_cell_1.BorderColor = BaseColor.WHITE;
                    t4_cell_2.BorderColor = BaseColor.WHITE;
                    t4_cell_3.BorderColor = BaseColor.BLACK;
                    t4_cell_4.BorderColor = BaseColor.BLACK;
                    t4_cell_5.BorderColor = BaseColor.WHITE;
                    table4.AddCell(t4_cell01);
                    table4.AddCell(t4_cell11);
                    table4.AddCell(t4_cell21);
                    table4.AddCell(t4_cell31);
                    table4.AddCell(t4_cell41);
                    table4.AddCell(t4_cell51);
                    table4.AddCell(t4_cell0);
                    table4.AddCell(t4_cell1);
                    table4.AddCell(t4_cell2);
                    table4.AddCell(t4_cell3);
                    table4.AddCell(t4_cell4);
                    table4.AddCell(t4_cell5);
                    table4.AddCell(t4_cell_0);
                    table4.AddCell(t4_cell_1);
                    table4.AddCell(t4_cell_2);
                    table4.AddCell(t4_cell_3);
                    table4.AddCell(t4_cell_4);
                    table4.AddCell(t4_cell_5);
                    document.Add(table4);
                    var table5 = new PdfPTable(3);
                    table5.TotalWidth = 580.0f;
                    table5.LockedWidth = true;

                    var widths5 = new float[] { 1.0f, 1.0f, 1.0f };
                    table5.SetWidths(widths5);
                    table5.HorizontalAlignment = (int)30.0f;
                    table5.SpacingBefore = 20.0f;
                    table5.SpacingAfter = 30.0f;
                    var t5_cell1 = new PdfPCell(new Phrase("__________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell2 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell3 = new PdfPCell(new Phrase("__________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell4 = new PdfPCell(new Phrase("ELABORO", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell5 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell6 = new PdfPCell(new Phrase("PROCESO", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell7 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell8 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell9 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell10 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell11 = new PdfPCell(new Phrase("_________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell12 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell13 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell14 = new PdfPCell(new Phrase("ELIMINO ARCHIVOS TXT", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell15 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    t5_cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell1.BorderColor = BaseColor.WHITE;
                    t5_cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell2.BorderColor = BaseColor.WHITE;
                    t5_cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell3.BorderColor = BaseColor.WHITE;
                    t5_cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell4.BorderColor = BaseColor.WHITE;
                    t5_cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell5.BorderColor = BaseColor.WHITE;
                    t5_cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell6.BorderColor = BaseColor.WHITE;
                    t5_cell7.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell7.BorderColor = BaseColor.WHITE;
                    t5_cell8.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell8.BorderColor = BaseColor.WHITE;
                    t5_cell9.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell9.BorderColor = BaseColor.WHITE;
                    t5_cell10.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell10.BorderColor = BaseColor.WHITE;
                    t5_cell11.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell11.BorderColor = BaseColor.WHITE;
                    t5_cell12.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell12.BorderColor = BaseColor.WHITE;
                    t5_cell13.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell13.BorderColor = BaseColor.WHITE;
                    t5_cell14.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell14.BorderColor = BaseColor.WHITE;
                    t5_cell15.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell15.BorderColor = BaseColor.WHITE;
                    table5.AddCell(t5_cell1);
                    table5.AddCell(t5_cell2);
                    table5.AddCell(t5_cell3);
                    table5.AddCell(t5_cell4);
                    table5.AddCell(t5_cell5);
                    table5.AddCell(t5_cell6);
                    table5.AddCell(t5_cell7);
                    table5.AddCell(t5_cell8);
                    table5.AddCell(t5_cell9);
                    table5.AddCell(t5_cell10);
                    table5.AddCell(t5_cell11);
                    table5.AddCell(t5_cell12);
                    table5.AddCell(t5_cell13);
                    table5.AddCell(t5_cell14);
                    table5.AddCell(t5_cell15);
                    document.Add(table5);
                    document.Close();
                }
            }
            catch (Exception)
            {
                b = false;
            }
            return b;
        }
        public bool GeneraPDF(string rutaReporte, string rutaImagenes, string tipoCta, string folio, string suc, Transferencia transferencia, string nomSociedad)
        {
            bool b = true;
            try
            {
                int thisDay = DateTime.Now.Day;
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                string mesCadena = string.Empty;
                string sTransferencia;
                string sBeneficiario;
                string sCuentaOrigen;
                string sCuentaDestino;
                string sImporte;
                string sFecha;
                decimal dTotal = 0m;
                switch (thisMonth)
                {
                    case 1:
                        mesCadena = "Enero";
                        break;
                    case 2:
                        mesCadena = "Febrero";
                        break;
                    case 3:
                        mesCadena = "Marzo";
                        break;
                    case 4:
                        mesCadena = "Abril";
                        break;
                    case 5:
                        mesCadena = "Mayo";
                        break;
                    case 6:
                        mesCadena = "Junio";
                        break;
                    case 7:
                        mesCadena = "Julio";
                        break;
                    case 8:
                        mesCadena = "Agosto";
                        break;
                    case 9:
                        mesCadena = "Septiembre";
                        break;
                    case 10:
                        mesCadena = "Octubre";
                        break;
                    case 11:
                        mesCadena = "Noviembre";
                        break;
                    case 12:
                        mesCadena = "Diciembre";
                        break;
                }

                using (var document = new iTextSharp.text.Document(PageSize.LETTER))
                {
                    // se configura la hoja
                    document.PageSize.Rotate();
                    // se configuran las propiedades
                    document.AddAuthor("Corponet");
                    document.AddTitle("Crear pdf");
                    document.SetMargins(10, 10, 80, 10);
                    document.NewPage();
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(rutaReporte, FileMode.Create));
                    writer.ViewerPreferences = PdfWriter.PageLayoutSinglePage;
                    // se abre el document
                    document.Open();
                    // Imagen
                    Image l = Image.GetInstance(rutaImagenes + "/logo1.jpg");
                    l.SetAbsolutePosition(document.Left + 5, document.Top - 5);
                    l.ScaleToFit(85.0f, 85.0f);
                    document.Add(l);
                    // Titulo del reporte
                    var titulo = new Paragraph("REPORTE DE ARCHIVOS TXT GENERADOS", new Font((Font.FontFamily)Font.BOLD, 14.0f));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);
                    // Fecha del reporte
                    var fecha = new Paragraph("Fecha: " + thisDay.ToString() + " de " + mesCadena + " del " + thisYear.ToString(), new Font((Font.FontFamily)Font.BOLD, 8.0f));
                    fecha.Alignment = Element.ALIGN_RIGHT;
                    document.Add(fecha);
                    // primera tabla
                    var table = new PdfPTable(2);
                    table.TotalWidth = 200.0f;
                    table.LockedWidth = true;
                    // table.DefaultCell.Border = Rectangle.NO_BORDER
                    var widths = new float[] { 1.0f, 2.0f };
                    table.SetWidths(widths);
                    table.HorizontalAlignment = (int)30.0f;
                    table.SpacingBefore = 20.0f;
                    table.SpacingAfter = 30.0f;
                    var cell1 = new PdfPCell(new Phrase("FOLIO: ", new Font((Font.FontFamily)Font.BOLD, 12.0f)));
                    var cell2 = new PdfPCell(new Phrase(folio, new Font((Font.FontFamily)Font.BOLD, 12.0f)));
                    cell1.BorderColor = BaseColor.YELLOW;
                    cell2.BorderColor = BaseColor.YELLOW;
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    document.Add(table);
                    var pagosEfectuados = new Paragraph("Pagos efectuados - Dispersión de pagos", new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    pagosEfectuados.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(pagosEfectuados);
                    // Tipo de operacion
                    var tipoOperacion = new Paragraph("Tipo de Cuenta: " + tipoCta, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    tipoOperacion.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(tipoOperacion);
                    // sociedad
                    var sociedad = new Paragraph("Sociedad: " + nomSociedad, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    sociedad.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(sociedad);
                    // sucursal
                    var sucursal = new Paragraph("Sucursal: " + suc, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    sucursal.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(sucursal);
                    // tabla 2
                    var table2 = new PdfPTable(6);
                    table2.TotalWidth = 580.0f;
                    table2.LockedWidth = true;
                    var Widths2 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table2.SetWidths(Widths2);
                    table2.HorizontalAlignment = (int)30.0f;
                    table2.SpacingBefore = 20.0f;
                    // table2.SpacingAfter = 30.0F
                    var T2_cell1 = new PdfPCell(new Phrase("# Transferencia ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell2 = new PdfPCell(new Phrase("Beneficiario ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell3 = new PdfPCell(new Phrase("Cuenta Origen: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell4 = new PdfPCell(new Phrase("Cuenta Destino: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell5 = new PdfPCell(new Phrase("Importe: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell6 = new PdfPCell(new Phrase("Fecha de Aplicación: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    T2_cell1.BorderColor = BaseColor.BLACK;
                    T2_cell2.BorderColor = BaseColor.BLACK;
                    T2_cell3.BorderColor = BaseColor.BLACK;
                    T2_cell4.BorderColor = BaseColor.BLACK;
                    T2_cell5.BorderColor = BaseColor.BLACK;
                    T2_cell6.BorderColor = BaseColor.BLACK;
                    T2_cell1.BackgroundColor = BaseColor.YELLOW;
                    T2_cell2.BackgroundColor = BaseColor.YELLOW;
                    T2_cell3.BackgroundColor = BaseColor.YELLOW;
                    T2_cell4.BackgroundColor = BaseColor.YELLOW;
                    T2_cell5.BackgroundColor = BaseColor.YELLOW;
                    T2_cell6.BackgroundColor = BaseColor.YELLOW;
                    table2.AddCell(T2_cell1);
                    table2.AddCell(T2_cell2);
                    table2.AddCell(T2_cell3);
                    table2.AddCell(T2_cell4);
                    table2.AddCell(T2_cell5);
                    table2.AddCell(T2_cell6);
                    document.Add(table2);
                    var table3 = new PdfPTable(6);
                    table3.TotalWidth = 580.0f;
                    table3.LockedWidth = true;
                    var Widths3 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table3.SetWidths(Widths3);
                    table3.HorizontalAlignment = (int)30.0f;


                    sTransferencia = transferencia.DocNum.ToString();
                    sBeneficiario = transferencia.CardName.ToString();
                    sCuentaOrigen = transferencia.Account.ToString();
                    sCuentaDestino = transferencia.DflAccount.ToString();
                    sImporte = transferencia.DocTotal.ToString("C");
                    sFecha = transferencia.DocDate.ToString("dd/MM/yyyy");
                    //dTotal = Conversions.ToDecimal(Strings.Format(dTotal + eImporte.String));
                    var T3_cell1 = new PdfPCell(new Phrase(sTransferencia, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell2 = new PdfPCell(new Phrase(sBeneficiario, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell3 = new PdfPCell(new Phrase(sCuentaOrigen, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell4 = new PdfPCell(new Phrase(sCuentaDestino, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell5 = new PdfPCell(new Phrase(sImporte, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell6 = new PdfPCell(new Phrase(sFecha, new Font((Font.FontFamily)Font.BOLD, 8.0f)));

                    table3.AddCell(T3_cell1);
                    table3.AddCell(T3_cell2);
                    table3.AddCell(T3_cell3);
                    table3.AddCell(T3_cell4);
                    table3.AddCell(T3_cell5);
                    table3.AddCell(T3_cell6);
                    dTotal = transferencia.DocTotal;
                    document.Add(table3);
                    var table4 = new PdfPTable(6);
                    table4.TotalWidth = 580.0f;
                    table4.LockedWidth = true;

                    var widths4 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table4.SetWidths(widths4);
                    table4.HorizontalAlignment = (int)30.0f;
                    table4.SpacingBefore = 20.0f;
                    table4.SpacingAfter = 30.0f;
                    var t4_cell0 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell1 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell2 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell3 = new PdfPCell(new Phrase("Total Proveedor: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell4 = new PdfPCell(new Phrase(dTotal.ToString("C"), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell5 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_0 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_1 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_2 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_3 = new PdfPCell(new Phrase("Total Folio: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_4 = new PdfPCell(new Phrase(dTotal.ToString("c"), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_5 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    t4_cell0.BorderColor = BaseColor.WHITE;
                    t4_cell1.BorderColor = BaseColor.WHITE;
                    t4_cell2.BorderColor = BaseColor.WHITE;
                    t4_cell3.BorderColor = BaseColor.BLACK;
                    t4_cell4.BorderColor = BaseColor.BLACK;
                    t4_cell5.BorderColor = BaseColor.WHITE;
                    t4_cell_0.BorderColor = BaseColor.WHITE;
                    t4_cell_1.BorderColor = BaseColor.WHITE;
                    t4_cell_2.BorderColor = BaseColor.WHITE;
                    t4_cell_3.BorderColor = BaseColor.BLACK;
                    t4_cell_4.BorderColor = BaseColor.BLACK;
                    t4_cell_5.BorderColor = BaseColor.WHITE;
                    table4.AddCell(t4_cell0);
                    table4.AddCell(t4_cell1);
                    table4.AddCell(t4_cell2);
                    table4.AddCell(t4_cell3);
                    table4.AddCell(t4_cell4);
                    table4.AddCell(t4_cell5);
                    table4.AddCell(t4_cell_0);
                    table4.AddCell(t4_cell_1);
                    table4.AddCell(t4_cell_2);
                    table4.AddCell(t4_cell_3);
                    table4.AddCell(t4_cell_4);
                    table4.AddCell(t4_cell_5);
                    document.Add(table4);
                    var table5 = new PdfPTable(3);
                    table5.TotalWidth = 580.0f;
                    table5.LockedWidth = true;

                    var widths5 = new float[] { 1.0f, 1.0f, 1.0f };
                    table5.SetWidths(widths5);
                    table5.HorizontalAlignment = (int)30.0f;
                    table5.SpacingBefore = 20.0f;
                    table5.SpacingAfter = 30.0f;
                    var t5_cell1 = new PdfPCell(new Phrase("__________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell2 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell3 = new PdfPCell(new Phrase("__________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell4 = new PdfPCell(new Phrase("ELABORO", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell5 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell6 = new PdfPCell(new Phrase("PROCESO", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell7 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell8 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell9 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell10 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell11 = new PdfPCell(new Phrase("_________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell12 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell13 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell14 = new PdfPCell(new Phrase("ELIMINO ARCHIVOS TXT", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell15 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    t5_cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell1.BorderColor = BaseColor.WHITE;
                    t5_cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell2.BorderColor = BaseColor.WHITE;
                    t5_cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell3.BorderColor = BaseColor.WHITE;
                    t5_cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell4.BorderColor = BaseColor.WHITE;
                    t5_cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell5.BorderColor = BaseColor.WHITE;
                    t5_cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell6.BorderColor = BaseColor.WHITE;
                    t5_cell7.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell7.BorderColor = BaseColor.WHITE;
                    t5_cell8.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell8.BorderColor = BaseColor.WHITE;
                    t5_cell9.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell9.BorderColor = BaseColor.WHITE;
                    t5_cell10.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell10.BorderColor = BaseColor.WHITE;
                    t5_cell11.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell11.BorderColor = BaseColor.WHITE;
                    t5_cell12.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell12.BorderColor = BaseColor.WHITE;
                    t5_cell13.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell13.BorderColor = BaseColor.WHITE;
                    t5_cell14.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell14.BorderColor = BaseColor.WHITE;
                    t5_cell15.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell15.BorderColor = BaseColor.WHITE;
                    table5.AddCell(t5_cell1);
                    table5.AddCell(t5_cell2);
                    table5.AddCell(t5_cell3);
                    table5.AddCell(t5_cell4);
                    table5.AddCell(t5_cell5);
                    table5.AddCell(t5_cell6);
                    table5.AddCell(t5_cell7);
                    table5.AddCell(t5_cell8);
                    table5.AddCell(t5_cell9);
                    table5.AddCell(t5_cell10);
                    table5.AddCell(t5_cell11);
                    table5.AddCell(t5_cell12);
                    table5.AddCell(t5_cell13);
                    table5.AddCell(t5_cell14);
                    table5.AddCell(t5_cell15);
                    document.Add(table5);
                    document.Close();
                }
            }
            catch (Exception)
            {
                b = false;
            }
            return b;
        }
        /*Dispersion servicios*/
        public List<Servicio> ObtenerTransferenciasServicios()
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = Properties.Resources.SQL_TRANSFERENCIAS_SERVICIOS;

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
                                        new Servicio
                                        {
                                            Sociedad = (string)row["Sociedad"],
                                            VATRegNum = (string)row["VATRegNum"],
                                            BankCtlKey = (string)row["BankCtlKey"],
                                            MandateID = (string)row["MandateID"],
                                            Account = (string)row["Account"],
                                            DflAccount = (string)row["DflAccount"],
                                            DocTotal = decimal.Parse(row["DocTotal"].ToString()),
                                            DocNum = int.Parse(row["DocNum"].ToString()),
                                            JrnlMemo = (string)row["JrnlMemo"],
                                            County = (string)row["County"],
                                            LicTradNum = (string)row["LicTradNum"],
                                            IVA = (string)row["IVA"],
                                            E_Mail = (string)row["E_Mail"],
                                            DocDate = (DateTime)row["DocDate"],
                                            CardName = (string)row["CardName"],
                                            DocNum2 = int.Parse(row["DocNum2"].ToString()),
                                            DocEntry = int.Parse(row["DocEntry"].ToString()),
                                            BPLName = (string)row["BPLName"],
                                            GLAccount = (string)row["GLAccount"],
                                            REF01 = (string)row["REF01"],
                                            REF02 = (string)row["REF02"],
                                            REF03 = (string)row["REF03"],
                                            REF04 = (string)row["REF04"],
                                            REF05 = (string)row["REF05"],
                                            REF06 = (string)row["REF06"],
                                            FILLER = (string)row["FILLER"],
                                            FECVEN = (string)row["FECVEN"]
                                        }).ToList();

                                    return list;
                                }
                                catch (Exception)
                                {
                                    throw;
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
        public List<string> GenerarArchivoServicio(List<Servicio> transferencias, string u, string p)
        {
            List<string> sResult = GenerarArchivoServicios(transferencias, u, p).Result;

            return sResult;
        }
        private async Task<List<string>> GenerarArchivoServicios(List<Servicio> servicios, string u, string p)
        {
            List<string> pdf = new List<string>();
            try
            {
                string sociedad, sucursal;
                foreach (Servicio servicio in servicios)
                {
                    sociedad = servicio.Sociedad;
                    sucursal = servicio.VATRegNum;
                    IEnumerable<string> cookies = null;
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                    HttpClient httpClient = new HttpClient(handler);
                    var d = new LoginApiData
                    {
                        CompanyDB = sociedad,
                        UserName = u,
                        Password = p
                    };
                    var datalogin = new StringContent(
                        JsonConvert.SerializeObject(d),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                    if (response.IsSuccessStatusCode)
                    {
                        cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                    }

                    Stream stream = null;
                    StreamWriter streamWriter = null;
                    DateTime date = DateTime.Now;
                    string dia = date.Day.ToString().PadLeft(2, '0');
                    string mes = date.Month.ToString().PadLeft(2, '0');
                    string anio = date.Year.ToString().Substring(2);
                    string tipoCta = "SERV"; // (transferencia.BankCtlKey == "04" || transferencia.BankCtlKey == "4") ? "SPEI" : "PAGTER";
                    string nombreArchivo = "";
                    string U_RutaArchivo;
                    string U_RutaReporte;
                    string U_RutaImagenes;
                    string U_CompnyName;
                    using (HanaConnection connH = new HanaConnection(ConnectionString))
                    {
                        connH.Open();
                        string sConsulta = " SELECT \"U_CompnyName\", \"U_RutaArchivo\", \"U_RutaReporte\", \"U_RutaImagenes\" FROM \"SBOGOVI\".\"@SOCIEDADES\" WHERE  \"U_DB\" = '" + sociedad + "' ";
                        using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                        {
                            using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                            {
                                using (DataTable data = new DataTable())
                                {
                                    data.Load(dataReader);
                                    U_RutaArchivo = (string)data.Rows[0]["U_RutaArchivo"];
                                    U_RutaReporte = (string)data.Rows[0]["U_RutaReporte"];
                                    U_RutaImagenes = (string)data.Rows[0]["U_RutaImagenes"];
                                    U_CompnyName = (string)data.Rows[0]["U_CompnyName"];
                                }
                            }
                        }
                    }
                    string nombreSucursal = sucursal;

                    if (!Directory.Exists(U_RutaArchivo))
                    {
                        Directory.CreateDirectory(U_RutaArchivo);
                    }
                    //pdf = pathReporte;
                    if (!Directory.Exists(U_RutaReporte))
                    {
                        Directory.CreateDirectory(U_RutaReporte);
                    }

                    /**/
                    int folioArchivo = 0;
                    StreamReader reader = new StreamReader("folio.txt");
                    folioArchivo = int.Parse(reader.ReadLine()) + 1;
                    reader.Close();
                    StreamWriter writer = new StreamWriter("folio.txt");
                    writer.Write(folioArchivo);
                    writer.Close();
                    /**/
                    nombreArchivo = tipoCta + "-" + dia + mes + anio + "-" + nombreSucursal + "-FOLIO" + folioArchivo;
                    string pathArchivo = U_RutaArchivo + "\\" + nombreArchivo + ".TXT";
                    //string pathArchivo = Path.Combine("Files", nombreArchivo + ".TXT");
                    if (File.Exists(pathArchivo))
                    {
                        File.Delete(pathArchivo);
                        stream = File.Create(pathArchivo);
                    }
                    else
                    {
                        stream = File.Create(pathArchivo);
                    }
                    streamWriter = new StreamWriter(stream, Encoding.Default);
                    StringBuilder renglon = new StringBuilder();
                    renglon.Append(servicio.MandateID.ToString().PadRight(6));
                    renglon.Append(servicio.BankCtlKey.ToString().PadLeft(2, '0'));
                    string importe = String.Format("{0:00.00}", servicio.DocTotal).Replace(".", "").PadLeft(15, '0');
                    renglon.Append(importe);
                    renglon.Append(servicio.Account.ToString().PadLeft(20, '0'));
                    renglon.Append(servicio.DflAccount.ToString().PadLeft(20, '0'));
                    renglon.Append(servicio.REF01.ToString().PadRight(40, ' '));
                    renglon.Append(servicio.FECVEN.ToString().PadLeft(8, '0'));
                    renglon.Append(servicio.E_Mail);
                    //renglon.Append(transferencia.E_Mail.Length > 39 ? transferencia.E_Mail.Substring(0, 39) : transferencia.E_Mail.ToString().PadRight(40));
                    renglon.Append(servicio.REF02.ToString().PadRight(40, ' '));
                    renglon.Append(servicio.REF03.ToString().PadRight(40, ' '));
                    renglon.Append(servicio.REF04.ToString().PadRight(40, ' '));
                    renglon.Append(servicio.REF05.ToString().PadRight(40, ' '));
                    renglon.Append(servicio.REF06.ToString().PadRight(40, ' '));
                    renglon.Append(servicio.FILLER.ToString().PadLeft(207, ' '));
                    renglon.Append(".");
                    renglon.AppendLine((string)null);
                    //Actualizar
                    var jsonobj = new { U_dispersion = "si" };
                    var udispersion = new StringContent(
                        JsonConvert.SerializeObject(jsonobj),
                        Encoding.UTF8,
                        "application/json");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments({servicio.DocEntry})", udispersion);

                    if (!(response2.StatusCode == HttpStatusCode.NoContent))
                    {
                        streamWriter.Close();
                        throw new Exception();
                    }
                    //Escribir
                    streamWriter.Write(renglon);
                    streamWriter.Close();
                    //Reporte
                    //----string pathReporte = Path.Combine("Files", nombreArchivo + ".pdf");
                    string pathReporte = U_RutaReporte + "\\" + nombreArchivo + ".pdf";
                    //Crear pdf
                    if (GeneraPDFServicio(pathReporte, U_RutaImagenes, tipoCta, folioArchivo.ToString(), nombreSucursal, servicio, U_CompnyName))
                    {

                        if (File.Exists(pathReporte))
                        {
                            //File.Copy(pathReporte, Path.Combine(dest, nombreArchivo + ".pdf"));
                        }
                    }
                    pdf.Add(nombreArchivo + ".pdf");
                    GuardarDispersionServicio(servicio, 1, u, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return pdf;
        }
        public string GenerarServicio(List<Servicio> transferencias, string u, string p)
        {
            string sResult = GenerarServicios(transferencias, u, p).Result;

            return sResult;
        }
        private async Task<string> GenerarServicios(List<Servicio> servicios, string u, string p)
        {
            string pdf = "Dispersion realizada correctamente";
            try
            {
                string sociedad, sucursal;
                foreach (Servicio transferencia in servicios)
                {
                    sociedad = transferencia.Sociedad;
                    sucursal = transferencia.VATRegNum;
                    IEnumerable<string> cookies = null;
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                    HttpClient httpClient = new HttpClient(handler);
                    var d = new LoginApiData
                    {
                        CompanyDB = sociedad,
                        UserName = u,
                        Password = p
                    };
                    var datalogin = new StringContent(
                        JsonConvert.SerializeObject(d),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                    if (response.IsSuccessStatusCode)
                    {
                        cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                    }
                    //Actualizar
                    var jsonobj = new { U_dispersion = "si" };
                    var udispersion = new StringContent(
                        JsonConvert.SerializeObject(jsonobj),
                        Encoding.UTF8,
                        "application/json");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments({transferencia.DocEntry})", udispersion);

                    if (!(response2.StatusCode == HttpStatusCode.NoContent))
                    {
                        return response2.Content.ReadAsStringAsync().Result;//
                    }

                    GuardarDispersionServicio(transferencia, 1, u, "generado sin archivo");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return pdf;
        }
        private void GuardarDispersionServicio(Servicio transferencia, int tipo, string u, string nArchivo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlConectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(@"INSERT INTO dbo.FoliosDispersion (Sociedad, VATRegNum, BankCtlKey, MandateID, Account, DflAccount, DocTotal, DocNum, JrnlMemo, County, LicTradNum, IVA, E_Mail, DocDate, CardName, DocNum2, DocEntry, nArchivo, Usuario, FechaGen, Tipo, REF01, REF02, REF03, REF04, REF05, REF06, FILLER, FECVEN)
                                                                                           VALUES (@sociedad, @vatregnum, @bankctlkey, @mandateid, @account, @dflaccount, @doctotal, @docnum, @jrnlmemo, @county, @lictradnum, @iva, @e_mail, @docdate, @cardname, @docnum2, @docentry, @narchivo, @usuario, @fechagen, @tipogen, @REF01, @REF02, @REF03, @REF04, @REF05, @REF06, @FILLER, @FECVEN)", connection))
                    {
                        command.Parameters.AddWithValue("@sociedad", transferencia.Sociedad);
                        command.Parameters.AddWithValue("@vatregnum", transferencia.VATRegNum);
                        command.Parameters.AddWithValue("@bankctlkey", transferencia.BankCtlKey);
                        command.Parameters.AddWithValue("@mandateid", transferencia.MandateID);
                        command.Parameters.AddWithValue("@account", transferencia.Account);
                        command.Parameters.AddWithValue("@dflaccount", transferencia.DflAccount);
                        command.Parameters.AddWithValue("@doctotal", transferencia.DocTotal);
                        command.Parameters.AddWithValue("@docnum", transferencia.DocNum);
                        command.Parameters.AddWithValue("@jrnlmemo", transferencia.JrnlMemo);
                        command.Parameters.AddWithValue("@county", transferencia.County);
                        command.Parameters.AddWithValue("@lictradnum", transferencia.LicTradNum);
                        command.Parameters.AddWithValue("@iva", transferencia.IVA);
                        command.Parameters.AddWithValue("@e_mail", transferencia.E_Mail);
                        command.Parameters.AddWithValue("@docdate", transferencia.DocDate);
                        command.Parameters.AddWithValue("@cardname", transferencia.CardName);
                        command.Parameters.AddWithValue("@docnum2", transferencia.DocNum);
                        command.Parameters.AddWithValue("@docentry", transferencia.DocEntry);
                        command.Parameters.AddWithValue("@narchivo", nArchivo);
                        command.Parameters.AddWithValue("@usuario", u);
                        command.Parameters.AddWithValue("@fechagen", DateTime.Now);
                        command.Parameters.AddWithValue("@tipogen", 3);
                        command.Parameters.AddWithValue("@REF01", transferencia.REF01);
                        command.Parameters.AddWithValue("@REF02", transferencia.REF02);
                        command.Parameters.AddWithValue("@REF03", transferencia.REF03);
                        command.Parameters.AddWithValue("@REF04", transferencia.REF04);
                        command.Parameters.AddWithValue("@REF05", transferencia.REF05);
                        command.Parameters.AddWithValue("@REF06", transferencia.REF06);
                        command.Parameters.AddWithValue("@FILLER", transferencia.FILLER);
                        command.Parameters.AddWithValue("@FECVEN", transferencia.FECVEN);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool GeneraPDFServicio(string rutaReporte, string rutaImagenes, string tipoCta, string folio, string suc, Servicio transferencia, string nomSociedad)
        {
            bool b = true;
            try
            {
                int thisDay = DateTime.Now.Day;
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                string mesCadena = string.Empty;
                string sTransferencia;
                string sBeneficiario;
                string sCuentaOrigen;
                string sCuentaDestino;
                string sImporte;
                string sFecha;
                decimal dTotal = 0m;
                switch (thisMonth)
                {
                    case 1:
                        mesCadena = "Enero";
                        break;
                    case 2:
                        mesCadena = "Febrero";
                        break;
                    case 3:
                        mesCadena = "Marzo";
                        break;
                    case 4:
                        mesCadena = "Abril";
                        break;
                    case 5:
                        mesCadena = "Mayo";
                        break;
                    case 6:
                        mesCadena = "Junio";
                        break;
                    case 7:
                        mesCadena = "Julio";
                        break;
                    case 8:
                        mesCadena = "Agosto";
                        break;
                    case 9:
                        mesCadena = "Septiembre";
                        break;
                    case 10:
                        mesCadena = "Octubre";
                        break;
                    case 11:
                        mesCadena = "Noviembre";
                        break;
                    case 12:
                        mesCadena = "Diciembre";
                        break;
                }

                using (var document = new iTextSharp.text.Document(PageSize.LETTER))
                {
                    // se configura la hoja
                    document.PageSize.Rotate();
                    // se configuran las propiedades
                    document.AddAuthor("Corponet");
                    document.AddTitle("Crear pdf");
                    document.SetMargins(10, 10, 80, 10);
                    document.NewPage();
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(rutaReporte, FileMode.Create));
                    writer.ViewerPreferences = PdfWriter.PageLayoutSinglePage;
                    // se abre el document
                    document.Open();
                    // Imagen
                    Image l = Image.GetInstance(rutaImagenes + "/logo1.jpg");
                    l.SetAbsolutePosition(document.Left + 5, document.Top - 5);
                    l.ScaleToFit(85.0f, 85.0f);
                    document.Add(l);
                    // Titulo del reporte
                    var titulo = new Paragraph("REPORTE DE ARCHIVOS TXT GENERADOS", new Font((Font.FontFamily)Font.BOLD, 14.0f));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);
                    // Fecha del reporte
                    var fecha = new Paragraph("Fecha: " + thisDay.ToString() + " de " + mesCadena + " del " + thisYear.ToString(), new Font((Font.FontFamily)Font.BOLD, 8.0f));
                    fecha.Alignment = Element.ALIGN_RIGHT;
                    document.Add(fecha);
                    // primera tabla
                    var table = new PdfPTable(2);
                    table.TotalWidth = 200.0f;
                    table.LockedWidth = true;
                    // table.DefaultCell.Border = Rectangle.NO_BORDER
                    var widths = new float[] { 1.0f, 2.0f };
                    table.SetWidths(widths);
                    table.HorizontalAlignment = (int)30.0f;
                    table.SpacingBefore = 20.0f;
                    table.SpacingAfter = 30.0f;
                    var cell1 = new PdfPCell(new Phrase("FOLIO: ", new Font((Font.FontFamily)Font.BOLD, 12.0f)));
                    var cell2 = new PdfPCell(new Phrase(folio, new Font((Font.FontFamily)Font.BOLD, 12.0f)));
                    cell1.BorderColor = BaseColor.YELLOW;
                    cell2.BorderColor = BaseColor.YELLOW;
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    document.Add(table);
                    var pagosEfectuados = new Paragraph("Pagos efectuados - Dispersión de pagos", new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    pagosEfectuados.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(pagosEfectuados);
                    // Tipo de operacion
                    var tipoOperacion = new Paragraph("Tipo de Cuenta: " + tipoCta, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    tipoOperacion.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(tipoOperacion);
                    // sociedad
                    var sociedad = new Paragraph("Sociedad: " + nomSociedad, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    sociedad.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(sociedad);
                    // sucursal
                    var sucursal = new Paragraph("Sucursal: " + suc, new Font((Font.FontFamily)Font.BOLD, 12.0f));
                    sucursal.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(sucursal);
                    // tabla 2
                    var table2 = new PdfPTable(6);
                    table2.TotalWidth = 580.0f;
                    table2.LockedWidth = true;
                    var Widths2 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table2.SetWidths(Widths2);
                    table2.HorizontalAlignment = (int)30.0f;
                    table2.SpacingBefore = 20.0f;
                    // table2.SpacingAfter = 30.0F
                    var T2_cell1 = new PdfPCell(new Phrase("# Transferencia ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell2 = new PdfPCell(new Phrase("Beneficiario ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell3 = new PdfPCell(new Phrase("Cuenta Origen: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell4 = new PdfPCell(new Phrase("Referencia 1: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell5 = new PdfPCell(new Phrase("Importe: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var T2_cell6 = new PdfPCell(new Phrase("Fecha de Aplicación: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    T2_cell1.BorderColor = BaseColor.BLACK;
                    T2_cell2.BorderColor = BaseColor.BLACK;
                    T2_cell3.BorderColor = BaseColor.BLACK;
                    T2_cell4.BorderColor = BaseColor.BLACK;
                    T2_cell5.BorderColor = BaseColor.BLACK;
                    T2_cell6.BorderColor = BaseColor.BLACK;
                    T2_cell1.BackgroundColor = BaseColor.YELLOW;
                    T2_cell2.BackgroundColor = BaseColor.YELLOW;
                    T2_cell3.BackgroundColor = BaseColor.YELLOW;
                    T2_cell4.BackgroundColor = BaseColor.YELLOW;
                    T2_cell5.BackgroundColor = BaseColor.YELLOW;
                    T2_cell6.BackgroundColor = BaseColor.YELLOW;
                    table2.AddCell(T2_cell1);
                    table2.AddCell(T2_cell2);
                    table2.AddCell(T2_cell3);
                    table2.AddCell(T2_cell4);
                    table2.AddCell(T2_cell5);
                    table2.AddCell(T2_cell6);
                    document.Add(table2);
                    var table3 = new PdfPTable(6);
                    table3.TotalWidth = 580.0f;
                    table3.LockedWidth = true;
                    var Widths3 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table3.SetWidths(Widths3);
                    table3.HorizontalAlignment = (int)30.0f;


                    sTransferencia = transferencia.DocNum.ToString();
                    sBeneficiario = transferencia.CardName.ToString();
                    sCuentaOrigen = transferencia.Account.ToString();
                    sCuentaDestino = transferencia.REF01.ToString();
                    sImporte = transferencia.DocTotal.ToString("C");
                    sFecha = transferencia.DocDate.ToString("dd/MM/yyyy");
                    //dTotal = Conversions.ToDecimal(Strings.Format(dTotal + eImporte.String));
                    var T3_cell1 = new PdfPCell(new Phrase(sTransferencia, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell2 = new PdfPCell(new Phrase(sBeneficiario, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell3 = new PdfPCell(new Phrase(sCuentaOrigen, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell4 = new PdfPCell(new Phrase(sCuentaDestino, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell5 = new PdfPCell(new Phrase(sImporte, new Font((Font.FontFamily)Font.BOLD, 8.0f)));
                    var T3_cell6 = new PdfPCell(new Phrase(sFecha, new Font((Font.FontFamily)Font.BOLD, 8.0f)));

                    table3.AddCell(T3_cell1);
                    table3.AddCell(T3_cell2);
                    table3.AddCell(T3_cell3);
                    table3.AddCell(T3_cell4);
                    table3.AddCell(T3_cell5);
                    table3.AddCell(T3_cell6);
                    dTotal = transferencia.DocTotal;
                    document.Add(table3);
                    var table4 = new PdfPTable(6);
                    table4.TotalWidth = 580.0f;
                    table4.LockedWidth = true;

                    var widths4 = new float[] { 2.0f, 4.0f, 2.0f, 2.0f, 2.0f, 2.0f };
                    table4.SetWidths(widths4);
                    table4.HorizontalAlignment = (int)30.0f;
                    table4.SpacingBefore = 20.0f;
                    table4.SpacingAfter = 30.0f;
                    var t4_cell0 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell1 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell2 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell3 = new PdfPCell(new Phrase("Total Proveedor: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell4 = new PdfPCell(new Phrase(dTotal.ToString("C"), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell5 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_0 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_1 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_2 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_3 = new PdfPCell(new Phrase("Total Folio: ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_4 = new PdfPCell(new Phrase(dTotal.ToString("c"), new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t4_cell_5 = new PdfPCell(new Phrase(" ", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    t4_cell0.BorderColor = BaseColor.WHITE;
                    t4_cell1.BorderColor = BaseColor.WHITE;
                    t4_cell2.BorderColor = BaseColor.WHITE;
                    t4_cell3.BorderColor = BaseColor.BLACK;
                    t4_cell4.BorderColor = BaseColor.BLACK;
                    t4_cell5.BorderColor = BaseColor.WHITE;
                    t4_cell_0.BorderColor = BaseColor.WHITE;
                    t4_cell_1.BorderColor = BaseColor.WHITE;
                    t4_cell_2.BorderColor = BaseColor.WHITE;
                    t4_cell_3.BorderColor = BaseColor.BLACK;
                    t4_cell_4.BorderColor = BaseColor.BLACK;
                    t4_cell_5.BorderColor = BaseColor.WHITE;
                    table4.AddCell(t4_cell0);
                    table4.AddCell(t4_cell1);
                    table4.AddCell(t4_cell2);
                    table4.AddCell(t4_cell3);
                    table4.AddCell(t4_cell4);
                    table4.AddCell(t4_cell5);
                    table4.AddCell(t4_cell_0);
                    table4.AddCell(t4_cell_1);
                    table4.AddCell(t4_cell_2);
                    table4.AddCell(t4_cell_3);
                    table4.AddCell(t4_cell_4);
                    table4.AddCell(t4_cell_5);
                    document.Add(table4);
                    var table5 = new PdfPTable(3);
                    table5.TotalWidth = 580.0f;
                    table5.LockedWidth = true;

                    var widths5 = new float[] { 1.0f, 1.0f, 1.0f };
                    table5.SetWidths(widths5);
                    table5.HorizontalAlignment = (int)30.0f;
                    table5.SpacingBefore = 20.0f;
                    table5.SpacingAfter = 30.0f;
                    var t5_cell1 = new PdfPCell(new Phrase("__________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell2 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell3 = new PdfPCell(new Phrase("__________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell4 = new PdfPCell(new Phrase("ELABORO", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell5 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell6 = new PdfPCell(new Phrase("PROCESO", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell7 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell8 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell9 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell10 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell11 = new PdfPCell(new Phrase("_________________________________", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell12 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell13 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell14 = new PdfPCell(new Phrase("ELIMINO ARCHIVOS TXT", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    var t5_cell15 = new PdfPCell(new Phrase("", new Font((Font.FontFamily)Font.BOLD, 9.0f)));
                    t5_cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell1.BorderColor = BaseColor.WHITE;
                    t5_cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell2.BorderColor = BaseColor.WHITE;
                    t5_cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell3.BorderColor = BaseColor.WHITE;
                    t5_cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell4.BorderColor = BaseColor.WHITE;
                    t5_cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell5.BorderColor = BaseColor.WHITE;
                    t5_cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell6.BorderColor = BaseColor.WHITE;
                    t5_cell7.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell7.BorderColor = BaseColor.WHITE;
                    t5_cell8.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell8.BorderColor = BaseColor.WHITE;
                    t5_cell9.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell9.BorderColor = BaseColor.WHITE;
                    t5_cell10.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell10.BorderColor = BaseColor.WHITE;
                    t5_cell11.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell11.BorderColor = BaseColor.WHITE;
                    t5_cell12.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell12.BorderColor = BaseColor.WHITE;
                    t5_cell13.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell13.BorderColor = BaseColor.WHITE;
                    t5_cell14.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell14.BorderColor = BaseColor.WHITE;
                    t5_cell15.HorizontalAlignment = Element.ALIGN_CENTER;
                    t5_cell15.BorderColor = BaseColor.WHITE;
                    table5.AddCell(t5_cell1);
                    table5.AddCell(t5_cell2);
                    table5.AddCell(t5_cell3);
                    table5.AddCell(t5_cell4);
                    table5.AddCell(t5_cell5);
                    table5.AddCell(t5_cell6);
                    table5.AddCell(t5_cell7);
                    table5.AddCell(t5_cell8);
                    table5.AddCell(t5_cell9);
                    table5.AddCell(t5_cell10);
                    table5.AddCell(t5_cell11);
                    table5.AddCell(t5_cell12);
                    table5.AddCell(t5_cell13);
                    table5.AddCell(t5_cell14);
                    table5.AddCell(t5_cell15);
                    document.Add(table5);
                    document.Close();
                }
            }
            catch (Exception)
            {
                b = false;
            }
            return b;
        }
        /*Informe*/
        public List<SdAldia> ObtenerSDAldia(string fecha)
        {
            try
            {
                string sHanaConsulta = "";
                using (SqlConnection connection = new SqlConnection(SqlConectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter command = new SqlDataAdapter("Consultas", connection))
                    {
                        command.SelectCommand.CommandType = CommandType.StoredProcedure;
                        command.SelectCommand.Parameters.AddWithValue("id", "informebancos");
                        DataTable result = new DataTable();
                        if (command.Fill(result) > 0)
                        {
                            sHanaConsulta = result.Rows[0][0].ToString();
                        }
                    }
                }
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();

                    string sConsulta = string.Format(sHanaConsulta, fecha); // " CALL \"STS_GROB\".\"SD_ALDIA_DN\"('" + fecha + "') ";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                var list = data.AsEnumerable().Select(row =>
                                new SdAldia
                                {
                                    Empresa = (string)row["Empresa"],
                                    Cuenta = (string)row["Cuenta"],
                                    NombreCuenta = (string)row["NombreCuenta"],
                                    SaldoDiario = (decimal)row["SaldoDiario"],
                                    Fecha = (string)row["Fecha"],
                                }).ToList();

                                return list;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new List<SdAldia> { };
                throw;
            }
        }

        public dynamic GetInforme(string id, Fechas fechas)
        {
            try
            {
                string sHanaConsulta = "";
                using (SqlConnection connection = new SqlConnection(SqlConectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter command = new SqlDataAdapter("Consultas", connection))
                    {
                        command.SelectCommand.CommandType = CommandType.StoredProcedure;
                        command.SelectCommand.Parameters.AddWithValue("id", id);
                        DataTable result = new DataTable();
                        if (command.Fill(result) > 0)
                        {
                            sHanaConsulta = result.Rows[0][0].ToString();
                        }
                    }
                }
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format(sHanaConsulta, fechas.FechaIni, fechas.FechaFin);
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                using (DataSet ds = new DataSet() { EnforceConstraints = false })
                                {
                                    ds.Tables.Add(data);
                                    data.Load(dataReader, LoadOption.OverwriteChanges);
                                    ds.Tables.Remove(data);
                                }

                                var dns = new List<dynamic>();

                                foreach (var item in data.AsEnumerable())
                                {
                                    // Expando objects are IDictionary<string, object>
                                    IDictionary<string, object> dn = new ExpandoObject();

                                    foreach (var column in data.Columns.Cast<DataColumn>())
                                    {
                                        dn[column.ColumnName] = item[column];
                                    }

                                    dns.Add(dn);
                                }

                                return new { rows = dns, };
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public dynamic GetParametros(string id)
        {
            try
            {
                DataTable result = new DataTable();
                using (SqlConnection connection = new SqlConnection(SqlConectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter command = new SqlDataAdapter("Parametros", connection))
                    {
                        command.SelectCommand.CommandType = CommandType.StoredProcedure;
                        command.SelectCommand.Parameters.AddWithValue("id", id);
                        command.Fill(result);
                    }
                }

                if (result.Rows.Count > 0)
                {
                    return new
                    {
                        cols = JsonConvert.DeserializeObject<List<ColModel>>(result.Rows[0][0].ToString()),
                        props = JsonConvert.DeserializeObject<List<PropModel>>(result.Rows[0][1].ToString())
                    };
                }
                else
                {
                    // Return empty but valid structure to prevent frontend crash, or throw clear error
                    return new { cols = new List<ColModel>(), props = new List<PropModel>(), error = $"No se encontraron parámetros para: {id}" };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en GetParametros('{id}'): {ex.Message}", ex);
            }
        }
        public dynamic ObtenerTransfersHeader(Fechas fechas)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("SELECT \"Empresa\", \"Sucursal\", \"NombreCuenta\", COUNT(*) AS \"Transferencias\", SUM(cast(\"Importe\" AS float)) \"ImporteTotal\" FROM \"_SYS_BIC\".\"ProductivaGOVI.ReporteTransferencias/TransferenciasSBOGOVI\" WHERE \"FEmision\" BETWEEN '{0}' AND '{1}' GROUP BY \"Empresa\", \"Sucursal\", \"NombreCuenta\" ORDER BY \"Empresa\", \"Sucursal\" ", fechas.FechaIni, fechas.FechaFin);
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                using (DataSet ds = new DataSet() { EnforceConstraints = false })
                                {
                                    ds.Tables.Add(data);
                                    data.Load(dataReader, LoadOption.OverwriteChanges);
                                    ds.Tables.Remove(data);
                                }
                                var list = data.AsEnumerable().Select(row =>
                                new
                                {
                                    Empresa = row["Empresa"],
                                    Sucursal = row["Sucursal"],
                                    NombreCuenta = row["NombreCuenta"],
                                    ImporteTotal = row["ImporteTotal"],
                                    Transferencias = row["Transferencias"]
                                }).ToList();

                                return list;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new { };
            }
        }
        public dynamic ObtenerTransfersDetails(string empresa, Fechas fechas)
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = string.Format("SELECT \"NombreCuenta\", \"Sucursal\", \"NumProv\", \"Beneficiario\", \"Transfer\", \"CHeque\", \"FEmision\", \"DescripciondeDispersion\", \"Importe\", \"U_Domiciliado\", \"FechaAct\" FROM \"_SYS_BIC\".\"ProductivaGOVI.ReporteTransferencias/TransferenciasSBOGOVI\" WHERE \"FEmision\" BETWEEN '{0}' AND '{1}' AND \"Empresa\" = '{2}' ORDER BY \"Sucursal\", \"NombreCuenta\" ", fechas.FechaIni, fechas.FechaFin, empresa);
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                using (DataSet ds = new DataSet() { EnforceConstraints = false })
                                {
                                    ds.Tables.Add(data);
                                    data.Load(dataReader, LoadOption.OverwriteChanges);
                                    ds.Tables.Remove(data);
                                }
                                var list = data.AsEnumerable().Select(row =>
                                new
                                {
                                    NombreCuenta = row["NombreCuenta"],
                                    NumProv = row["NumProv"],
                                    Beneficiario = row["Beneficiario"],
                                    Transfer = row["Transfer"],
                                    CHeque = row["CHeque"],
                                    FEmision = row["FEmision"].ToString().Substring(0, 10),
                                    DescripciondeDispersion = row.IsNull("DescripciondeDispersion") ? "" : row["DescripciondeDispersion"],
                                    Importe = row["Importe"],
                                    U_Domiciliado = row["U_Domiciliado"],
                                    FechaAct = row.IsNull("FechaAct") ? "" : row["FechaAct"].ToString().Substring(0, 10)
                                }).ToList();

                                return list;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return new { };
            }
        }
        /*Ajustes*/
        public List<Sucursal> ObtenerCedis()
        {
            List<Sucursal> list = null;
            try
            {
                using (var con = new SqlConnection(SqlConectionStringAjustes))
                {
                    con.Open();
                    SqlDataAdapter sqlCommand = new SqlDataAdapter(@"SELECT BPLId, BPLName, BPLFrName FROM CEDIS", con);
                    DataTable result = new DataTable();
                    if (sqlCommand.Fill(result) > 0)
                    {
                        list = result.AsEnumerable().Select(row =>
                        new Sucursal
                        {
                            BPLId = (int)row["BPLId"],
                            BPLName = (string)row["BPLName"],
                            BPLFrName = (string)row["BPLFrName"]
                        }).ToList();

                    }
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            return list;
        }
        public async Task<ActionsResponse> AjusteSalida(List<Ajuste> ajustes, string motivo, string usuario, Sucursal sucursal, HttpRequest requestMessage)
        {
            int docEntry = 0;
            ActionsResponse actionsResponse = null;
            try
            {
                List<Ajuste> salidas = ajustes.FindAll(x => x.SALIDA > 0);
                using (var con = new SqlConnection(SqlConectionStringAjustes))
                {
                    con.Open();
                    SqlDataAdapter sqlCommand = new SqlDataAdapter(@"INSERT INTO OIGE (BPLId, BPLName,TipoOperacion,MotivoAjuste, DocDate, Solicitante) OUTPUT INSERTED.DocEntry VALUES (@BPLId, @BPLName, 'Entrada', @Motivo, getdate(), @Solicitante)", con);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@Motivo", motivo);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@BPLId", sucursal.BPLId);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@BPLName", sucursal.BPLFrName);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@Solicitante", usuario);
                    DataTable result = new DataTable();
                    if (sqlCommand.Fill(result) > 0)
                    {
                        docEntry = (int)result.Rows[0][0];
                        foreach (Ajuste item in salidas)
                        {
                            SqlCommand sqlCommand2 = new SqlCommand(@"INSERT INTO [dbo].[IGE1]([DocEntry],[Localizacion],[Producto],[Marca],[Entrada],[Salida],[Cuenta],[Descripcion], [Existencia])
                                                                                        VALUES (@DocEntry, @Localizacion, @Producto, @Marca, @Entrada, @Salida, @Cuenta, @Descripcion, @Existencia)", con);
                            sqlCommand2.Parameters.AddWithValue("@DocEntry", docEntry);
                            sqlCommand2.Parameters.AddWithValue("@Localizacion", item.LOCALIZACION);
                            sqlCommand2.Parameters.AddWithValue("@Producto", item.PRODUCTO);
                            sqlCommand2.Parameters.AddWithValue("@Marca", item.MARCA);
                            sqlCommand2.Parameters.AddWithValue("@Entrada", item.ENTRADA);
                            sqlCommand2.Parameters.AddWithValue("@Salida", item.SALIDA);
                            sqlCommand2.Parameters.AddWithValue("@Cuenta", item.CUENTA);
                            sqlCommand2.Parameters.AddWithValue("@Descripcion", item.DESCRIPCION);
                            sqlCommand2.Parameters.AddWithValue("@Existencia", item.EXISTENCIA);
                            sqlCommand2.ExecuteNonQuery();
                        }
                    }
                }
                //Reemplazar con services layer
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

                    IEnumerable<AjusteModel> salidasModel = salidas.Select(row =>
                    new AjusteModel
                    {
                        itemcode = row.PRODUCTO,
                        location = row.LOCALIZACION,
                        quantity = row.SALIDA
                    });
                    HttpClient httpClient = new HttpClient(handler);
                    var itemsSalida = new StringContent(
                        JsonConvert.SerializeObject(new { items = salidasModel, bplid = sucursal.BPLId }),
                        Encoding.UTF8,
                        "application/json");
                    var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiOpenUI5") + $"salida?comments={motivo}", itemsSalida);

                    if (response.IsSuccessStatusCode)
                    {
                        var document = response.Content.ReadAsStringAsync().Result;

                        //enviar a SAP
                        HttpClient httpClient2 = new HttpClient(handler);
                        var body = new StringContent(
                            document,
                            Encoding.UTF8,
                            "application/json");
                        IEnumerable<string> cookie1 = new string[] { "B1SESSION=" + requestMessage.Headers["b1session"], "ROUTEID=" + requestMessage.Headers["routeid"] };
                        httpClient2.DefaultRequestHeaders.Add("Cookie", cookie1);

                        var response2 = await httpClient2.PostAsync(_configuration.GetConnectionString("ApiSAP") + "InventoryGenExits", body);

                        if (response2.IsSuccessStatusCode)
                        {
                            actionsResponse = new ActionsResponse(true, response2.Content.ReadAsStringAsync().Result);
                        }
                        else actionsResponse = new ActionsResponse(false, response2.Content.ReadAsStringAsync().Result);
                    }
                    else actionsResponse = new ActionsResponse(false, response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                actionsResponse = new ActionsResponse(false, ex.Message);
                throw;
            }

            return actionsResponse;
        }
        public async Task<ActionsResponse> AjusteEntrada(List<Ajuste> ajustes, string motivo, string usuario, Sucursal sucursal, HttpRequest requestMessage)
        {
            int docEntry = 0;
            ActionsResponse actionsResponse = null;
            try
            {
                List<Ajuste> entradas = ajustes.FindAll(x => x.ENTRADA > 0);
                using (var con = new SqlConnection(SqlConectionStringAjustes))
                {
                    con.Open();
                    SqlDataAdapter sqlCommand = new SqlDataAdapter(@"INSERT INTO OIGN (BPLId, BPLName,TipoOperacion,MotivoAjuste, DocDate, Solicitante) OUTPUT INSERTED.DocEntry VALUES (@BPLId, @BPLName, 'Entrada', @Motivo, getdate(), @Solicitante)", con);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@Motivo", motivo);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@BPLId", sucursal.BPLId);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@BPLName", sucursal.BPLFrName);
                    sqlCommand.SelectCommand.Parameters.AddWithValue("@Solicitante", usuario);
                    DataTable result = new DataTable();
                    if (sqlCommand.Fill(result) > 0)
                    {
                        docEntry = (int)result.Rows[0][0];
                        foreach (Ajuste item in entradas)
                        {
                            SqlCommand sqlCommand2 = new SqlCommand(@"INSERT INTO [dbo].[IGN1]([DocEntry],[Localizacion],[Producto],[Marca],[Entrada],[Salida],[Cuenta],[Descripcion])
                                                                                            VALUES (@DocEntry, @Localizacion, @Producto, @Marca, @Entrada, @Salida, @Cuenta, @Descripcion)", con);
                            sqlCommand2.Parameters.AddWithValue("@DocEntry", docEntry);
                            sqlCommand2.Parameters.AddWithValue("@Localizacion", item.LOCALIZACION);
                            sqlCommand2.Parameters.AddWithValue("@Producto", item.PRODUCTO);
                            sqlCommand2.Parameters.AddWithValue("@Marca", item.MARCA);
                            sqlCommand2.Parameters.AddWithValue("@Entrada", item.ENTRADA);
                            sqlCommand2.Parameters.AddWithValue("@Salida", item.SALIDA);
                            sqlCommand2.Parameters.AddWithValue("@Cuenta", item.CUENTA);
                            sqlCommand2.Parameters.AddWithValue("@Descripcion", item.DESCRIPCION);
                            sqlCommand2.ExecuteNonQuery();
                        }
                    }

                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

                        IEnumerable<AjusteModel> entradasModel = entradas.Select(row =>
                        new AjusteModel
                        {
                            itemcode = row.PRODUCTO,
                            location = row.LOCALIZACION,
                            quantity = row.ENTRADA
                        });
                        HttpClient httpClient = new HttpClient(handler);
                        var itemsEntradas = new StringContent(
                            JsonConvert.SerializeObject(new { items = entradasModel, bplid = sucursal.BPLId }),
                            Encoding.UTF8,
                            "application/json");
                        var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiOpenUI5") + $"entrada?comments={motivo}", itemsEntradas);

                        if (response.IsSuccessStatusCode)
                        {
                            var document = response.Content.ReadAsStringAsync().Result;

                            //enviar a SAP
                            HttpClient httpClient2 = new HttpClient(handler);
                            var body = new StringContent(
                                document,
                                Encoding.UTF8,
                                "application/json");
                            IEnumerable<string> cookie1 = new string[] { "B1SESSION=" + requestMessage.Headers["b1session"], "ROUTEID=" + requestMessage.Headers["routeid"] };
                            httpClient2.DefaultRequestHeaders.Add("Cookie", cookie1);

                            var response2 = await httpClient2.PostAsync(_configuration.GetConnectionString("ApiSAP") + "InventoryGenEntries", body);

                            if (response2.IsSuccessStatusCode)
                            {
                                actionsResponse = new ActionsResponse(true, response2.Content.ReadAsStringAsync().Result);
                            }
                            else actionsResponse = new ActionsResponse(false, response2.Content.ReadAsStringAsync().Result);
                        }
                        else actionsResponse = new ActionsResponse(false, response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception ex)
            {

                actionsResponse = new ActionsResponse(false, ex.Message);
                throw;
            }

            return actionsResponse;
        }
        /**/
        public async Task<IEnumerable<string>> Login(LoginApiData loginApiData)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlConectionString))
                {
                    await connection.OpenAsync();
                    string sConsulta = " SELECT TOP 1 [UserName], [Password] FROM [DISPERSION].[dbo].[usuarios] WHERE [UserName] = @UserName AND [Password] = @Password ";
                    using (SqlCommand command = new SqlCommand(sConsulta, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", loginApiData.UserName);
                        command.Parameters.AddWithValue("@Password", loginApiData.Password);
                        using (var dataReader = await command.ExecuteReaderAsync())
                        {
                            if (dataReader.HasRows)
                            {
                                IEnumerable<string> cookies = null;
                                HttpClientHandler handler = new HttpClientHandler();
                                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                                HttpClient httpClient = new HttpClient(handler);
                                var d = new LoginApiData
                                {
                                    CompanyDB = _configuration.GetValue<string>("UserData:CompanyDB"),
                                    UserName = loginApiData.UserName,
                                    Password = loginApiData.Password
                                };
                                var datalogin = new StringContent(
                                    JsonConvert.SerializeObject(d),
                                    Encoding.UTF8,
                                    "application/json");

                                var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                                if (response.IsSuccessStatusCode)
                                {
                                    cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                                }

                                return cookies;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        /*Tunel Pagos*/
        public async Task<List<dynamic>> UploadTxt(List<IFormFile> files)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\");
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\", Path.GetFileName(formFile.FileName));
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                return LoadFiles();
            }
            catch (System.Exception)
            {
                return null;
            }

        }
        private List<dynamic> LoadFiles()
        {
            List<dynamic> files = new List<dynamic>();
            try
            {
                string[] filesPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\");
                foreach (string filep in filesPath)
                {

                    var datosLeidos = LoadData(filep);
                    var list = datosLeidos.Item2.AsEnumerable().Select(row => new
                    {
                        Referencia = row["Referencia"],
                        InstruccionPago = row["InstruccionPago"],
                        CuentaOrigen = row["CuentaOrigen"],
                        CuentaDestino = row["CuentaDestino"],
                        Importe = row["Importe"],
                        FechaAplicacion = row["FechaAplicacion"].ToString().Substring(0, 10)
                    }).ToList();
                    files.Add(new { name = Path.GetFileName(filep), totalImporte = datosLeidos.Item1, filas = list });
                }
            }
            catch (Exception)
            {

            }
            return files;
        }
        private Tuple<double, DataTable> LoadData(string file)
        {

            string OpenPath;
            string line;
            double dValorComputado = 0.0;
            // Create new DataTable.

            DataTable table = CreateTable();
            DataRow row;
            try
            {
                OpenPath = file;//Server.MapPath("/ArchivosPlanos") + '/' + file;
                string FILENAME = OpenPath;
                //Get a StreamReader class that can be used to read the file
                StreamReader objStreamReader;
                objStreamReader = File.OpenText(FILENAME);

                dValorComputado = 0.0;
                while ((line = objStreamReader.ReadLine()) != null)
                {
                    row = table.NewRow();
                    row[0] = line.Substring(0, 2).ToString().Trim();
                    row[1] = line.Substring(2, 13).ToString().Trim();
                    //ro12"] = line.Substring(15, 63).ToString().Trim();
                    row[2] = line.Substring(15, 20).ToString().Trim();
                    row[3] = line.Substring(35, 20).ToString().Trim();
                    string importe = line.Substring(55, 14).TrimStart('0');
                    row[4] = importe.Substring(0, importe.Length - 2) + "." + importe.Substring(importe.Length - 2);
                    row[5] = line.Substring(69, 10).ToString().Trim();
                    row[6] = line.Substring(79, 30).ToString().Trim();
                    row[7] = line.Substring(109, 1).ToString().Trim();
                    row[8] = line.Substring(110, 1).ToString().Trim();
                    row[9] = line.Substring(111, 13).ToString().Trim();
                    row[10] = line.Substring(124, 14).ToString().Trim();
                    row[11] = line.Substring(138, 39).ToString().Trim();
                    row[12] = DateTime.ParseExact(line.Substring(177, 8).ToString().Trim(), "ddMMyyyy", CultureInfo.InvariantCulture);
                    row[13] = line.Substring(185).ToString().Trim();
                    table.Rows.Add(row);
                }
                objStreamReader.Close();
                dValorComputado = (double)table.Compute("SUM(Importe)", "");

                // Set to DataGrid.DataSource property to the table.

            }
            catch (Exception)
            {

            }

            return Tuple.Create(dValorComputado, table);
        }
        public ActionsResponse EnviarTunel(string[] archivosSeleccionado)
        {
            ActionsResponse actionsResponse = null;

            string localPath = AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\"; //De donde se escogen
            string Respaldo = @"C:\BANCOS\DISPERSION\RESPALDO DE TXT";
            string Origen = @"C:\BANCOS\DISPERSION\BANCOS";
            string targetBan = AppDomain.CurrentDomain.BaseDirectory + "\\PuenteBancario\\";
            //string targetBan = @"\\172.16.2.121\\a_enviar";
            string newFile = "";
            string flag = "1";
            try
            {
                DateTime date = DateTime.Now;
                string dia = date.Day.ToString().PadLeft(2, '0');
                string mes = date.Month.ToString().PadLeft(2, '0');
                string anio = date.Year.ToString().Substring(2);
                foreach (string sArchivoTxt in archivosSeleccionado)
                {
                    flag = "2";
                    newFile = targetBan + "\\PP139448" + anio + mes + dia + sArchivoTxt.Substring(sArchivoTxt.Length - 7).ToUpper();
                    flag = "4";
                    File.Copy(Path.Combine(@localPath, sArchivoTxt), Path.Combine(@Respaldo, sArchivoTxt));
                    File.Delete(Path.Combine(@Origen, sArchivoTxt));
                    flag = "5";
                    //NetworkShare.ConnectToShare(@"\\172.16.2.121\a_enviar", "Banorte", "Ban0rt3!"); //Connect with the new credentials
                    File.Move(Path.Combine(@localPath, sArchivoTxt), @newFile); //Aqui fue que no funciono
                    //NetworkShare.DisconnectFromShare(@"\\172.16.2.121\a_enviar", true); //Disconnect in case we are currently connected with our credentials;
                }
                actionsResponse = new ActionsResponse(true, "Archivos procesados correctamente");
                //Que borra 
                try
                {
                    DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\");
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        file.Delete();
                    }
                }
                catch (Exception ex)
                {
                    actionsResponse = new ActionsResponse(false, ex.Message);

                }
            }
            catch (Exception ex)
            {
                actionsResponse = new ActionsResponse(false, flag + " | " + ex.Message + " | " + localPath + " | " + newFile + "");
            }
            return actionsResponse;
        }
        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            try
            {
                DataColumn column;

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Operación";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "ClaveID";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "CuentaOrigen";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "CuentaDestino";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Double");
                column.ColumnName = "Importe";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Referencia";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Descripción";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "MonedaOrigen";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "MonedaDestino";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "RFCOrdenante";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "IVA";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "emailbeneficiario";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.DateTime");
                column.ColumnName = "FechaAplicacion";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "InstruccionPago";
                table.Columns.Add(column);
            }
            catch (Exception)
            {

            }
            return table;

        }

        /*Tunel Servicio*/
        public async Task<List<dynamic>> UploadTxtServicio(List<IFormFile> files)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\");
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\", Path.GetFileName(formFile.FileName));
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                return LoadFilesServicio();
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        private List<dynamic> LoadFilesServicio()
        {
            List<dynamic> files = new List<dynamic>();
            try
            {
                string[] filesPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\");
                foreach (string filep in filesPath)
                {
                    var fileName = Path.GetFileName(filep);
                    var fecha = fileName.Split('-')[1];
                    var datosLeidos = LoadDataServicio(filep);
                    var list = datosLeidos.Item2.AsEnumerable().Select(row => new
                    {
                        Referencia = row["CVESER"],
                        InstruccionPago = row["FORPAG"],
                        CuentaOrigen = row["CTACAR"],
                        Referencia1 = row["REF01"],
                        Importe = row["Importe"],
                        FechaAplicacion = DateTime.ParseExact(fecha, "ddMMyy", CultureInfo.InvariantCulture).ToString().Substring(0, 10) //row["FECVEN"]
                    }).ToList();
                    DataTable table = new DataTable();
                    try
                    {
                        using (var con = new SqlConnection(SqlConectionStringMigrarTxt))
                        {
                            con.Open();
                            SqlCommand sqlCommand = new SqlCommand(@"ApiQuery", con);
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.AddWithValue("@sXml", string.Format("<i><d tipo='GetNombreFacturador' NumFacturador='{0}' /></i>", datosLeidos.Item2.Rows[0][0]));
                            new SqlDataAdapter(sqlCommand).Fill(table);
                            con.Close();
                        }
                    }
                    catch (Exception) { }
                    files.Add(new { name = Path.GetFileName(filep), nombre = table.Rows[0][0], totalImporte = datosLeidos.Item1, filas = list });
                }
            }
            catch (Exception)
            {

            }
            return files;
        }
        private Tuple<double, DataTable> LoadDataServicio(string file)
        {

            string OpenPath;
            string line;
            double dValorComputado = 0.0;
            // Create new DataTable.

            DataTable table = CreateTableServicio();
            DataRow row;
            try
            {
                OpenPath = file;//Server.MapPath("/ArchivosPlanos") + '/' + file;
                string FILENAME = OpenPath;
                //Get a StreamReader class that can be used to read the file
                StreamReader objStreamReader;
                objStreamReader = File.OpenText(FILENAME);

                dValorComputado = 0.0;
                while ((line = objStreamReader.ReadLine()) != null)
                {
                    row = table.NewRow();
                    row[0] = line.Substring(0, 6).ToString().Trim();
                    row[1] = line.Substring(6, 2).ToString().Trim();
                    string importe = line.Substring(8, 15).TrimStart('0');
                    row[2] = importe.Substring(0, importe.Length - 2) + "." + importe.Substring(importe.Length - 2);
                    row[3] = line.Substring(23, 20).ToString().Trim();
                    row[4] = line.Substring(43, 20).ToString().Trim();
                    row[5] = line.Substring(63, 40).ToString().Trim();
                    row[6] = line.Substring(102, 9).ToString().Trim();
                    row[7] = line.Substring(111, 40).ToString().Trim();
                    table.Rows.Add(row);
                }
                objStreamReader.Close();
                dValorComputado = (double)table.Compute("SUM(Importe)", "");

                // Set to DataGrid.DataSource property to the table.

            }
            catch (Exception)
            {

            }

            return Tuple.Create(dValorComputado, table);
        }
        public ActionsResponse EnviarTunelServicio(string[] archivosSeleccionado)
        {
            ActionsResponse actionsResponse = null;

            string localPath = AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\"; //De donde se escogen
            string OrigenPath = @"C:\BANCOS\DISPERSION\BANCOS"; //De donde se escogen
            string Respaldo = @"C:\BANCOS\DISPERSION\RESPALDO DE TXT";
            string targetBan = AppDomain.CurrentDomain.BaseDirectory + "\\PuenteBancario\\";
            //string targetBan = @"\\172.16.2.121\\a_enviar";
            string newFile = "";
            string flag = "1";
            try
            {
                DateTime date = DateTime.Now;
                string dia = date.Day.ToString().PadLeft(2, '0');
                string mes = date.Month.ToString().PadLeft(2, '0');
                string anio = date.Year.ToString().Substring(2);
                foreach (string p in archivosSeleccionado)
                {
                    flag = "2";
                    newFile = targetBan + "\\PC139448" + anio + mes + dia + p.Substring(p.Length - 7).ToUpper();
                    flag = "4";
                    File.Copy(Path.Combine(@OrigenPath, p), Path.Combine(@Respaldo, p));
                    flag = "4";
                    //NetworkShare.ConnectToShare(@"\\172.16.2.121\a_enviar", "Banorte", "Ban0rt3!"); //Connect with the new credentials
                    File.Move(Path.Combine(@OrigenPath, p), @newFile);
                    //NetworkShare.DisconnectFromShare(@"\\172.16.2.121\a_enviar", true); //Disconnect in case we are currently connected with our credentials;
                }
                actionsResponse = new ActionsResponse(true, "Archivos procesados correctamente");
                //Que borra 
                try
                {
                    DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\ArchivosPlanos\\");
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        file.Delete();
                    }
                }
                catch (Exception ex)
                {
                    actionsResponse = new ActionsResponse(false, ex.Message);

                }
            }
            catch (Exception ex)
            {
                actionsResponse = new ActionsResponse(false, flag + " | " + ex.Message + " | " + localPath + " | " + newFile + "");
            }
            return actionsResponse;
        }
        private DataTable CreateTableServicio()
        {
            DataTable table = new DataTable();
            try
            {
                DataColumn column;

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "CVESER";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = "FORPAG";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Double");
                column.ColumnName = "Importe";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "CTACAR";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "CTAABO";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "REF01";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "FECVEN";
                table.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "EMAIL";
                table.Columns.Add(column);

            }
            catch (Exception)
            {

            }
            return table;

        }

        /*Informe tunel pagos*/
        public dynamic GenerarInformeTunel(Fechas fechas)
        {
            return LoadEncabezado(fechas);
        }
        private dynamic LoadEncabezado(Fechas fechas)
        {
            List<dynamic> result = new List<dynamic>();
            dynamic statistics;
            dynamic diferencias;
            try
            {

                DataTable dataTable = new DataTable("Encabezados");
                using (var con = new SqlConnection(SqlConectionStringMigrarTxt))
                {
                    con.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"ApiQuery", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@sXml", string.Format("<i><d tipo='Encabezado' FechaFin = '{0}' /></i>", fechas.FechaFin));
                    new SqlDataAdapter(sqlCommand).Fill(dataTable);

                }
                foreach (DataRow dato in dataTable.Rows)
                {
                    DataTable d = LoadDetalle((int)dato["DocEntry"]);
                    result.Add(new
                    {
                        Folio = dato["Folio"],
                        Fecha = dato["Fecha"],
                        Confirmacion = dato["Confirmacion"].ToString().Trim(),
                        DocEntry = dato["DocEntry"],
                        Sucursal = dato["Sucursal"],
                        detalle = d.AsEnumerable().Select(row => new
                        {
                            Id = row["id"],
                            Operacion = row["Operacion"],
                            CuentaOrigen = row["CuentaOrigen"],
                            CuentaDestino = row["CuentaDestino"],
                            Importe = row["Importe"],
                            Referencia = row["Referencia"],
                            Descripcion = row["Descripcion"].ToString().Trim(),
                            FechaEjecucion = row["FechaEjecucion"],
                            TitulardelaCuenta = row["TitulardelaCuenta"].ToString().Trim(),
                            Confirmacion = row["Confirmacion"].ToString().Trim()
                        }).ToList()
                    });
                }
                DataTable count = LoadCountData(fechas.FechaFin, fechas.FechaIni);
                statistics = count.AsEnumerable().Select(row => new
                {
                    title = row["title"],
                    total = row["total"]
                }).ToList();
                DataTable listDeferencias = LoadDiferencias(fechas.FechaFin, fechas.FechaIni);
                diferencias = listDeferencias.AsEnumerable().Select(row => new
                {
                    Id = row["id"],
                    Folio = row["Folio"],
                    CuentaOrigen = row["CuentaOrigen"],
                    CuentaDestino = row["CuentaDestino"],
                    Importe = row["Importe"],
                    Referencia = row["Referencia"],
                    Descripcion = row["Descripcion"].ToString().Trim(),
                    FechaEjecucion = row["FechaEjecucion"],
                    TitulardelaCuenta = row["TitulardelaCuenta"].ToString().Trim(),
                    Sucursal = row["Sucursal"].ToString().Trim()
                }).ToList();
                return new
                {
                    rows = result,
                    statistics,
                    diferencias
                };

            }
            catch (Exception)
            {
                return new
                {
                    rows = result,
                    statistics = string.Empty
                };
            }
        }
        private DataTable LoadDetalle(int docEntry)
        {
            DataTable table = new DataTable();
            try
            {
                using (var con = new SqlConnection(SqlConectionStringMigrarTxt))
                {
                    con.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"ApiQuery", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@sXml", string.Format("<i><d tipo='Detalle' DocEntry = '{0}' /></i>", docEntry));
                    new SqlDataAdapter(sqlCommand).Fill(table);
                }

            }
            catch (Exception)
            {

            }
            return table;

        }
        private DataTable LoadCountData(string fechaUno, string fechaDos)
        {
            DataTable table = new DataTable();
            try
            {
                using (var con = new SqlConnection(SqlConectionStringMigrarTxt))
                {
                    con.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"ApiQuery", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@sXml", string.Format("<i><d tipo='Staticts' FechaIni = '{0}' FechaFin = '{1}' /></i>", fechaDos, fechaUno));
                    new SqlDataAdapter(sqlCommand).Fill(table);
                }
            }
            catch (Exception)
            {

            }
            return table;
        }
        private DataTable LoadDiferencias(string fechaUno, string fechaDos)
        {
            DataTable table = new DataTable();
            try
            {
                using (var con = new SqlConnection(SqlConectionStringMigrarTxt))
                {
                    con.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"ApiQuery", con);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@sXml", string.Format("<i><d tipo='Diferencias' FechaIni = '{0}' FechaFin = '{1}' /></i>", fechaDos, fechaUno));
                    new SqlDataAdapter(sqlCommand).Fill(table);
                }
            }
            catch (Exception)
            {

            }
            return table;
        }

        public async void UpdateClase()
        {
            try
            {
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    try
                    {
                        connH.Open();
                        string sConsulta = "select T1.\"ItemCode\", T1.\"Clase\", T2.\"U_CLASIF\" from \"STS_GROB\".\"PortalGROB.oTables::TEMP_CLASIFDSUC\" T1 inner join \"SBOGOVI\".\"OITM\" T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" AND T2.\"U_CLASIF\" <> T1.\"Clase\" where \"Cedis\"='01'";
                        using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                        {
                            using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                            {
                                using (DataTable data = new DataTable())
                                {
                                    data.Load(dataReader);
                                    IEnumerable<string> cookies = null;
                                    HttpClientHandler handler = new HttpClientHandler();
                                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                                    HttpClient httpClient = new HttpClient(handler);
                                    var d = new LoginApiData
                                    {
                                        CompanyDB = "SBOGOVI",
                                        UserName = "manager",
                                        Password = "corponet"
                                    };
                                    var datalogin = new StringContent(
                                        JsonConvert.SerializeObject(d),
                                        Encoding.UTF8,
                                        "application/json");

                                    var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                                        httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                                        foreach (DataRow item in data.Rows)
                                        {
                                            string ItemCode = item["ItemCode"].ToString();
                                            string Clase = item["Clase"].ToString();
                                            var jsonobj = new { U_CLASIF = Clase };
                                            var U_CLASIF = new StringContent(
                                                JsonConvert.SerializeObject(jsonobj),
                                                Encoding.UTF8,
                                                "application/json");
                                            var response2 = await httpClient.PatchAsync(_configuration.GetConnectionString("ApiSAP") + $"Items('{ItemCode}')", U_CLASIF);

                                            if (!(response2.StatusCode == HttpStatusCode.NoContent))
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(List<dynamic> Data, bool ok, string message)> CrearNotaDebito(List<NotaDebitoModel> notaDebitoModels, LoginApiData loginApiData)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                HttpClient httpClient = new HttpClient(handler);
                var d = new LoginApiData
                {
                    CompanyDB = "SBOGOVI",
                    UserName = loginApiData.UserName,
                    Password = loginApiData.Password
                };
                var datalogin = new StringContent(
                    JsonConvert.SerializeObject(d),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);
                IEnumerable<string> cookies = null;
                if (response.IsSuccessStatusCode)
                {
                    cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                }
                //////////

                var result = new List<dynamic>();
                var errors = new List<Tuple<NotaDebitoModel, string>>();
                foreach (var item in notaDebitoModels)
                {
                    var lines = new List<dynamic>();
                    lines.Add(new
                    {
                        ItemCode = item.Concepto,
                        Quantity = item.Cantidad, // $10.00
                        UnitPrice = item.Precio,
                        TaxCode = "A3"
                    });
                    DateTime pDueDate = DateTime.Now;
                    var dataInput = new
                    {
                        CardCode = item.Cliente,
                        DocDate = Convert.ToDateTime(pDueDate),
                        DocDueDate = Convert.ToDateTime(pDueDate),
                        BPL_IDAssignedToInvoice = 1,
                        Comments = item.Comentarios,
                        PaymentMethod = "99",
                        U_LugarExpedicion = "66023",
                        U_B1SYS_MainUsage = "G03",
#if DEBUG
                        Series = 177,
#else
                        Series = 689,
#endif
                        DocCurrency = "MXP",
                        DocumentSubType = "bod_DebitMemo",
                        DocType = "dDocument_Items",
                        ContactPersonCode = 0,
                        DocumentLines = lines
                    };
                    var InvoicesInput = new StringContent(
                        JsonConvert.SerializeObject(dataInput),
                        Encoding.UTF8,
                        "application/json");

                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + $"Invoices", InvoicesInput);

                    if (response2.StatusCode == HttpStatusCode.Created)
                    {
                        var resultRequest = JsonConvert.DeserializeObject<Models.Invoices.Root>(response2.Content.ReadAsStringAsync().Result);
                        result.Add(new
                        {
                            item.Documento,
                            item.Cliente,
                            item.Concepto,
                            item.Cantidad,
                            item.Precio,
                            item.Comentarios,
                            DocEntry = resultRequest.DocNum
                        });
                    }
                    else
                    {
                        errors.Add(Tuple.Create(item, response2.Content.ReadAsStringAsync().Result));
                    }
                }

                if (errors.Count > 0)
                {
                    string messageBody = "<font>Buenas Tardes, resultado de la operacion de nota debito con errores: </font><br><br>";
                    string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                    string htmlTableEnd = "</table>";
                    string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                    string htmlHeaderRowEnd = "</tr>";
                    string htmlTrStart = "<tr style=\"color:#555555;\">";
                    string htmlTrEnd = "</tr>";
                    string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                    string htmlTdEnd = "</td>";
                    messageBody += htmlTableStart;
                    messageBody += htmlHeaderRowStart;
                    messageBody += htmlTdStart + "Documento" + htmlTdEnd;
                    messageBody += htmlTdStart + "Cliente" + htmlTdEnd;
                    messageBody += htmlTdStart + "Concepto" + htmlTdEnd;
                    messageBody += htmlTdStart + "Cantidad" + htmlTdEnd;
                    messageBody += htmlTdStart + "Precio" + htmlTdEnd;
                    messageBody += htmlTdStart + "Comentario" + htmlTdEnd;
                    messageBody += htmlTdStart + "Error" + htmlTdEnd;
                    messageBody += htmlHeaderRowEnd;
                    foreach (var item in errors)
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + item.Item1.Documento + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.Item1.Cliente + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.Item1.Concepto + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.Item1.Cantidad.ToString() + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.Item1.Precio + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.Item1.Comentarios + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.Item2 + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
                    messageBody = messageBody + htmlTableEnd;
                    SendNotification(messageBody);
                }
                return (result, true, "");

            }
            catch (Exception ex)
            {
                return (new List<dynamic>(), false, ex.Message);
            }
        }

        public async Task<(string Data, bool ok, string message)> CrearPasivos(List<PasivoModel> pasivoModels, string sociedad, string u, string p)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                HttpClient httpClient = new HttpClient(handler);
                var d = new LoginApiData
                {
                    CompanyDB = sociedad,
                    UserName = u,
                    Password = p
                };
                var datalogin = new StringContent(
                    JsonConvert.SerializeObject(d),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);
                IEnumerable<string> cookies = null;
                if (response.IsSuccessStatusCode)
                {
                    cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                }
                var result = new List<string>();
                foreach (var item in pasivoModels)
                {
                    var data = JsonConvert.SerializeObject(item);
                    var vendorPaymentInput = new StringContent(
                            data,
                            Encoding.UTF8,
                            "application/json");

                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                    var response2 = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments", vendorPaymentInput);
                    if (response2.StatusCode != HttpStatusCode.Created)
                    {
                        result.Add($"Error cardCode: {item.CardCode} - {response2.Content.ReadAsStringAsync().Result}");
                    }
                    var resultRequest = JsonConvert.DeserializeObject<Models.VendorPayments.Root>(response2.Content.ReadAsStringAsync().Result);

                    result.Add(resultRequest.DocNum.ToString());
                }
                return ("Documentos procesados: " + string.Join(",", result.ToArray()), true, "");

            }
            catch (Exception ex)
            {
                return ("", false, ex.Message);
            }
        }
        public async Task<(string Data, bool ok, string message)> CrearPagoFilial(List<FiliarModel> filiarModels, string sociedad, string sucursal, string proveedor, string u, string p, HttpRequest requestMessage)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                HttpClient httpClient = new HttpClient(handler);
                var d = new LoginApiData
                {
                    CompanyDB = sociedad,
                    UserName = u,
                    Password = p
                };
                var datalogin = new StringContent(
                    JsonConvert.SerializeObject(d),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + "Login", datalogin);
                IEnumerable<string> cookies = null;
                if (response.IsSuccessStatusCode)
                {
                    cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                }
                int idSucursal;
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " Select L0.\"BPLId\" From \"" + sociedad + "\".\"OBPL\" L0 LEFT JOIN \"" + sociedad + "\".\"OCST\" L1 ON L1.\"Code\" = L0.\"State\" LEFT JOIN \"" + sociedad + "\".\"OCRY\" L2 ON L2.\"Code\" = L0.\"Country\" WHERE L0.\"BPLFrName\" IS NOT NULL AND LENGTH(L0.\"BPLName\") <= 3 AND L0.\"BPLName\" = '" + sucursal + "'";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                idSucursal = (int)data.Rows[0]["BPLId"];
                            }
                        }
                    }
                }
                int Series;
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    string sConsulta = " Select \"Series\" From \"" + sociedad + "\".\"NNM1\" WHERE \"ObjectCode\" = '46' AND \"Remark\" = '" + sucursal + "' AND \"Locked\" = 'N'";
                    using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                    {
                        using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                        {
                            using (DataTable data = new DataTable())
                            {
                                data.Load(dataReader);
                                Series = (int)data.Rows[0]["Series"];
                            }
                        }
                    }
                }
                var dataInput = new
                {
                    CardCode = proveedor,
                    BPLID = idSucursal,
                    BPLName = sucursal,
                    Series,
                    Remarks = "PAGO DE FILIALES",
                    JournalRemarks = "PAGO DE FILIALES",
                    PaymentInvoices = GetPaymentInvoices(filiarModels, sociedad),
                    TransferAccount = filiarModels[0].Prefectuado,
                    TransferDate = DateTime.Now,
                    TransferSum = filiarModels.Sum(e => e.Total)
                };
                var vendorPaymentInput = new StringContent(
                        JsonConvert.SerializeObject(dataInput),
                        Encoding.UTF8,
                        "application/json");

                httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
                var response2 = await httpClient.PostAsync(_configuration.GetConnectionString("ApiSAP") + $"VendorPayments", vendorPaymentInput);

                if (response2.StatusCode == HttpStatusCode.Created)
                {
                    return (response2.Content.ReadAsStringAsync().Result, true, "");//
                }
                return ("", false, response2.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return ("", false, ex.Message);
            }
        }
        /// <summary>
        /// Notificacion via correo electronico de el cambio de tasa diario
        /// </summary>
        /// <param name="htmlString"></param>
        static void SendNotification(string htmlString)
        {
            try
            {

                MailMessage message = new MailMessage("notpagos@govi.mx", "sap.govi@suspension-grob.com")
                {
                    Priority = MailPriority.High,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    Subject = "Notificacion de errores",
                    IsBodyHtml = true, //to make message body as html  
                    Body = htmlString
                };
                SmtpClient smtp = new SmtpClient();
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<dynamic> GetPaymentInvoices(List<FiliarModel> filiarModels, string sociedad)
        {
            try
            {
                List<dynamic> result = new List<dynamic>();
                using (HanaConnection connH = new HanaConnection(ConnectionString))
                {
                    connH.Open();
                    foreach (FiliarModel item in filiarModels)
                    {
                        string sConsulta = string.Format("SELECT \"DocEntry\" FROM \"{1}\".\"{2}\" WHERE \"DocNum\" = {0}", item.Documento, sociedad, item.Tipo == 18 ? "OPCH" : "ORPC");
                        using (HanaCommand cmdHSAP = new HanaCommand(sConsulta, connH))
                        {
                            using (HanaDataReader dataReader = cmdHSAP.ExecuteReader())
                            {
                                using (DataTable data = new DataTable())
                                {
                                    data.Load(dataReader);
                                    var tipoInvoice = "";
                                    double MontoPaid = 0.0;
                                    if (item.Tipo == 19)
                                    {
                                        tipoInvoice = "it_PurchaseCreditNote";
                                        MontoPaid = item.Total * -1;
                                    }
                                    else
                                    {
                                        tipoInvoice = "it_PurchaseInvoice";
                                        MontoPaid = item.Total;

                                    }
                                    var row = data.AsEnumerable().Select(rowItem =>
                                    new
                                    {
                                        DocEntry = rowItem["DocEntry"],
                                        SumApplied = item.Total,
                                        InvoiceType = tipoInvoice
                                    }).FirstOrDefault();
                                    result.Add(row);
                                }
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalGovi.DataProvider;
using PortalGovi.Models;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private DataManager dataManager;
        string sociedad;
        string sucursal;
        string cuenta;
        string operacion;
        string fecha;
        string usuario;
        string pass;
        public DataAppController(IConfiguration configuration, DataManager dataManager)
        {
            this.dataManager = new DataManager(configuration);
            this._configuration = configuration;
        }

        [HttpGet("ping")]
        public ActionResult Ping()
        {
            var results = new System.Collections.Generic.Dictionary<string, string>();
            
            // 1. Test SQL Connection
            try 
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(_configuration.GetConnectionString("SQL")))
                {
                    connection.Open();
                    results["SQL"] = "OK - Connected to 192.168.1.206";
                }
            }
            catch (Exception ex)
            {
                results["SQL"] = "FAILED: " + ex.Message;
            }

            // 2. Test SAP Service Layer (ApiSAP)
            try
            {
                var apiSapUrl = _configuration.GetConnectionString("ApiSAP");
                results["ApiSAP_URL"] = apiSapUrl;
                
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    // Just try to see if the port is open by making a request
                    var res = httpClient.GetAsync(apiSapUrl).GetAwaiter().GetResult();
                    results["ServiceLayer"] = $"OK - Status {(int)res.StatusCode} ({res.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                results["ServiceLayer"] = "FAILED: " + ex.Message;
                if (ex.InnerException != null) results["ServiceLayer_Detail"] = ex.InnerException.Message;
            }

            // 3. Test HANA Connection
            try
            {
                var hanaConnStr = _configuration.GetConnectionString("Sap");
                results["Hana_URL"] = hanaConnStr;
                // Attempt to check if the type can be resolved
                var hanaType = typeof(Sap.Data.Hana.HanaConnection);
                results["Hana_Assembly"] = hanaType.Assembly.FullName;

                using (var connH = new HanaConnection(hanaConnStr))
                {
                    connH.Open();
                    using (var cmd = new HanaCommand("SELECT VERSION FROM SYS.M_DATABASE", connH))
                    {
                        var version = cmd.ExecuteScalar();
                        results["Hana"] = $"OK - Version {version}";
                    }
                }
            }
            catch (FileLoadException flex)
            {
                results["Hana"] = "FAILED (DLL Mismatch): " + flex.Message;
                results["Hana_FusionLog"] = flex.FusionLog;
            }
            catch (Exception ex)
            {
                results["Hana"] = "FAILED: " + ex.Message;
                if (ex.InnerException != null) results["Hana_Detail"] = ex.InnerException.Message;
            }

            results["PATH"] = Environment.GetEnvironmentVariable("PATH");
            results["Process_Bitness"] = Environment.Is64BitProcess ? "64-bit" : "32-bit";
            results["File_Exists"] = System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libadonetHDB.dll")).ToString();

            return Ok(new { 
                status = results.Values.Any(v => v.StartsWith("FAILED")) ? "ERROR" : "OK",
                details = results,
                buildTimestamp = "2026-01-14 18:05" 
            });
        }

        [HttpGet("sociedades")]
        public ActionResult GetSociedades() //
        {
            List<Sociedad> dataResult = this.dataManager.ObtenerSociedades();

            return Ok(dataResult);
        }
        [HttpGet("sucursales")]
        public ActionResult GetSucursales() //
        {
            sociedad = Request.Query["sociedad"];
            List<Sucursal> dataResult = this.dataManager.ObtenerSucursales(sociedad);

            return Ok(dataResult);
        }
        [HttpGet("cuentas")]
        public ActionResult GetCuentas() //
        {
            sociedad = Request.Query["sociedad"];
            sucursal = Request.Query["sucursal"];
            List<Cuenta> dataResult = this.dataManager.ObtenerCuentas(sociedad, sucursal);

            return Ok(dataResult);
        }
        [HttpGet("transferencias")]
        public ActionResult GetTransfers() //
        {
            sociedad = Request.Query["sociedad"];
            sucursal = Request.Query["sucursal"];
            cuenta = Request.Query["cuenta"];
            operacion = Request.Query["operacion"];
            int year = sociedad == null ? 0 : int.Parse(Request.Query["year"]);
            List<Transferencia> dataResult = this.dataManager.ObtenerTransferencias(sociedad, sucursal, cuenta, operacion, year);

            return Ok(dataResult);
        }
        [HttpGet("pasivos")]
        public ActionResult GetPasivos() //
        {
            sociedad = Request.Query["sociedad"];
            sucursal = Request.Query["sucursal"];
            cuenta = Request.Query["cuenta"];
            var dataResult = this.dataManager.GetPasivo(sociedad, sucursal, cuenta);

            return Ok(dataResult);
        }
        [HttpGet("transferenciasDispersion")]
        public ActionResult GetTransfersDispersion() //
        {
            sociedad = Request.Query["sociedad"];
            sucursal = Request.Query["fecha1"];
            cuenta = Request.Query["fecha2"];

            var dataResult = this.dataManager.GetTransferencias(sociedad, sucursal, cuenta);

            return Ok(dataResult);
        }
        [HttpPost("updateDispersion")]
        public async Task<ActionResult> PostUpdateDispersionAsync([FromBody] dynamic transf)
        {
            sociedad = Request.Query["sociedad"];
            usuario = Request.Query["u"];
            pass = Request.Query["p"];
            var trans = JsonDocument.Parse(transf.ToString());
            await dataManager.UpdateDispersion(sociedad, usuario, pass, trans);
            return Ok();
        }
        [HttpGet("servicios")]
        public ActionResult GetServicios() //
        {
            List<Servicio> dataResult = this.dataManager.ObtenerTransferenciasServicios();

            return Ok(dataResult);
        }
        [HttpPost("transferencias")]
        public ActionResult PostTransfers([FromBody] List<Transferencia> transferencias)
        {
            sociedad = Request.Query["sociedad"];
            sucursal = ((string)Request.Query["sucursal"]).Replace(" ", "");
            operacion = Request.Query["operacion"];
            usuario = Request.Query["u"];
            pass = Request.Query["p"];

            string file = dataManager.GenerarArchivo(transferencias, sociedad, sucursal, operacion, usuario, pass);
            if (file == "" || !file.Contains(".pdf"))
            {
                return BadRequest(file);
            }

            Response.Headers.Add("filename", file);

            return Ok();

        }
        [HttpPost("servicios")]
        public ActionResult PostServicios([FromBody] List<Servicio> servicios)
        {
            usuario = Request.Query["u"];
            pass = Request.Query["p"];
            string generarTxt = Request.Query["g"]; //0 no generar archivos, 1 generar archivos
            if (generarTxt == "0")
            {
                return Ok(dataManager.GenerarServicio(servicios, usuario, pass));
            }
            List<string> files = dataManager.GenerarArchivoServicio(servicios, usuario, pass);
            if (files.Count == 0)
            {
                return BadRequest(files);
            }

            return Ok(files);

        }
        [HttpPost("transferenciasbyone")]
        public ActionResult PostTransfersByone([FromBody] List<Transferencia> transferencias)
        {
            sociedad = Request.Query["sociedad"];
            usuario = Request.Query["u"];
            pass = Request.Query["p"];
            string generarTxt = Request.Query["g"]; //0 no generar archivos, 1 generar archivos
            if (generarTxt == "0")
            {
                return Ok(dataManager.GenerarArchivoByone_(transferencias, usuario, pass));
            }
            List<string> files = dataManager.GenerarArchivoByone(transferencias, usuario, pass);
            if (files.Count == 0)
            {
                return BadRequest(files);
            }

            return Ok(files);

        }
        [HttpGet("download/{file}")]
        public IActionResult DownloadFile(string file)
        {
            try
            {
                string dest = @"C:\BANCOS\DISPERSION\REPORTES";
                IFileProvider provider = new PhysicalFileProvider(dest);
                IFileInfo fileInfo = provider.GetFileInfo(file);
                var readStream = fileInfo.CreateReadStream();
                var mimeType = "application/octet-stream";
                return new FileStreamResult(readStream, mimeType)
                {
                    FileDownloadName = file
                };

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        #region "Informes"
        [HttpGet("sdaldia")]
        public ActionResult GetSdaldia()
        {
            fecha = Request.Query["fecha"];
            List<SdAldia> dataresult = dataManager.ObtenerSDAldia(fecha);
            return Ok(dataresult);
        }
        [HttpPost("infotransfers")]
        public ActionResult GetInfoTransfers(Fechas fechas)
        {
            dynamic dataresult = dataManager.ObtenerTransfersHeader(fechas);
            return Ok(dataresult);
        }
        [HttpPost("detailstransfers")]
        public ActionResult GetDetailsTransfers(Fechas fechas)
        {
            sociedad = Request.Query["empresa"];
            dynamic dataresult = dataManager.ObtenerTransfersDetails(sociedad, fechas);
            return Ok(dataresult);
        }
        [HttpGet("informe")]
        [DisableRequestSizeLimit]
        public ActionResult GetInforme()
        {
            Fechas fechas = new Fechas();
            fechas.FechaIni = Request.Query["fechaini"];
            fechas.FechaFin = Request.Query["fechafin"];
            string id = Request.Query["informe"];

            dynamic result = dataManager.GetInforme(id, fechas);
            return Ok(result);
        }
        [HttpGet("cuadroinversion")]
        [DisableRequestSizeLimit]
        public ActionResult OnGetCuadroInversion(string inputFechas)
        {
            string[] fechas = inputFechas.Split(',');

            var result = dataManager.GetCuadroInversion(fechas);
            return Ok(result);
        }
        [HttpGet("parametros")]
        [DisableRequestSizeLimit]
        public ActionResult GetParametros()
        {
            string id = Request.Query["informe"];

            dynamic result = dataManager.GetParametros(id);
            return Ok(result);
        }
        #endregion
        /**/
        [HttpGet("cedis")]
        public ActionResult GetCedis()
        {
            List<Sucursal> dataresult = dataManager.ObtenerCedis();
            return Ok(dataresult);
        }
        [HttpPost("salida")]
        [DisableRequestSizeLimit]
        public ActionResult PostSalida([FromBody] AjusteRequest ajusteRequest)
        {
            usuario = Request.Query["u"];
            ActionsResponse resp = dataManager.AjusteSalida(ajusteRequest.Ajustes, ajusteRequest.Motivo, usuario, ajusteRequest.Sucursal, Request).Result;
            if (resp.ok)
            {
                return Ok(resp.message);
            }
            return BadRequest(resp.message);
        }
        [HttpPost("entrada")]
        [DisableRequestSizeLimit]
        public ActionResult PostEntrada([FromBody] AjusteRequest ajusteRequest)
        {
            usuario = Request.Query["u"];
            ActionsResponse resp = dataManager.AjusteEntrada(ajusteRequest.Ajustes, ajusteRequest.Motivo, usuario, ajusteRequest.Sucursal, Request).Result;
            if (resp.ok)
            {
                return Ok(resp.message);
            }
            return BadRequest(resp.message);
        }
        /**/
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginApiData loginApiData)
        {
            try 
            {
                var cookies = await dataManager.Login(loginApiData);

                if (cookies != null)
                {
                    Response.Headers.Add("B1SESSION", cookies.ElementAt(0).Split("=")[1]);
                    Response.Headers.Add("ROUTEID", cookies.ElementAt(1).Split("=")[1]);
                    return Ok();
                }

                return Unauthorized("Credenciales inválidas o acceso denegado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("loginpagos")]
        public async Task<ActionResult> Login2([FromBody] LoginApiData loginApiData)
        {
            try 
            {
                var cookies = await dataManager.Login(loginApiData);

                if (cookies != null)
                {
                    Response.Headers.Add("B1SESSION", cookies.ElementAt(0).Split("=")[1]);
                    Response.Headers.Add("ROUTEID", cookies.ElementAt(1).Split("=")[1]);
                    return Ok();
                }

                return Unauthorized("Credenciales inválidas o acceso denegado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        #region "Tunel bancario"
        [HttpPost("uploadtxt"), DisableRequestSizeLimit]
        public IActionResult OnPostUpnloadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            List<dynamic> result = dataManager.UploadTxt(files).Result;
            return Ok(result);
        }

        [HttpPost("tunel"), DisableRequestSizeLimit]
        public IActionResult OnPostTunel(string[] nameFiles)
        {
            ActionsResponse resp = dataManager.EnviarTunel(nameFiles);
            if (resp.ok)
            {
                return Ok(resp.message);

            }
            return Ok(resp.message);
        }

        [HttpPost("uploadtxtservicio"), DisableRequestSizeLimit]
        public IActionResult OnPostUpnloadServicioAsync(List<IFormFile> files)
        {
            List<dynamic> result = dataManager.UploadTxtServicio(files).Result;
            return Ok(result);
        }

        [HttpPost("tunelservicio"), DisableRequestSizeLimit]
        public IActionResult OnPostTunelServicio(string[] nameFiles)
        {
            ActionsResponse resp = dataManager.EnviarTunelServicio(nameFiles);
            if (resp.ok)
            {
                return Ok(resp.message);

            }
            return Ok(resp.message);
        }

        [HttpPost("informe"), DisableRequestSizeLimit]
        public IActionResult informe(Fechas fechas)
        {
            dynamic result = dataManager.GenerarInformeTunel(fechas);
            return Ok(result);
        }
        #endregion

        [HttpPatch("clase")]
        public ActionResult PatchClase()
        {
            dataManager.UpdateClase();
            return NoContent();
        }

        [HttpPost("notadebito")]
        [DisableRequestSizeLimit]
        public ActionResult PostNotaDebito([FromBody] NotaDebitoRequestModel notaDebitoRequest)
        {
            var resp = dataManager.CrearNotaDebito(notaDebitoRequest.Notas, notaDebitoRequest.Login).GetAwaiter().GetResult();
            if (!resp.ok)
            {
                return BadRequest(resp.message);
            }
            
            return Ok(resp.Data);
            
        }

        [HttpPost("pagofiliales")]
        [DisableRequestSizeLimit]
        public ActionResult PostPagoFiliales([FromBody] List<FiliarModel> filiarModels)
        {
            sociedad = Request.Query["sociedad"];
            sucursal = Request.Query["sucursal"];
            string proveedor = Request.Query["proveedor"];
            usuario = Request.Query["u"];
            pass = Request.Query["p"];
            var resp = dataManager.CrearPagoFilial(filiarModels, sociedad, sucursal, proveedor, usuario, pass, Request).GetAwaiter().GetResult();
            if (!resp.ok)
            {
                return BadRequest(resp.message);
            }

            return Ok(resp.Data);

        }

        [HttpPost("pasivos")]
        [DisableRequestSizeLimit]
        public ActionResult PostPasivos([FromBody] List<PasivoModel> pasivoModels)
        {
            sociedad = Request.Query["sociedad"];
            usuario = Request.Query["u"];
            pass = Request.Query["p"];
            var resp = dataManager.CrearPasivos(pasivoModels, sociedad, usuario, pass).GetAwaiter().GetResult();
            if (!resp.ok)
            {
                return BadRequest(resp.message);
            }

            return Ok(resp.Data);

        }


    }
}

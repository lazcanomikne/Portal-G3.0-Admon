using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortalGovi.DataProvider;
using PortalGovi.Models.Configuracion;
using System.Threading.Tasks;
using Dapper;

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigCuentasController : ControllerBase
    {
        private CuentasManager _manager;
        private readonly IConfiguration _configuration;

        public ConfigCuentasController(IConfiguration config)
        {
            _configuration = config;
            _manager = new CuentasManager(config);
        }

        [HttpGet("principales")]
        public async Task<IActionResult> GetPrincipales()
        {
            return Ok(await _manager.GetPrincipales());
        }

        [HttpPost("principales")]
        public async Task<IActionResult> AddPrincipal([FromBody] CuentaPrincipalModel model)
        {
            return Ok(await _manager.AddPrincipal(model));
        }

        [HttpGet("dependencias/{ordenId}")]
        public async Task<IActionResult> GetDependencias(int ordenId)
        {
            return Ok(await _manager.GetDependencias(ordenId));
        }

        [HttpPost("usd")]
        public async Task<IActionResult> AddUSD([FromBody] CuentaDependienteModel model)
        {
            await _manager.AddUSD(model);
            return Ok("Agregado USD");
        }

        [HttpPost("referenciada")]
        public async Task<IActionResult> AddReferenciada([FromBody] CuentaDependienteModel model)
        {
            await _manager.AddReferenciada(model);
            return Ok("Agregado Referencia");
        }
    }
}

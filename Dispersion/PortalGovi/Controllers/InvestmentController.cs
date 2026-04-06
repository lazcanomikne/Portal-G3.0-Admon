using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortalGovi.DataProvider;
using PortalGovi.Models.Investment;
using System.Threading.Tasks;

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    public class InvestmentController : ControllerBase
    {
        private InvestmentManager _investmentManager;

        public InvestmentController(IConfiguration configuration)
        {
            _investmentManager = new InvestmentManager(configuration);
        }

        // GET: SaldoDiponible
        [HttpGet("saldodisponible")]
        public async Task<IActionResult> OnGetSaldoDisponible(string fecha)
        {
            var result = await _investmentManager.GetSaldoDisponible(fecha);
            return Ok(result);
        }

        // POST: SaldoDisponible
        [HttpPost("saldodisponible")]
        public async Task<IActionResult> OnPostSaldoDisponible([FromBody] SaldoDisponibleModel request)
        {
            var result = await _investmentManager.InsertSaldoDisponible(request);
            return Ok(result);
        }

        // DELETE: SaldoDisponible
        [HttpDelete("saldodisponible/{id}")]
        public async Task<IActionResult> OnPostSaldoDisponible(int id)
        {
            var result = await _investmentManager.DeleteSaldoDisponible(id);
            return Ok(result);
        }

        // GET: SaldoFijo
        [HttpGet("saldofijo")]
        public async Task<IActionResult> OnGetSaldoFijo()
        {
            return Ok(await _investmentManager.GetSaldoFijo());
        }
        // GET: SaldoFijo
        [HttpPost("saldofijo")]
        public async Task<IActionResult> OnPostSaldoFijo([FromBody] SaldoFijoModel saldoFijoModel)
        {
            return Ok(await _investmentManager.InsertSaldoFijo(saldoFijoModel));
        }
        // GET: SaldoFijo
        [HttpPut("saldofijo/{id}")]
        public async Task<IActionResult> OnPutSaldoFijo(int id, [FromBody] SaldoFijoModel saldoFijoModel)
        {
            return Ok(await _investmentManager.UpdateSaldoFijo(id, saldoFijoModel));
        }
        // GET: SaldoFijo
        [HttpDelete("saldofijo/{id}")]
        public async Task<IActionResult> OnGetSaldoFijo(int id)
        {
            return Ok(await _investmentManager.DeleteSaldoFijo(id));
        }

        
        // ---------------------------------------------------------
        // MODULO APARTADOS
        // ---------------------------------------------------------

        // GET: Obtener lista filtrada por fecha
        [HttpGet("apartados")]
        public async Task<IActionResult> OnGetApartados(string fecha)
        {
            var result = await _investmentManager.GetApartados(fecha);
            return Ok(result);
        }

        // POST: Guardar registro manual (Uno solo)
        [HttpPost("apartados/manual")]
        public async Task<IActionResult> OnPostApartadoManual([FromBody] ApartadoModel request)
        {
            var result = await _investmentManager.InsertApartado(request);
            return Ok(result);
        }

        // POST: Carga Masiva Excel (Lista)
        [HttpPost("apartados/bulk")]
        public async Task<IActionResult> OnPostApartadosBulk([FromBody] System.Collections.Generic.List<ApartadoModel> request)
        {
            var result = await _investmentManager.InsertApartadosList(request);
            return Ok(result);
        }
        // PUT: Actualizar un apartado existente
        [HttpPut("apartados/{id}")]
        public async Task<IActionResult> OnPutApartado(int id, [FromBody] ApartadoModel request)
        {
            request.ID = id;
            var result = await _investmentManager.UpdateApartado(request);
            return Ok(result);
        }

        // DELETE: Eliminar una línea
        [HttpDelete("apartados/{id}")]
        public async Task<IActionResult> OnDeleteApartado(int id)
        {
            var result = await _investmentManager.DeleteApartado(id);
            return Ok(result);
        }
        // GET: Obtener lista de cuentas ordenadas
        [HttpGet("cuentas/orden")]
        public async Task<IActionResult> OnGetCuentasOrden()
        {
            var result = await _investmentManager.GetCuentasOrden();
            return Ok(result);
        }
    }
}

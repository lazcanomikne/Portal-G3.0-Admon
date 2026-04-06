using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortalGovi.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private CreditManager _creditManager;
        string sociedad;
        string sucursal;
        string cuenta;
        string operacion;
        string fecha;
        string usuario;

        public CreditController(IConfiguration configuration)
        {
            _configuration = configuration;
            _creditManager = new CreditManager(configuration);
        }

        // GET: api/<CreditController>
        [HttpGet("customers")]
        public IActionResult GetCustomer()
        {
            sociedad = Request.Query["sociedad"];
            sucursal = Request.Query["sucursal"];
            return Ok(_creditManager.GetCustomers(sociedad, sucursal));
        }

        // GET api/<CreditController>/5
        [HttpGet("pagocta")]
        public IActionResult GetPagoCta()
        {
            sociedad = Request.Query["sociedad"];
            cuenta = Request.Query["cuenta"];
            return Ok(_creditManager.GetPagoCta(sociedad, cuenta));
        }
        
        [HttpGet("pendingbill")]
        public IActionResult GetSaldoPendiente()
        {
            sociedad = Request.Query["sociedad"];
            string cliente = Request.Query["cliente"];
            return Ok(_creditManager.GetPendingBill(sociedad, cliente));
        }
        
        [HttpGet("saldoFavor")]
        public IActionResult GetSaldoFavor()
        {
            string cliente = Request.Query["cliente"];
            return Ok(_creditManager.GetSaldoFavor(cliente));
        }

        [HttpGet("typediscount")]
        public IActionResult GetTypeDiscount()
        {
            sociedad = Request.Query["sociedad"];
            return Ok(_creditManager.GetTypeDiscount(sociedad));
        }

        [HttpPost("pagos")]
        public async Task<IActionResult> PostPagos(CYC_PagosEncabezado pago)
        {
            int result = await _creditManager.InsertarPagoAsync(pago);
            return Ok(result);
        }

        [HttpGet("pagos/{id}")]
        public async Task<IActionResult> GetPagoById(int id)
        {
            var pago = await _creditManager.GetPagoByIdAsync(id);
            if (pago == null)
                return NotFound();
            return Ok(pago);
        }

        [HttpDelete("pagos/{id}")]
        public async Task<IActionResult> DeletePagoById(int id)
        {
            var deleted = await _creditManager.DeletePagoByIdAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpGet("operation/header")]
        public IActionResult GetOperationHeader()
        {
            return Ok(_creditManager.GetOperationHeader().GetAwaiter().GetResult());
        }

        [HttpPut("operation/header")]
        public IActionResult PutOperationHeader([FromQuery] string folio)
        {
            _creditManager.UpdateOperationHeader(folio).GetAwaiter().GetResult();
            return NoContent();
        }

        [HttpGet("report/header")]
        public IActionResult GetResporHeader()
        {
            fecha = Request.Query["fecha"];
            return Ok(_creditManager.GetResporHeader(fecha).GetAwaiter().GetResult());
        }

        [HttpGet("report/details")]
        public IActionResult GetResporDetails()
        {
            operacion = Request.Query["folio"];
            return Ok(_creditManager.GetResporDetails(folio: operacion).GetAwaiter().GetResult());
        }
        
        [HttpGet("report/header/cuadroinversion")]
        public IActionResult GetResportHeaderCuadroInversion()
        {
            fecha = Request.Query["fecha"];
            return Ok(_creditManager.GetResportHeaderCuadroInversion(fecha).GetAwaiter().GetResult());
        }

        [HttpGet("report/details/cuadroinversion")]
        public IActionResult GetResportDetailsCuadroInversion()
        {
            operacion = Request.Query["cuenta"];
            return Ok(_creditManager.GetResporDetails(folio: operacion).GetAwaiter().GetResult());
        }

    }
}

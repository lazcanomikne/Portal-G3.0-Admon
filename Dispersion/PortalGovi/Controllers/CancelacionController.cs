using Microsoft.AspNetCore.Mvc;
using PortalGovi.DataProvider;
using PortalGovi.Models.Cancelacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CancelacionController : ControllerBase
    {
        private readonly CancelacionManager _cancelacionManager;

        public CancelacionController(CancelacionManager cancelacionManager)
        {
            _cancelacionManager = cancelacionManager;
        }

        [HttpPost]
        public async Task<IActionResult> OnPost([FromBody] List<CancelacionMasiva> list)
        {
            var result = await _cancelacionManager.CargarData(list);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> OnGet([FromQuery] string fecha) {
            var sFecha = fecha == null ? DateTime.Now.ToString("yyyy-MM-dd") : fecha;
            var result = await _cancelacionManager.GetMasics(sFecha);
            return Ok(result); 
        }

        [HttpPut]
        public async Task<IActionResult> OnPut()
        {
            await _cancelacionManager.UpdatePending();
            return Ok();
        }
    }
}

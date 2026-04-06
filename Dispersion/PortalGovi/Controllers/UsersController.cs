using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortalGovi.DataProvider;
using PortalGovi.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UsersManager _manager;

        public UsersController(UsersManager manager)
        {
            _manager = manager;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("UsersController is alive");

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _manager.GetUsuarios());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UsuarioModel model)
        {
            var result = await _manager.CreateUsuario(model);
            if (result != "OK") return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("test-sap")]
        public async Task<IActionResult> TestSap([FromBody] UsuarioModel model)
        {
            bool success = await _manager.TestSapLogin(model.UserName, model.Password);
            if (success) return Ok(new { message = "Conexión a SAP Exitosa" });
            return BadRequest(new { message = "Fallo la autenticación en SAP" });
        }

        [HttpGet("permissions/{username}")]
        public async Task<IActionResult> GetPermissions(string username)
        {
            return Ok(await _manager.GetUserPermissionsTree(username));
        }

        [HttpPost("permissions/{username}")]
        public async Task<IActionResult> SavePermissions(string username, [FromBody] List<UserPermissionRequest> perms)
        {
            await _manager.SavePermissions(username, perms);
            return Ok("Permisos actualizados");
        }
    }
}

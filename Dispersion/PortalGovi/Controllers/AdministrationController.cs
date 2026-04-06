using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortalGovi.DataProvider;
using PortalGovi.Models.Administration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortalGovi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private AdministrationManager _adminManager;

        public AdministrationController(IConfiguration configuration)
        {
            _adminManager = new AdministrationManager(configuration);
        }

        // 1. Obtener Usuarios
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _adminManager.GetUsers());
        }

        // 2. Crear Usuario
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] UsuarioModel user)
        {
            if (await _adminManager.UserExists(user.UserName))
                return BadRequest("El usuario ya existe.");

            return Ok(await _adminManager.CreateUser(user));
        }

        // 3. Test SAP Login
        [HttpPost("test-sap")]
        public async Task<IActionResult> TestSapLogin([FromBody] UsuarioModel user)
        {
            bool isValid = await _adminManager.ValidateSapCredentials(user.UserName, user.Password);
            if (isValid) return Ok(new { message = "Conexión a SAP exitosa" });
            return BadRequest("Credenciales inválidas en SAP");
        }

        // 4. Obtener Estructura de Permisos para un Usuario
        [HttpGet("permissions/{username}")]
        public async Task<IActionResult> GetPermissions(string username)
        {
            return Ok(await _adminManager.GetMenuPermissionsTree(username));
        }

        // 5. Guardar Permisos
        [HttpPost("permissions")]
        public async Task<IActionResult> SavePermissions([FromBody] GuardarPermisoRequest request)
        {
            return Ok(await _adminManager.UpdateUserPermissions(request));
        }
    }
}

using System.Collections.Generic;

namespace PortalGovi.Models.Administration
{
    public class UsuarioModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    // Modelo para mostrar el árbol de permisos
    public class MenuPermisoModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public List<SubMenuPermisoModel> SubMenus { get; set; } = new List<SubMenuPermisoModel>();
    }

    public class SubMenuPermisoModel
    {
        public int Id { get; set; }
        public int IdMenu { get; set; }
        public string Tag { get; set; }
        public bool TieneAcceso { get; set; } // Si existe en tabla MenuUsuarios
        public bool CanCreate { get; set; }   // Campo especifico
    }

    // Modelo para guardar permisos
    public class GuardarPermisoRequest
    {
        public string UserName { get; set; }
        public List<SubMenuUsuario> Permisos { get; set; }
    }

    public class SubMenuUsuario 
    {
        public int IdSubMenu { get; set; }
        public bool CanCreate { get; set; }
    }
}

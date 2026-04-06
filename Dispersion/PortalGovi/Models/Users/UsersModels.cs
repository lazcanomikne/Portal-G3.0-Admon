using System.Collections.Generic;

namespace PortalGovi.Models.Users
{
    public class UsuarioModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class MenuTreeModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public bool HasAccess { get; set; } // Propiedad para el checkbox del título
        public List<SubMenuModel> SubMenus { get; set; } = new List<SubMenuModel>();
    }

    public class SubMenuModel
    {
        public int Id { get; set; }
        public int IdMenu { get; set; }
        public string Tag { get; set; }
        public bool HasAccess { get; set; } // Para el checkbox del UI
        public bool CanCreate { get; set; } // Campo específico de la tabla
        public string Path { get; set; }
    }

    public class UserPermissionRequest
    {
        public string UserName { get; set; }
        public int IdSubMenu { get; set; }
        public bool CanCreate { get; set; }
    }
}

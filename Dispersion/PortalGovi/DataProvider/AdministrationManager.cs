using Dapper;
using Microsoft.Extensions.Configuration;
using PortalGovi.Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.DataProvider
{
    public class AdministrationManager
    {
        private readonly string _connString;

        public AdministrationManager(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString("SQL");
        }

        public async Task<IEnumerable<UsuarioModel>> GetUsers()
        {
            using var db = new SqlConnection(_connString);
            return await db.QueryAsync<UsuarioModel>("SELECT UserName, Password FROM [DISPERSION].[dbo].[usuarios]");
        }

        public async Task<bool> UserExists(string username)
        {
            using var db = new SqlConnection(_connString);
            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM [DISPERSION].[dbo].[usuarios] WHERE UserName = @username", new { username });
            return count > 0;
        }

        public async Task<int> CreateUser(UsuarioModel user)
        {
            using var db = new SqlConnection(_connString);
            return await db.ExecuteAsync("INSERT INTO [DISPERSION].[dbo].[usuarios] (UserName, Password, Status) VALUES (@UserName, @Password, 1)", user);
        }

        public async Task<bool> ValidateSapCredentials(string username, string password)
        {
            using var db = new SqlConnection(_connString);
            var user = await db.QueryFirstOrDefaultAsync<UsuarioModel>(
                "SELECT UserName, Password FROM [DISPERSION].[dbo].[usuarios] WHERE UserName = @username AND Password = @password",
                new { username, password }
            );
            return user != null;
        }

        public async Task<List<MenuPermisoModel>> GetMenuPermissionsTree(string username)
        {
            using var db = new SqlConnection(_connString);
            
            // 1. Obtener todos los Menús
            var menus = (await db.QueryAsync<MenuPermisoModel>("SELECT Id, Tag, Icon FROM [DISPERSION].[dbo].[Menu]")).ToList();

            // 2. Obtener todos los SubMenús con información de acceso para el usuario
            var subMenusRaw = await db.QueryAsync<dynamic>(@"
                SELECT 
                    s.Id, 
                    s.IdMenu, 
                    s.Tag,
                    CASE WHEN mu.IdSubMenu IS NOT NULL THEN 1 ELSE 0 END as TieneAcceso,
                    ISNULL(mu.CanCreate, 0) as CanCreate
                FROM [DISPERSION].[dbo].[SubMenu] s
                LEFT JOIN [DISPERSION].[dbo].[MenuUsuarios] mu ON s.Id = mu.IdSubMenu AND mu.UserName = @username",
                new { username }
            );

            // 3. Organizar en árbol
            foreach (var menu in menus)
            {
                var relatedSubMenus = subMenusRaw
                    .Where(s => (int)s.IdMenu == menu.Id)
                    .Select(s => new SubMenuPermisoModel
                    {
                        Id = (int)s.Id,
                        IdMenu = (int)s.IdMenu,
                        Tag = (string)s.Tag,
                        TieneAcceso = s.TieneAcceso > 0,
                        CanCreate = s.CanCreate == true || (s.CanCreate is int && (int)s.CanCreate == 1)
                    }).ToList();
                
                menu.SubMenus = relatedSubMenus;
            }

            return menus;
        }

        public async Task<bool> UpdateUserPermissions(GuardarPermisoRequest request)
        {
            using var db = new SqlConnection(_connString);
            await db.OpenAsync();
            using var transaction = db.BeginTransaction();

            try
            {
                // 1. Borrar permisos actuales
                await db.ExecuteAsync("DELETE FROM [DISPERSION].[dbo].[MenuUsuarios] WHERE UserName = @UserName", new { UserName = request.UserName }, transaction);

                // 2. Insertar nuevos permisos
                if (request.Permisos != null && request.Permisos.Any())
                {
                    var sql = "INSERT INTO [DISPERSION].[dbo].[MenuUsuarios] (UserName, IdSubMenu, CanCreate) VALUES (@UserName, @IdSubMenu, @CanCreate)";
                    foreach (var p in request.Permisos)
                    {
                        await db.ExecuteAsync(sql, new { UserName = request.UserName, p.IdSubMenu, p.CanCreate }, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

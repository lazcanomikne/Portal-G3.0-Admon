using Dapper;
using Microsoft.Extensions.Configuration;
using PortalGovi.Models.Users;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using PortalGovi.Models;

namespace PortalGovi.DataProvider
{
    public class UsersManager
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersManager(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _connectionString = configuration.GetConnectionString("SQL");
        }

        // 1. Obtener Usuarios
        public async Task<IEnumerable<UsuarioModel>> GetUsuarios()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT [UserName], [Password] FROM [DISPERSION].[dbo].[usuarios]";
            return await connection.QueryAsync<UsuarioModel>(sql);
        }

        // 2. Crear Usuario (Con validación de existencia)
        public async Task<string> CreateUsuario(UsuarioModel user)
        {
            using var connection = new SqlConnection(_connectionString);
            
            // Validar si existe
            var exists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM [DISPERSION].[dbo].[usuarios] WHERE UserName = @UserName", 
                new { user.UserName });

            if (exists > 0) return "El usuario ya existe.";

            var sql = "INSERT INTO [DISPERSION].[dbo].[usuarios] (UserName, Password, Status) VALUES (@UserName, @Password, 1)";
            await connection.ExecuteAsync(sql, user);
            return "OK";
        }

        // 3. Obtener Árbol de Menús y Permisos de un Usuario
        public async Task<List<MenuTreeModel>> GetUserPermissionsTree(string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            
            // Traemos Menus y Submenus
            var menus = (await connection.QueryAsync<MenuTreeModel>("SELECT * FROM [DISPERSION].[dbo].[Menu]")).ToList();
            var submenus = await connection.QueryAsync<SubMenuModel>("SELECT * FROM [DISPERSION].[dbo].[SubMenu]");
            
            // Traemos los permisos actuales del usuario
            var userPerms = await connection.QueryAsync<UserPermissionRequest>(
                "SELECT * FROM [DISPERSION].[dbo].[MenuUsuarios] WHERE UserName = @UserName", 
                new { UserName = userName });

            // Armamos el árbol
            foreach (var menu in menus)
            {
                var children = submenus.Where(s => s.IdMenu == menu.Id).ToList();
                foreach(var child in children)
                {
                    var perm = userPerms.FirstOrDefault(p => p.IdSubMenu == child.Id);
                    if(perm != null)
                    {
                        child.HasAccess = true;
                        child.CanCreate = perm.CanCreate;
                    }
                }
                menu.SubMenus = children;
                menu.HasAccess = children.Any(c => c.HasAccess);
            }
            return menus;
        }

        // 4. Guardar Permisos (Borra anteriores y pone nuevos)
        public async Task SavePermissions(string userName, List<UserPermissionRequest> permissions)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var trans = connection.BeginTransaction();

            try
            {
                // Limpiar permisos anteriores
                await connection.ExecuteAsync(
                    "DELETE FROM [DISPERSION].[dbo].[MenuUsuarios] WHERE UserName = @UserName", 
                    new { UserName = userName }, trans);

                // Insertar nuevos
                if (permissions.Any())
                {
                    var sql = @"INSERT INTO [DISPERSION].[dbo].[MenuUsuarios] (UserName, IdSubMenu, CanCreate) 
                                VALUES (@UserName, @IdSubMenu, @CanCreate)";
                    
                    // Asegurar que el UserName viaja en cada objeto
                    permissions.ForEach(p => p.UserName = userName); 
                    await connection.ExecuteAsync(sql, permissions, trans);
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        // 5. Test SAP Real (Contra Service Layer)
        public async Task<bool> TestSapLogin(string user, string pass)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                
                using var client = new HttpClient(handler);
                
                var loginData = new
                {
                    CompanyDB = _configuration.GetValue<string>("UserData:CompanyDB") ?? "SBOGOVI",
                    UserName = user,
                    Password = pass
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(loginData),
                    Encoding.UTF8,
                    "application/json");

                var apiSapUrl = _configuration.GetConnectionString("ApiSAP");
                var response = await client.PostAsync(apiSapUrl + "Login", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                System.Diagnostics.Debug.WriteLine($"SAP Login Test Error: {ex.Message}");
                return false;
            }
        }
    }
}

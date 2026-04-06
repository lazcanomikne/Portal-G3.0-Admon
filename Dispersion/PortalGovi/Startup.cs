using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortalGovi.DataProvider;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace PortalGovi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddMvc();
            services.AddControllers();
            services.AddCors(o => o.AddPolicy("pol", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("filename", "B1SESSION", "ROUTEID");

            }));
            services.AddSingleton(Configuration);
            services.AddTransient<DataManager>();
            services.AddTransient<ConfigManager>();
            services.AddTransient<CancelacionManager>();
            services.AddTransient<UsersManager>();
            services.AddTransient<CuentasManager>();
            services.AddTransient<AdministrationManager>();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "API v1");

                // custom CSS
                c.InjectStylesheet("/swagger-ui/custom.css");
            });
            app.UseDeveloperExceptionPage();
            //app.UseDatabaseErrorPage();
            //app.UseDatabaseErrorPage();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseCors("pol");

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

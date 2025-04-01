using AlquimiaEsencial.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlquimiaEsencial
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ?? Conexión a base de datos
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // ?? Identity + Roles
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // ?? MVC y Razor Pages
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();
            Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");


            // ?? Crear roles iniciales al iniciar
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await ConfiguracionInicial.CrearRolesIniciales(services);
            }

            // ??? Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // ?? Rutas por defecto
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");


            await app.RunAsync();
        }
    }
}

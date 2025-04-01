using Microsoft.AspNetCore.Identity;

namespace AlquimiaEsencial.Data
{
    public static class ConfiguracionInicial
    {
        public static async Task CrearRolesIniciales(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Cliente" };

            // Crear roles si no existen
            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            // Crear usuario admin si no existe
            var emailAdmin = "admin@alquimia.com";
            var userAdmin = await userManager.FindByEmailAsync(emailAdmin);

            if (userAdmin == null)
            {
                var usuario = new IdentityUser
                {
                    UserName = emailAdmin,
                    Email = emailAdmin,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuario, "Admin123*");
                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuario, "Admin");
                }
            }
        }
    }
}


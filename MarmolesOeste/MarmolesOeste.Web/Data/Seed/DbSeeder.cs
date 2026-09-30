using MarmolesOeste.Web.Data.Entities;
using MarmolesOeste.Web.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MarmolesOeste.Web.Data.Seed;

// Datos de prueba para desarrollo. Es idempotente: si ya hay materiales
// cargados, no vuelve a insertar nada (así se puede llamar en cada arranque
// sin duplicar filas).
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);

        if (await context.Materiales.AnyAsync())
        {
            return;
        }

        context.Materiales.AddRange(
            new Material { Nombre = "Granito Gris Mara", Tipo = TipoMaterial.Granito, Espesor = 2, PrecioPorM2 = 180_000m, PrecioPorMetroLinealZocalo = 12_000m },
            new Material { Nombre = "Granito Negro Absoluto", Tipo = TipoMaterial.Granito, Espesor = 2, PrecioPorM2 = 210_000m, PrecioPorMetroLinealZocalo = 14_000m },
            new Material { Nombre = "Mármol Travertino", Tipo = TipoMaterial.Marmol, Espesor = 2, PrecioPorM2 = 195_000m, PrecioPorMetroLinealZocalo = 13_000m },
            new Material { Nombre = "Mármol Blanco Carrara", Tipo = TipoMaterial.Marmol, Espesor = 2, PrecioPorM2 = 260_000m, PrecioPorMetroLinealZocalo = 16_000m },
            new Material { Nombre = "Cuarzo Blanco Ártico", Tipo = TipoMaterial.Cuarzo, Espesor = 2, PrecioPorM2 = 230_000m, PrecioPorMetroLinealZocalo = 15_000m },
            new Material { Nombre = "Cuarzo Gris Cemento", Tipo = TipoMaterial.Cuarzo, Espesor = 2, PrecioPorM2 = 225_000m, PrecioPorMetroLinealZocalo = 15_000m }
        );

        context.TerminacionesCanto.AddRange(
            new TerminacionCanto { Nombre = "Pulido recto", PrecioPorMetroLineal = 8_000m },
            new TerminacionCanto { Nombre = "Boleado", PrecioPorMetroLineal = 10_000m },
            new TerminacionCanto { Nombre = "Media caña", PrecioPorMetroLineal = 10_000m },
            new TerminacionCanto { Nombre = "Bisel", PrecioPorMetroLineal = 9_000m },
            new TerminacionCanto { Nombre = "Laminado", PrecioPorMetroLineal = 15_000m }
        );

        context.Adicionales.AddRange(
            new Adicional { Nombre = "Colocación", Precio = 45_000m, UnidadMedida = UnidadMedidaAdicional.MetroCuadrado },
            new Adicional { Nombre = "Flete", Precio = 25_000m, UnidadMedida = UnidadMedidaAdicional.Global },
            new Adicional { Nombre = "Medición en obra", Precio = 15_000m, UnidadMedida = UnidadMedidaAdicional.Global },
            new Adicional { Nombre = "Calado de bacha", Precio = 20_000m, UnidadMedida = UnidadMedidaAdicional.Unidad },
            new Adicional { Nombre = "Calado de anafe", Precio = 20_000m, UnidadMedida = UnidadMedidaAdicional.Unidad },
            new Adicional { Nombre = "Perforación de grifería", Precio = 6_000m, UnidadMedida = UnidadMedidaAdicional.Unidad }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { Roles.Dueno, Roles.Empleado })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    // Usuarios de desarrollo para poder loguearse y probar el panel.
    // EmailConfirmed = true a mano porque no hay envío de mail real configurado
    // (IdentityNoOpEmailSender) y RequireConfirmedAccount está en true.
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserIfMissingAsync(userManager, "dueno@marmolesoeste.com", "Dueno123!", "Dueño", Roles.Dueno);
        await CreateUserIfMissingAsync(userManager, "empleado@marmolesoeste.com", "Empleado123!", "Empleado", Roles.Empleado);
    }

    private static async Task CreateUserIfMissingAsync(
        UserManager<ApplicationUser> userManager, string email, string password, string nombre, string rol)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Nombre = nombre,
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, rol);
        }
    }
}

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarmolesOeste.Web.Components;
using MarmolesOeste.Web.Components.Account;
using MarmolesOeste.Web.Data;
using MarmolesOeste.Web.Data.Seed;
using MudBlazor.Services;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using MarmolesOeste.Web.Pdf;

// QuestPDF exige declarar una licencia antes de generar cualquier documento.
// La licencia Community es gratuita para empresas con facturación anual menor
// a USD 1M — cambiar a LicenseType.Professional/Enterprise si eso deja de
// aplicar a Mármoles Oeste.
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// IDbContextFactory en vez de AddDbContext directo: en Blazor Server el DbContext
// scoped se comparte durante todo el circuito del usuario, y dos operaciones async
// concurrentes sobre la misma instancia (algo fácil de pisar sin querer en páginas
// con varias cargas de datos) tiran "A second operation was started on this context".
// La factory nos da una instancia nueva y corta por cada operación. También registramos
// ApplicationDbContext scoped porque Identity (AddEntityFrameworkStores) lo necesita así.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddMudServices();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Solo en desarrollo: aplica migraciones pendientes y carga datos de prueba
    // al arrancar. En producción las migraciones se aplican a mano con
    // `dotnet ef database update`, no automáticamente al levantar la app.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Endpoint HTTP normal (no un componente Blazor) porque la descarga de un archivo
// necesita una respuesta HTTP real con Content-Disposition; eso no existe dentro
// de un circuito de SignalR.
app.MapGet("/api/presupuestos/{id:int}/pdf", async (int id, IDbContextFactory<ApplicationDbContext> dbFactory) =>
{
    await using var db = await dbFactory.CreateDbContextAsync();
    var presupuesto = await db.Presupuestos
        .Include(p => p.Cliente)
        .Include(p => p.Piezas).ThenInclude(pz => pz.Tramos)
        .Include(p => p.Piezas).ThenInclude(pz => pz.Calados)
        .Include(p => p.Adicionales)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (presupuesto is null)
    {
        return Results.NotFound();
    }

    var pdfBytes = new PresupuestoPdfDocument(presupuesto).GeneratePdf();
    return Results.File(pdfBytes, "application/pdf", $"Presupuesto-{presupuesto.NumeroCorrelativo}.pdf");
}).RequireAuthorization();

app.MapGet("/api/facturacion/exportar", async (int year, int month, string formato, IDbContextFactory<ApplicationDbContext> dbFactory) =>
{
    if (month is < 1 or > 12)
    {
        return Results.BadRequest("Mes inválido.");
    }

    string[] nombresMeses =
    [
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre",
    ];
    var nombreMes = nombresMeses[month - 1];

    // Mismo criterio que en Facturación: límites del mes en hora local (Argentina),
    // convertidos a UTC para comparar contra Venta.Fecha.
    var inicioLocal = new DateTime(year, month, 1);
    var finLocal = inicioLocal.AddMonths(1);
    var inicioUtc = DateTime.SpecifyKind(inicioLocal, DateTimeKind.Local).ToUniversalTime();
    var finUtc = DateTime.SpecifyKind(finLocal, DateTimeKind.Local).ToUniversalTime();

    await using var db = await dbFactory.CreateDbContextAsync();
    var ventas = await db.Ventas
        .Include(v => v.Presupuesto).ThenInclude(p => p.Cliente)
        .Where(v => v.Fecha >= inicioUtc && v.Fecha < finUtc)
        .OrderBy(v => v.Fecha)
        .ToListAsync();

    if (formato == "excel")
    {
        var excelBytes = FacturacionExcelBuilder.Build(ventas, year, nombreMes);
        return Results.File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Facturacion-{year:D4}-{month:D2}.xlsx");
    }

    if (formato == "pdf")
    {
        var pdfBytes = new FacturacionPdfDocument(ventas, year, nombreMes).GeneratePdf();
        return Results.File(pdfBytes, "application/pdf", $"Facturacion-{year:D4}-{month:D2}.pdf");
    }

    return Results.BadRequest("Formato inválido.");
}).RequireAuthorization();

app.Run();

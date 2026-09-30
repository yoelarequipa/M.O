using System.Reflection;
using MarmolesOeste.Web.Data.Configurations;
using MarmolesOeste.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarmolesOeste.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Material> Materiales => Set<Material>();
    public DbSet<TerminacionCanto> TerminacionesCanto => Set<TerminacionCanto>();
    public DbSet<Adicional> Adicionales => Set<Adicional>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Presupuesto> Presupuestos => Set<Presupuesto>();
    public DbSet<PresupuestoPieza> PresupuestoPiezas => Set<PresupuestoPieza>();
    public DbSet<PresupuestoPiezaTramo> PresupuestoPiezaTramos => Set<PresupuestoPiezaTramo>();
    public DbSet<PresupuestoPiezaCalado> PresupuestoPiezaCalados => Set<PresupuestoPiezaCalado>();
    public DbSet<PresupuestoAdicional> PresupuestoAdicionales => Set<PresupuestoAdicional>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<OrdenProduccion> OrdenesProduccion => Set<OrdenProduccion>();
    public DbSet<Entrega> Entregas => Set<Entrega>();
    public DbSet<MovimientoCaja> MovimientosCaja => Set<MovimientoCaja>();
    public DbSet<LlegadaStock> LlegadasStock => Set<LlegadaStock>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Tarea> Tareas => Set<Tarea>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>(PresupuestoConfiguration.NumeroCorrelativoSequence).StartsAt(1);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    // Aplica el filtro "FechaBaja == null" a toda entidad que implemente ISoftDelete,
    // para no repetirlo a mano en cada configuración y no olvidarlo en una nueva.
    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var method = typeof(ApplicationDbContext)
                .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);
            method.Invoke(null, [modelBuilder]);
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDelete
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.FechaBaja == null);
    }
}

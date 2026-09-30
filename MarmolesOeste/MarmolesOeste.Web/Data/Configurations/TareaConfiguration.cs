using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> builder)
    {
        builder.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Descripcion).HasMaxLength(500);

        builder
            .HasOne(t => t.Empleado)
            .WithMany(e => e.Tareas)
            .HasForeignKey(t => t.EmpleadoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(t => t.Venta)
            .WithMany()
            .HasForeignKey(t => t.VentaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

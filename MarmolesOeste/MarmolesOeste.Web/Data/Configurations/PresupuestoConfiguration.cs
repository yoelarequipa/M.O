using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class PresupuestoConfiguration : IEntityTypeConfiguration<Presupuesto>
{
    public const string NumeroCorrelativoSequence = "presupuesto_numero_seq";

    public void Configure(EntityTypeBuilder<Presupuesto> builder)
    {
        builder.Property(p => p.Subtotal).HasPrecision(18, 2);
        builder.Property(p => p.DescuentoPorcentaje).HasPrecision(5, 2);
        builder.Property(p => p.Descuento).HasPrecision(18, 2);
        builder.Property(p => p.IvaPorcentaje).HasPrecision(5, 2);
        builder.Property(p => p.Iva).HasPrecision(18, 2);
        builder.Property(p => p.Total).HasPrecision(18, 2);
        builder.Property(p => p.NombreReferencia).HasMaxLength(150);

        // Número atómico generado por Postgres (nextval), no por MAX()+1 en C#:
        // así dos presupuestos creados al mismo tiempo nunca chocan de número.
        builder.Property(p => p.NumeroCorrelativo)
            .HasDefaultValueSql($"nextval('{NumeroCorrelativoSequence}')");
        builder.HasIndex(p => p.NumeroCorrelativo).IsUnique();

        builder
            .HasOne(p => p.Cliente)
            .WithMany(c => c.Presupuestos)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

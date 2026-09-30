using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class MovimientoCajaConfiguration : IEntityTypeConfiguration<MovimientoCaja>
{
    public void Configure(EntityTypeBuilder<MovimientoCaja> builder)
    {
        builder.Property(m => m.Concepto).IsRequired().HasMaxLength(300);
        builder.Property(m => m.Monto).HasPrecision(18, 2);

        builder
            .HasOne(m => m.Venta)
            .WithMany()
            .HasForeignKey(m => m.VentaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

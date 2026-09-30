using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class EntregaConfiguration : IEntityTypeConfiguration<Entrega>
{
    public void Configure(EntityTypeBuilder<Entrega> builder)
    {
        builder.Property(e => e.Direccion).IsRequired().HasMaxLength(300);

        builder
            .HasOne(e => e.Venta)
            .WithMany(v => v.Entregas)
            .HasForeignKey(e => e.VentaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

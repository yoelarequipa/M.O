using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class OrdenProduccionConfiguration : IEntityTypeConfiguration<OrdenProduccion>
{
    public void Configure(EntityTypeBuilder<OrdenProduccion> builder)
    {
        builder
            .HasOne(o => o.Venta)
            .WithOne(v => v.OrdenProduccion)
            .HasForeignKey<OrdenProduccion>(o => o.VentaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

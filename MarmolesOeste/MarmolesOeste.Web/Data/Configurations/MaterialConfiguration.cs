using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(m => m.Espesor).HasPrecision(18, 2);
        builder.Property(m => m.PrecioPorM2).HasPrecision(18, 2);
        builder.Property(m => m.PrecioPorMetroLinealZocalo).HasPrecision(18, 2);
        builder.Property(m => m.ImagenUrl).HasMaxLength(500);
        builder.Property(m => m.ColorHex).IsRequired().HasMaxLength(7);
        builder.Property(m => m.MotivoUltimaActualizacion).HasMaxLength(300);
        builder.Property(m => m.AlertaStockCantidad).HasPrecision(18, 2);

        builder.Ignore(m => m.Activo);
    }
}

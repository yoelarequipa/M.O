using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class AdicionalConfiguration : IEntityTypeConfiguration<Adicional>
{
    public void Configure(EntityTypeBuilder<Adicional> builder)
    {
        builder.Property(a => a.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(a => a.Precio).HasPrecision(18, 2);
        builder.Property(a => a.MotivoUltimaActualizacion).HasMaxLength(300);

        builder.Ignore(a => a.Activo);
    }
}

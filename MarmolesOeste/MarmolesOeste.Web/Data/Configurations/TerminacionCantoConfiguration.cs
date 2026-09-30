using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class TerminacionCantoConfiguration : IEntityTypeConfiguration<TerminacionCanto>
{
    public void Configure(EntityTypeBuilder<TerminacionCanto> builder)
    {
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(t => t.PrecioPorMetroLineal).HasPrecision(18, 2);
        builder.Property(t => t.MotivoUltimaActualizacion).HasMaxLength(300);

        builder.Ignore(t => t.Activo);
    }
}

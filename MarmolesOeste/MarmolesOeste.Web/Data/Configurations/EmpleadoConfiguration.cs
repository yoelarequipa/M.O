using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> builder)
    {
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Telefono).HasMaxLength(50);
        builder.Property(e => e.Email).HasMaxLength(150);
        builder.Property(e => e.Puesto).HasMaxLength(100);

        builder.Ignore(e => e.Activo);
    }
}

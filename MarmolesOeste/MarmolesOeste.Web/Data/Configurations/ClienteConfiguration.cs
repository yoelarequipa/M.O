using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Telefono).HasMaxLength(50);
        builder.Property(c => c.Email).HasMaxLength(150);
        builder.Property(c => c.DireccionObra).HasMaxLength(300);

        builder.Ignore(c => c.Activo);
    }
}

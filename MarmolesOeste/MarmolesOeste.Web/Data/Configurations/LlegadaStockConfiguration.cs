using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class LlegadaStockConfiguration : IEntityTypeConfiguration<LlegadaStock>
{
    public void Configure(EntityTypeBuilder<LlegadaStock> builder)
    {
        builder.Property(l => l.Descripcion).IsRequired().HasMaxLength(300);
    }
}

using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Data.Configurations;

public class FlowerTypeConfiguration : IEntityTypeConfiguration<FlowerType>
{
    public void Configure(EntityTypeBuilder<FlowerType> builder)
    {
        builder.ToTable(flowerType => flowerType.HasCheckConstraint($"CK_{nameof(FlowerType)}s_Stock_Positive", "Stock >= 0"));

        builder.HasIndex(ft => ft.Name);
        builder.HasIndex(ft => ft.Color);
        
        builder.HasIndex(ft => new { ft.Name, ft.Color })
            .IsUnique()
            .HasDatabaseName($"UX_{nameof(FlowerType)}s_{nameof(FlowerType.Name)}_{nameof(FlowerType.Color)}");
        
        builder.Property(ft => ft.Stock)
            .HasDefaultValue(0);
    }
}
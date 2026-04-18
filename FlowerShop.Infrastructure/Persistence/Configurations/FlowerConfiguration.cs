using FlowerShop.Domain.Entities.FlowerTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class FlowerConfiguration : IEntityTypeConfiguration<Flower>
{
    public void Configure(EntityTypeBuilder<Flower> builder)
    {
        builder.ToTable(flowerType => flowerType.HasCheckConstraint($"CK_{nameof(Flower)}s_Stock_Positive", "Stock >= 0"));

        builder.HasIndex(ft => ft.Name);
        builder.HasIndex(ft => ft.Color);
        
        builder.HasIndex(ft => new { ft.Name, ft.Color })
            .IsUnique()
            .HasDatabaseName($"UX_{nameof(Flower)}s_{nameof(Flower.Name)}_{nameof(Flower.Color)}");
        
        builder.Property(ft => ft.Stock)
            .HasDefaultValue(0);
    }
}
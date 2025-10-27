using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Data.Configurations;

public class ProductFlowerConfiguration : IEntityTypeConfiguration<ProductFlower>
{
    public void Configure(EntityTypeBuilder<ProductFlower> builder)
    {
        builder.ToTable(pf => pf.HasCheckConstraint($"CK_{nameof(ProductFlower)}s_{nameof(ProductFlower.Quantity)}_Positive", "Quantity >= 0"));
        
        builder.HasKey(pf => new { pf.ProductId, pf.FlowerId });
        
        builder.HasOne(pf => pf.Product)
            .WithMany(p => p.ProductFlowers)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(pf => pf.FlowerType)
            .WithMany(f => f.ProductFlowers)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(pf => pf.Quantity)
            .HasDefaultValue(1);
        
    }
}
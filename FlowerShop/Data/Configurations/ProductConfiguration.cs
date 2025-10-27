using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(products => products.HasCheckConstraint($"CK_{nameof(Product)}s_{nameof(Product.Price)}_Positive", "Price > 0"));
        builder.ToTable(products => products.HasCheckConstraint($"CK_{nameof(Product)}s_{nameof(Product.Stock)}_Positive", "Stock >= 0"));
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName($"UX_{nameof(Product)}s_{nameof(Product.Name)}_{nameof(Product.CategoryId)}");
        
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => new { p.CategoryId, p.Price });

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(p => p.Stock)
            .HasDefaultValue(0);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.PromoPrice)
            .HasPrecision(18, 2);

    }
}
using FlowerShop.Domain.Entities.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.ToTable(items => items.HasCheckConstraint(
            $"CK_{nameof(CartItem)}s_{nameof(CartItem.Quantity)}_Positive",
            "Quantity >= 1"));

        builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        builder.Property(ci => ci.Price)
            .HasPrecision(18, 2);

        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
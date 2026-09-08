using FlowerShop.Domain.Entities.ProductReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.HasKey(pr => pr.Id);
        
        builder.Property(pr => pr.Rating)
            .HasPrecision(3, 2)
            .IsRequired();
        
        builder.Property(pr => pr.Comment)
            .HasMaxLength(1000);

        builder.Property(pr => pr.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.HasOne(pr => pr.Product)
            .WithMany(p => p.ProductReviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(pr => pr.User)
            .WithMany(u => u.ProductReviews)
            .HasForeignKey(pr => pr.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pr => new { pr.ProductId, pr.ReviewerId })
            .IsUnique();
    }
}
using FlowerShop.Domain.Entities.ServiceReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class ServiceReviewConfiguration : IEntityTypeConfiguration<ServiceReview>
{
    public void Configure(EntityTypeBuilder<ServiceReview> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired()
            .HasPrecision(3, 2);

        builder.Property(r => r.Comment)
            .HasMaxLength(500);

        builder.HasOne(r => r.Reviewer)
            .WithMany(u => u.ServiceReviews)
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Order)
            .WithOne(o => o.ServiceReview)
            .HasForeignKey<ServiceReview>(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(r => new {r.OrderId, r.ReviewerId})
            .IsUnique();
        
    }
}
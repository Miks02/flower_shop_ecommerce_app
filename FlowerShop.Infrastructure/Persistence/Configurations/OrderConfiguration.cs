using FlowerShop.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.RecipientFullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(o => o.RecipientPhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(o => o.OrderAddress)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(o => o.Note)
            .HasMaxLength(500);

        builder.Property(o => o.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.OrderStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.DeliveryStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Ignore(o => o.OrderPrice);

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(o => o.Deliverer)
            .WithMany(d => d.Orders)
            .HasForeignKey(o => o.DelivererId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.UserId);

        builder.HasIndex(o => o.CreatedAt);

        builder.HasIndex(o => o.OrderStatus);
    }
}
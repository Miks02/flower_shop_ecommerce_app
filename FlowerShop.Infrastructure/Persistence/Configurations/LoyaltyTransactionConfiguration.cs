using FlowerShop.Domain.Entities.LoyaltyTransactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.HasKey(lt => lt.Id);
        
        builder.HasOne(lt => lt.User)
            .WithMany(u => u.LoyaltyTransactions)
            .HasForeignKey(lt => lt.UserId)
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder.HasOne(lt => lt.Order)
            .WithMany(o => o.LoyaltyTransactions)
            .HasForeignKey(lt => lt.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(lt => lt.TransactionType)
            .HasConversion<string>();
        
        builder.ToTable(lt => lt.HasCheckConstraint($"CK_{nameof(LoyaltyTransaction)}s_{nameof(LoyaltyTransaction.CurrentPoints)}_NonNegative", "CurrentPoints >= 0"));
        
    }
}
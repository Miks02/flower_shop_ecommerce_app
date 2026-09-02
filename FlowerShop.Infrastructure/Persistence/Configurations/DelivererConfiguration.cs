using FlowerShop.Domain.Entities.Deliverers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class DelivererConfiguration : IEntityTypeConfiguration<Deliverer>
{
    public void Configure(EntityTypeBuilder<Deliverer> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.HasOne(x => x.User)
            .WithOne(x => x.Deliverer)
            .HasForeignKey<Deliverer>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(t => t.HasCheckConstraint("CK_Deliverers_MaxAmountOfOrders_Positive", "\"MaxAmountOfOrders\" >= 0"));
    }
}
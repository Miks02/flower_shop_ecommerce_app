using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Data.Configurations;

public class OccasionConfiguration : IEntityTypeConfiguration<Occasion>
{
    public void Configure(EntityTypeBuilder<Occasion> builder)
    {
        builder.HasIndex(oc => oc.Name)
            .IsUnique()
            .HasDatabaseName($"UX_{nameof(Occasion)}s_{nameof(Occasion.Name)}");

        builder.HasData(
            new Occasion {Id = 1, Name = "Dan zaljubljenih"},
            new Occasion {Id = 2, Name = "8. Mart - Dan žena"},
            new Occasion {Id = 3, Name = "Rodjendan"},
            new Occasion {Id = 4, Name = "Svadba i venčanje"}
        );
        
    }
}
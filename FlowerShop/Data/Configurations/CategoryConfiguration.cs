using FlowerShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName($"UX_{nameof(Category)}s_{nameof(Category.Name)}");

        builder.HasData(
            new Category {Id = 1, Name = "Buketi"},
            new Category {Id = 2, Name = "Aranžmani"},
            new Category {Id = 3, Name = "Specijalni pokloni"},
            new Category {Id = 4, Name = "101 Ruža"},
            new Category {Id = 5, Name = "Saksijsko cveće"},
            new Category {Id = 6, Name = "Box mede"},
            new Category {Id = 7, Name = "Dehidrirane ruže"},
            new Category {Id = 8, Name = "Venci i suze"}
        );
    }
}
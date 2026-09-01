using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowerShop.Infrastructure.Persistence.Configurations;

public class FlowerConfiguration : IEntityTypeConfiguration<Flower>
{
    public void Configure(EntityTypeBuilder<Flower> builder)
    {
        builder.ToTable(flowerType => flowerType.HasCheckConstraint($"CK_{nameof(Flower)}s_Stock_Positive", "Stock >= 0"));

        builder.HasIndex(ft => ft.Name);
        builder.HasIndex(ft => ft.Color);

        builder.HasIndex(ft => new { ft.Name, ft.FlowerCategory, ft.Color })
            .IsUnique();
        
        builder.HasIndex(ft => new { ft.Name, ft.Color })
            .IsUnique()
            .HasDatabaseName($"UX_{nameof(Flower)}s_{nameof(Flower.Name)}_{nameof(Flower.Color)}");
        
        builder.Property(ft => ft.Stock)
            .HasDefaultValue(0);
        
        builder.HasData(
            new Flower { Id = 1, Name = "Ruža", Color = "Crvena", FlowerCategory = FlowerCategory.Fresh, Stock = 100 },
            new Flower { Id = 2, Name = "Lala", Color = "Žuta", FlowerCategory = FlowerCategory.Dehydrated, Stock = 150 },
            new Flower { Id = 3, Name = "Ljiljan", Color = "Bela", FlowerCategory = FlowerCategory.Artificial, Stock = 80 },
            new Flower { Id = 4, Name = "Bela rada", Color = "Roza", FlowerCategory = FlowerCategory.Fresh, Stock = 120 },
            new Flower { Id = 5, Name = "Karanfil", Color = "Crvena", FlowerCategory = FlowerCategory.Fresh, Stock = 200 },
            new Flower { Id = 6, Name = "Gerber", Color = "Narandžasta", FlowerCategory = FlowerCategory.Fresh, Stock = 90 },
            new Flower { Id = 7, Name = "Orhideja", Color = "Ljubičasta", FlowerCategory = FlowerCategory.Fresh, Stock = 50 },
            new Flower { Id = 8, Name = "Iris", Color = "Plava", FlowerCategory = FlowerCategory.Fresh, Stock = 70 },
            new Flower { Id = 9, Name = "Narcis", Color = "Žuta", FlowerCategory = FlowerCategory.Fresh, Stock = 110 },
            new Flower { Id = 10, Name = "Suncokret", Color = "Žuta", FlowerCategory = FlowerCategory.Fresh, Stock = 60 },
            new Flower { Id = 11, Name = "Jorgovan", Color = "Ljubičasta", FlowerCategory = FlowerCategory.Fresh, Stock = 40 },
            new Flower { Id = 12, Name = "Magnolija", Color = "Bela", FlowerCategory = FlowerCategory.Fresh, Stock = 30 },
            new Flower { Id = 13, Name = "Hrizantema", Color = "Žuta", FlowerCategory = FlowerCategory.Fresh, Stock = 100 },
            new Flower { Id = 14, Name = "Božur", Color = "Roza", FlowerCategory = FlowerCategory.Fresh, Stock = 45 },
            new Flower { Id = 15, Name = "Jasmin", Color = "Bela", FlowerCategory = FlowerCategory.Fresh, Stock = 85 },
            new Flower { Id = 16, Name = "Lavanda", Color = "Ljubičasta", FlowerCategory = FlowerCategory.Dehydrated, Stock = 300 },
            new Flower { Id = 17, Name = "Mimoza", Color = "Žuta", FlowerCategory = FlowerCategory.Fresh, Stock = 55 },
            new Flower { Id = 18, Name = "Ruža", Color = "White", FlowerCategory = FlowerCategory.Dehydrated, Stock = 40 }
        );

    }
}
using FlowerShop.Application.Features.Products.Commands.AddProduct;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class FlowerRepository : Repository<Flower>, IFlowerRepository
{
    private readonly AppDbContext _context;
    
    public FlowerRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FlowerDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Flowers
            .Select(f => new FlowerDto
            {
                Id = f.Id,
                Name = f.Name,
                Stock = f.Stock,
                Color = f.Color
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Flower>> GetFlowersByIdsAsync(IReadOnlyList<int> flowerIds, CancellationToken ct = default)
    {
        return await _context.Flowers
            .Where(f => flowerIds.Contains(f.Id))
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<int>> GetInvalidFlowerIdsAsync(IReadOnlyList<int> flowerIds, CancellationToken ct = default)
    {
        if (!flowerIds.Any())
            return [];
        
        var validIds = await _context.Flowers
            .Where(f => flowerIds.Contains(f.Id))
            .Select(f => f.Id)
            .ToListAsync(ct);
        
        return flowerIds.Except(validIds).ToList();
    }
    
}
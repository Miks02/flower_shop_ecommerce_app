using FlowerShop.Application.Features.Products.Commands.AddProduct;
using FlowerShop.Domain.Entities.Flowers;
using FlowerShop.Domain.Enums;
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

    public async Task<Flower?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Flowers.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(string name, string color, FlowerCategory flowerCategory, CancellationToken ct = default)
    {
        return await _context.Flowers.AnyAsync(f =>
            f.Name == name && f.Color == color && f.FlowerCategory == flowerCategory, ct);
    }

    public async Task<bool> IsUsedInProductsAsync(int id, CancellationToken ct = default)
    {
        return await _context.ProductFlowers.AnyAsync(pf => pf.FlowerId == id, ct);
    }

}
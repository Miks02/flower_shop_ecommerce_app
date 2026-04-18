using FlowerShop.Domain.Entities.Ocassions;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class OccasionRepository : Repository<Occasion>, IOccasionRepository
{
    private readonly AppDbContext _context;
    
    public OccasionRepository(AppDbContext context) : base(context)
    {
        _context = context;   
    }
    
    public async Task<IReadOnlyList<int>> GetInvalidOccasionIdsAsync(IReadOnlyList<int> occasionIds, CancellationToken ct = default)
    {
        var validIs = await _context.Occasions
            .Where(o => occasionIds.Contains(o.Id))
            .Select(o => o.Id)
            .ToListAsync(ct);
        
        return occasionIds.Except(validIs).ToList();
    }
}
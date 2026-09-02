using FlowerShop.Domain.Entities.Deliverers;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using FlowerShop.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class DelivererRepository : Repository<Deliverer>, IDelivererRepository
{
    private readonly AppDbContext _context;

    public DelivererRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<DelivererDto>> GetPagedDeliverersAsync(
        string? search,
        string? sortBy,
        VehicleType? vehicleType,
        DelivererStatus? delivererStatus,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _context.Deliverers
            .Include(d => d.User)
            .AsQueryable();

        if (vehicleType is not null)
            query = query.Where(d => d.VehicleType == vehicleType);

        if (delivererStatus is not null)
            query = query.Where(d => d.DelivererStatus == delivererStatus);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d =>
                d.User.FirstName.Contains(search) ||
                d.User.LastName.Contains(search) ||
                d.User.Email!.Contains(search) ||
                d.User.PhoneNumber!.Contains(search));

        query = sortBy switch
        {
            "name_asc" => query.OrderBy(d => d.User.FirstName).ThenBy(d => d.User.LastName),
            "name_desc" => query.OrderByDescending(d => d.User.FirstName).ThenByDescending(d => d.User.LastName),
            "status_asc" => query.OrderBy(d => d.DelivererStatus),
            "status_desc" => query.OrderByDescending(d => d.DelivererStatus),
            "vehicle_asc" => query.OrderBy(d => d.VehicleType),
            "vehicle_desc" => query.OrderByDescending(d => d.VehicleType),
            _ => query.OrderBy(d => d.User.FirstName).ThenBy(d => d.User.LastName)
        };

        var totalCount = await query.CountAsync(ct);

        var delivererList = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DelivererDto
            {
                Id = d.Id,
                FirstName = d.User.FirstName,
                LastName = d.User.LastName,
                Email = d.User.Email!,
                PhoneNumber = d.User.PhoneNumber!,
                VehicleType = d.VehicleType,
                DelivererStatus = d.DelivererStatus,
                CreatedAt = d.User.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<DelivererDto>(delivererList, page, pageSize, totalCount, delivererList.Count);
    }

    public async Task<DelivererStatisticsDto> GetStatisticsAsync(CancellationToken ct = default)
    {
        var items = await _context.Deliverers
            .Select(d => new { d.DelivererStatus, d.VehicleType })
            .ToListAsync(ct);

        var total = items.Count;
        var available = items.Count(d => d.DelivererStatus == DelivererStatus.Available);
        var onDuty = items.Count(d => d.DelivererStatus == DelivererStatus.OnDuty);
        var unavailable = items.Count(d => d.DelivererStatus == DelivererStatus.Unavailable);
        var bicycle = items.Count(d => d.VehicleType == VehicleType.Bicycle);
        var scooter = items.Count(d => d.VehicleType == VehicleType.Scooter);
        var car = items.Count(d => d.VehicleType == VehicleType.Car);

        return new DelivererStatisticsDto(total, available, onDuty, unavailable, bicycle, scooter, car);
    }

    public async Task<Deliverer?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await _context.Deliverers
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
    {
        return await _context.Deliverers.AnyAsync(d => d.Id == id, ct);
    }
}
using FlowerShop.SharedKernel.Results;

namespace FlowerShop.Domain.Entities.Deliverers;

public interface IDelivererRepository
{
    Task<PagedResult<DelivererDto>> GetPagedDeliverersAsync(
        string? search,
        string? sortBy,
        VehicleType? vehicleType,
        DelivererStatus? delivererStatus,
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task<DelivererStatisticsDto> GetStatisticsAsync(CancellationToken ct = default);
    Task<Deliverer?> GetByIdAsync(string id, CancellationToken ct = default);
    void Add(Deliverer deliverer);
    void Update(Deliverer deliverer);
    void Remove(Deliverer deliverer);
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);
}
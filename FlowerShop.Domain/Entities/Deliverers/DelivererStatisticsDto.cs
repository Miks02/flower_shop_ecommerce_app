namespace FlowerShop.Domain.Entities.Deliverers;

public record DelivererStatisticsDto(
    int TotalCount,
    int AvailableCount,
    int OnDutyCount,
    int UnavailableCount,
    int BicycleCount,
    int ScooterCount,
    int CarCount);

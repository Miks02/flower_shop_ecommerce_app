using FlowerShop.Domain.Entities.Deliverers;

namespace FlowerShop.Application.Features.Deliverers.Commands.UpdateDelivererStatus;

public record UpdateDelivererStatusCommand(string Id, DelivererStatus Status);

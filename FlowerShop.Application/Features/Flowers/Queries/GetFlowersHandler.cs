using FlowerShop.Application.Common.Abstractions;
using FlowerShop.Domain.Entities.Flowers;

namespace FlowerShop.Application.Features.Flowers.Queries;

public class GetFlowersHandler(IFlowerRepository flowerRepo) : IHandler
{
    public async Task<GetFlowersResponse> Handle(CancellationToken ct = default)
    {
        return new GetFlowersResponse
        {
            Flowers = await flowerRepo.GetAllAsync(ct)
        };
    }
}
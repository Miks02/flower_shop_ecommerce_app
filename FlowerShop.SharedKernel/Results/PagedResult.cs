namespace FlowerShop.SharedKernel.Results;

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize,  int TotalCount, int PaginatedCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
};
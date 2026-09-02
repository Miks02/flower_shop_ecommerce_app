using FlowerShop.Domain.Entities.ProductFlowers;
using FlowerShop.Domain.Entities.Products;
using FlowerShop.Infrastructure.Persistence.EntityFramework;
using FlowerShop.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetPagedProductsAsync(
        string? search,
        string? sortBy,
        int? categoryId,
        bool isDeleted,
        int page, 
        int pageSize, 
        IReadOnlyList<int> occasionIds,
        CancellationToken ct = default)
    {
        var query = _context.Products
            .IgnoreQueryFilters();
        
        if(categoryId is not null)
            query = query.Where(p => p.CategoryId == categoryId);
        
        if(isDeleted)
            query = query.Where(p => p.IsDeleted);
        
        query = sortBy switch
        {
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "stock_asc" => query.OrderBy(p => p.Stock),
            "stock_desc" => query.OrderByDescending(p => p.Stock),
            _ => query.OrderBy(p => p.Id)
        };
        
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));
        
        if(occasionIds.Any())
            query = query.Where(p => p.Occasions.Any(o => occasionIds.Contains(o.Id)));
        
        var productList = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PromoPrice = p.PromoPrice,
                DiscountType = p.DiscountType,
                Stock = p.Stock,
                Description = p.Description,
                CategoryName = p.Category.Name,
                Occasions = p.Occasions.Select(o => o.Name).ToList(),
                ProductFlowers = p.ProductFlowers.Select(pf => new ProductFlowerDto
                {
                    ProductId = pf.ProductId,
                    ProductName = p.Name,
                    FlowerId = pf.FlowerId,
                    FlowerName = pf.Flower.Name,
                    Quantity = pf.Quantity
                }).ToList(),
                ProductImage = p.ImageUrl,
                IsDeleted = p.IsDeleted
            })
            .ToListAsync(ct);
        
        var totalCount = await query.CountAsync(ct);

        return new PagedResult<ProductDto>(productList, page, pageSize, totalCount, productList.Count);

    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products
            .AsSplitQuery()
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Occasions)
            .Include(p => p.ProductFlowers)
                .ThenInclude(pf => pf.Flower)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.Id == id, ct);   
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.Name == name, ct);
    }
}
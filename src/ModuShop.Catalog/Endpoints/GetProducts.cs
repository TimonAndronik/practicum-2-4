using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Catalog.Data;

namespace ModuShop.Catalog.Endpoints;

public class ProductListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class GetProducts(CatalogDbContext dbContext)
    : EndpointWithoutRequest<List<ProductListItem>>
{
    public override void Configure()
    {
        Get("/api/catalog/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var products = await dbContext.Products
            .Include(x => x.Category)
            .Select(x => new ProductListItem
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                CategoryId = x.CategoryId,
                CategoryName = x.Category != null ? x.Category.Name : null
            })
            .ToListAsync(ct);

        await Send.OkAsync(products, ct);
    }
}
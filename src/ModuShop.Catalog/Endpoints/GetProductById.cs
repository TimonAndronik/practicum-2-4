using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Catalog.Data;

namespace ModuShop.Catalog.Endpoints;

public class GetProductByIdRequest
{
    public int Id { get; set; }
}

public class GetProductByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class GetProductById(CatalogDbContext dbContext)
    : Endpoint<GetProductByIdRequest, GetProductByIdResponse>
{
    public override void Configure()
    {
        Get("/api/catalog/products/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetProductByIdRequest req, CancellationToken ct)
    {
        var product = await dbContext.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

        if (product is null)
        {
            AddError("Product not found");
            ThrowIfAnyErrors();
            return;
        }

        Response = new GetProductByIdResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        };

        await Send.OkAsync(Response, ct);
    }
}
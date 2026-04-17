using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ModuShop.Catalog.Data;
using ModuShop.Catalog.Entities;

namespace ModuShop.Catalog.Endpoints;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0.01, 999999)]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
}

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}

public class CreateProduct(CatalogDbContext dbContext)
    : Endpoint<CreateProductRequest, CreateProductResponse>
{
    public override void Configure()
    {
        Post("/api/catalog/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var categoryExists = await dbContext.Categories.FindAsync(new object[] { req.CategoryId }, ct);

        if (categoryExists is null)
        {
            AddError("Category not found");
            ThrowIfAnyErrors();
            return;
        }

        var product = new Product
        {
            Name = req.Name,
            Description = req.Description,
            Price = req.Price,
            CategoryId = req.CategoryId
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);

        Response = new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId
        };

        await Send.OkAsync(Response, ct);
    }
}
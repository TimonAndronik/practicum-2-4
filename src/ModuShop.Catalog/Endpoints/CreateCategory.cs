using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using ModuShop.Catalog.Data;
using ModuShop.Catalog.Entities;

namespace ModuShop.Catalog.Endpoints;

public class CreateCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}

public class CreateCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateCategory(CatalogDbContext dbContext)
    : Endpoint<CreateCategoryRequest, CreateCategoryResponse>
{
    public override void Configure()
    {
        Post("/api/catalog/categories");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateCategoryRequest req, CancellationToken ct)
    {
        var category = new Category
        {
            Name = req.Name
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(ct);

        Response = new CreateCategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };

        await Send.OkAsync(Response, ct);
    }
}
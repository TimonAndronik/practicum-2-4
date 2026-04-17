using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModuShop.Catalog.Data;
using Serilog;

namespace ModuShop.Catalog;

public static class CatalogModuleExtensions
{
    public static IHostApplicationBuilder AddCatalogModuleServices(
        this IHostApplicationBuilder builder,
        ILogger logger)
    {
        builder.AddSqlServerDbContext<CatalogDbContext>("catalogdb");

        logger.Information("{Module} module services registered", "Catalog");

        return builder;
    }

    public static async Task EnsureCatalogModuleDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
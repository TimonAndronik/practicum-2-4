using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModuShop.Orders.Data;
using Serilog;

namespace ModuShop.Orders;

public static class OrdersModuleExtensions
{
    public static IHostApplicationBuilder AddOrdersModuleServices(
        this IHostApplicationBuilder builder,
        ILogger logger)
    {
        builder.AddSqlServerDbContext<OrdersDbContext>("ordersdb");

        logger.Information("{Module} module services registered", "Orders");

        return builder;
    }

    public static async Task EnsureOrdersModuleDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Orders.Data;

namespace ModuShop.Reporting.Endpoints;

public class TopProductItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class GetTopProducts(OrdersDbContext ordersDb)
    : EndpointWithoutRequest<List<TopProductItem>>
{
    public override void Configure()
    {
        Get("/api/reports/top-products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var topProducts = await ordersDb.OrderItems
            .GroupBy(x => new { x.ProductId, x.ProductName })
            .Select(g => new TopProductItem
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                TotalQuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.LineTotal)
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(10)
            .ToListAsync(ct);

        await Send.OkAsync(topProducts, ct);
    }
}
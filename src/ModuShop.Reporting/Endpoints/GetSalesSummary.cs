using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Orders.Data;

namespace ModuShop.Reporting.Endpoints;

public class SalesSummaryResponse
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class GetSalesSummary(OrdersDbContext ordersDb)
    : EndpointWithoutRequest<SalesSummaryResponse>
{
    public override void Configure()
    {
        Get("/api/reports/sales-summary");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var totalOrders = await ordersDb.Orders.CountAsync(ct);
        var totalRevenue = await ordersDb.Orders.SumAsync(x => (decimal?)x.TotalAmount, ct) ?? 0m;
        var averageOrderValue = totalOrders == 0 ? 0m : totalRevenue / totalOrders;

        await Send.OkAsync(new SalesSummaryResponse
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = averageOrderValue
        }, ct);
    }
}
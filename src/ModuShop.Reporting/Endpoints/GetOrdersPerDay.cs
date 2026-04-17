using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Orders.Data;

namespace ModuShop.Reporting.Endpoints;

public class OrdersPerDayItem
{
    public DateTime Date { get; set; }
    public int OrdersCount { get; set; }
    public decimal Revenue { get; set; }
}

public class GetOrdersPerDay(OrdersDbContext ordersDb)
    : EndpointWithoutRequest<List<OrdersPerDayItem>>
{
    public override void Configure()
    {
        Get("/api/reports/orders-per-day");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var orders = await ordersDb.Orders
            .AsNoTracking()
            .ToListAsync(ct);

        var result = orders
            .GroupBy(x => x.CreatedAtUtc.Date)
            .Select(g => new OrdersPerDayItem
            {
                Date = g.Key,
                OrdersCount = g.Count(),
                Revenue = g.Sum(x => x.TotalAmount)
            })
            .OrderBy(x => x.Date)
            .ToList();

        await Send.OkAsync(result, ct);
    }
}
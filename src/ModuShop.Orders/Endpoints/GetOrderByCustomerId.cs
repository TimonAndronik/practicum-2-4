using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Orders.Data;

namespace ModuShop.Orders.Endpoints;

public class GetOrdersByCustomerIdRequest
{
    public string CustomerId { get; set; } = string.Empty;
}

public class CustomerOrderListItem
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public decimal TotalAmount { get; set; }
    public int ItemsCount { get; set; }
}

public class GetOrdersByCustomerId(OrdersDbContext dbContext)
    : Endpoint<GetOrdersByCustomerIdRequest, List<CustomerOrderListItem>>
{
    public override void Configure()
    {
        Get("/api/orders/by-customer/{customerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetOrdersByCustomerIdRequest req, CancellationToken ct)
    {
        var orders = await dbContext.Orders
            .Include(x => x.Items)
            .Where(x => x.CustomerId == req.CustomerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new CustomerOrderListItem
            {
                Id = x.Id,
                CreatedAtUtc = x.CreatedAtUtc,
                TotalAmount = x.TotalAmount,
                ItemsCount = x.Items.Count
            })
            .ToListAsync(ct);

        await Send.OkAsync(orders, ct);
    }
}
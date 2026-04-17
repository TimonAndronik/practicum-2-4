using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ModuShop.Orders.Data;

namespace ModuShop.Orders.Endpoints;

public class GetOrderByIdRequest
{
    public int Id { get; set; }
}

public class GetOrderByIdResponse
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public decimal TotalAmount { get; set; }
    public List<GetOrderItemResponse> Items { get; set; } = new();
}

public class GetOrderItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class GetOrderById(OrdersDbContext dbContext)
    : Endpoint<GetOrderByIdRequest, GetOrderByIdResponse>
{
    public override void Configure()
    {
        Get("/api/orders/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetOrderByIdRequest req, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

        if (order is null)
        {
            AddError("Order not found");
            ThrowIfAnyErrors();
            return;
        }

        Response = new GetOrderByIdResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAtUtc = order.CreatedAtUtc,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(x => new GetOrderItemResponse
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                LineTotal = x.LineTotal
            }).ToList()
        };

        await Send.OkAsync(Response, ct);
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModuShop.Catalog.Data;
using ModuShop.Orders.Data;
using ModuShop.Orders.Entities;
using ModuShop.Emailing.Contracts;

namespace ModuShop.Orders.Endpoints;

public class CreateOrderRequest
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderResponse
{
    public int OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateOrder(
    OrdersDbContext ordersDb,
    CatalogDbContext catalogDb,
    UserManager<IdentityUser> userManager,
    IEmailSender emailSender)
    : Endpoint<CreateOrderRequest, CreateOrderResponse>
{
    public override void Configure()
    {
        Post("/api/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        if (req.Items.Count == 0)
        {
            AddError("Order must contain at least one item");
            ThrowIfAnyErrors();
            return;
        }

        var customer = await userManager.FindByIdAsync(req.CustomerId);
        if (customer is null)
        {
            AddError("Customer not found");
            ThrowIfAnyErrors();
            return;
        }

        var productIds = req.Items.Select(x => x.ProductId).Distinct().ToList();

        var products = await catalogDb.Products
            .Where(x => productIds.Contains(x.Id))
            .ToListAsync(ct);

        if (products.Count != productIds.Count)
        {
            AddError("One or more products were not found");
            ThrowIfAnyErrors();
            return;
        }

        var order = new ModuShop.Orders.Entities.Order
        {
            CustomerId = req.CustomerId,
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var item in req.Items)
        {
            if (item.Quantity <= 0)
            {
                AddError($"Invalid quantity for product {item.ProductId}");
                ThrowIfAnyErrors();
                return;
            }

            var product = products.First(x => x.Id == item.ProductId);

            var lineTotal = product.Price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                LineTotal = lineTotal
            });
        }

        order.TotalAmount = order.Items.Sum(x => x.LineTotal);

        ordersDb.Orders.Add(order);
        await ordersDb.SaveChangesAsync(ct);
        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            await emailSender.SendOrderConfirmationAsync(
                customer.Email,
                order.Id,
                order.TotalAmount,
                ct);
        }
        Response = new CreateOrderResponse
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAtUtc = order.CreatedAtUtc
        };

        await Send.OkAsync(Response, ct);
    }
}
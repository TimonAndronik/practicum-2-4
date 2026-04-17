using System;
using System.Collections.Generic;

namespace ModuShop.Orders.Entities;

public class Order
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public decimal TotalAmount { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
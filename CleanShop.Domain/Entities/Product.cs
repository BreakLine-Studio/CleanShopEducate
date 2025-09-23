using System;
using CleanShop.Domain.ValueObjects;

namespace CleanShop.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public Sku Sku { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }

    private Product() { } // EF
    public Product(string name, Sku sku, decimal price, int stock)
    { Name = name; Sku = sku; Price = price; Stock = stock; }
}

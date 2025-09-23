using System;

namespace CleanShop.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public Category() { }
    public Category(string name) { Name = name; }
}

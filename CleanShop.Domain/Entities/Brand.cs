using System;

namespace CleanShop.Domain.Entities;

public class Brand
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    public Brand() { }
    public Brand(string name, string description)
    {
        Name = name;
        Description = description;
    }

}

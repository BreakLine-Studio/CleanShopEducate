using System;

namespace CleanShop.Domain.ValueObjects;

public class Sku : IEquatable<Sku>
{
    protected Sku() { }
    public string Value { get; private set; } = null!;
    private Sku(string value) => Value = value.ToUpperInvariant().Trim();
    public static Sku Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("SKU vacío");
        return new Sku(value);
    }
    public bool Equals(Sku? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Sku o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}

using System;
using CleanShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanShop.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
               .IsRequired()
               .HasColumnType("varchar(50)");

        builder.Property(p => p.Price)
            .HasColumnType("numeric(18,2)");
        builder.Property(p => p.Stock)
            .IsRequired()
            .HasColumnType("integer");

        builder.OwnsOne(p => p.Sku, b =>
        {
            b.Property(x => x.Value).HasColumnName("sku").IsRequired().HasMaxLength(64);
        });
        builder.HasIndex(p => p.CreatedAt);

    }
}

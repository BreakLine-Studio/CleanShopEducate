using System;
using CleanShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanShop.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name)
               .IsRequired()
               .HasColumnType("varchar(50)");

        builder.HasMany(p => p.Products)
        .WithOne(c => c.Category)
        .HasForeignKey(p => p.CategoryId)
        .OnDelete(DeleteBehavior.SetNull); 

    }
}

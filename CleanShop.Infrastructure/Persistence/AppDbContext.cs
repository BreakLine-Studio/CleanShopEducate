using System;
using CleanShop.Domain.Entities;
using CleanShop.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace CleanShop.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<UserMember> UsersMembers => Set<UserMember>();
    public DbSet<Rol> Rols => Set<Rol>();
    // public DbSet<UserMemberRol> UsersMembersRols => Set<UserMemberRol>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}

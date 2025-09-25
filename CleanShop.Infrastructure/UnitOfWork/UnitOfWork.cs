using System;
using CleanShop.Application.Abstractions;
using CleanShop.Infrastructure.Persistence;
using CleanShop.Infrastructure.Persistence.Repositories;

namespace CleanShop.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IProductRepository? _productRepository;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await operation(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
    // public IProductRepository Products
    // {
    //     get
    //     {
    //         if (_productRepository == null)
    //         {
    //             _productRepository = new ProductRepository(_context);
    //         }
    //         return _productRepository;
    //     }
    // }
    public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
}

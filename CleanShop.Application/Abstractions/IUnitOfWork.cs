using System;

namespace CleanShop.Application.Abstractions;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    // Task<int> SaveAsync();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}

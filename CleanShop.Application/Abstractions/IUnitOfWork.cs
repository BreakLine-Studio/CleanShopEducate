using System;
using CleanShop.Application.Abstractions.Auth;

namespace CleanShop.Application.Abstractions;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IUserMemberService UserMembers { get; }
    IUserMemberRolService UserMemberRoles { get; }
    IRolService Roles { get; }
    // Task<int> SaveAsync();
    Task<int> SaveChanges(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}

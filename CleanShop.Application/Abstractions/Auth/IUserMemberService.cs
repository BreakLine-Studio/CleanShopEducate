using System;
using System.Linq.Expressions;
using CleanShop.Domain.Entities.Auth;

namespace CleanShop.Application.Abstractions.Auth;

public interface IUserMemberService
{
    Task<UserMember?> GetByIdAsync(int id, CancellationToken ct = default);
    IEnumerable<UserMember> Find(Expression<Func<UserMember, bool>> expression);
    Task<IEnumerable<UserMember>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<UserMember>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(UserMember entity, CancellationToken ct = default);
    Task UpdateAsync(UserMember entity, CancellationToken ct = default);
    Task RemoveAsync(UserMember entity, CancellationToken ct = default);
    Task<UserMember?> GetByUserNameAsync(string userName);

    Task<UserMember> GetByRefreshTokenAsync(string refreshToken);
}

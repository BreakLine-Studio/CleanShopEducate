using System;
using System.Linq;
using System.Linq.Expressions;
using CleanShop.Application.Abstractions.Auth;
using CleanShop.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace CleanShop.Infrastructure.Persistence.Repositories.Auth;

public class RolRepository(AppDbContext db) : IRolService
{
    public async Task AddAsync(Rol entity, CancellationToken ct = default)
    {
        db.Rols.Add(entity);
        // await db.SaveChangesAsync(ct);
        await Task.CompletedTask;
    }

    public Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = db.Rols.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToUpper();
            query = query.Where(p =>
                p.Name.ToUpper().Contains(term));
        }
        return query.CountAsync(ct);
    }

    public IEnumerable<Rol> Find(Expression<Func<Rol, bool>> expression)
    {
        return db.Set<Rol>().Where(expression);
    }

    public async Task<IEnumerable<Rol>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Rols.AsNoTracking().ToListAsync(ct);
    }

    public Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Rols.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<IEnumerable<Rol>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(Rol entity, CancellationToken ct = default)
    {
        db.Rols.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Rol entity, CancellationToken ct = default)
    {
        db.Rols.Update(entity);
        await Task.CompletedTask;
    }
}

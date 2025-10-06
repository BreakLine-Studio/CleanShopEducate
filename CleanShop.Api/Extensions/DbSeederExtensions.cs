using System;
using System.Linq;
using System.Threading.Tasks;
using CleanShop.Api.Helpers;
using CleanShop.Domain.Entities.Auth;
using CleanShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanShop.Api.Extensions;

public static class DbSeederExtensions
{
    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existingNames = await db.Rols.Select(r => r.Name).ToListAsync();
        var targetNames = Enum.GetNames(typeof(UserAuthorization.Roles));

        var toAdd = targetNames
            .Except(existingNames, StringComparer.OrdinalIgnoreCase)
            .Select(n => new Rol
            {
                Name = n,
                Description = $"{n} role"
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.Rols.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }
}

using System;
using CleanShop.Domain.Entities.Auth;

namespace CleanShop.Application.Abstractions.Auth;

public interface IUserMemberRolService
{
        Task<IEnumerable<UserMemberRol>> GetAllAsync();
        void Remove(UserMemberRol entity);
        void Update(UserMemberRol entity);
        Task<UserMemberRol?> GetByIdsAsync(int userMemberId, int roleId);

}

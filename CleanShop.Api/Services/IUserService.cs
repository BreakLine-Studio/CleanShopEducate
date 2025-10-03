using System;
using CleanShop.Api.DTOs.Auth;

namespace CleanShop.Api.Services;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterDto model);
    Task<DataUserDto> GetTokenAsync(LoginDto model);

    Task<string> AddRoleAsync(AddRoleDto model);

    Task<DataUserDto> RefreshTokenAsync(string refreshToken);
}

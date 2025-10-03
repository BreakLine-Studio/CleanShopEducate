using System;
using CleanShop.Api.Helpers;
using CleanShop.Application.Abstractions;
using CleanShop.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace CleanShop.Api.Services;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
}

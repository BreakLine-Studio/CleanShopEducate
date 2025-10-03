using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CleanShop.Api.DTOs.Auth;
using CleanShop.Api.Helpers;
using CleanShop.Application.Abstractions;
using CleanShop.Application.Abstractions.Auth;
using CleanShop.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanShop.Api.Services;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
    public UserService(IOptions<JWT> jwt, IUnitOfWork unitOfWork, IPasswordHasher<UserMember> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var usuario = new UserMember
        {
            Username = registerDto.Username ?? throw new ArgumentNullException(nameof(registerDto.Username)),
            Email = registerDto.Email ?? throw new ArgumentNullException(nameof(registerDto.Email)),
            Password = registerDto.Password ?? throw new ArgumentNullException(nameof(registerDto.Password)),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        usuario.Password = _passwordHasher.HashPassword(usuario, registerDto.Password!);

        var usuarioExiste = _unitOfWork.UserMembers
                                    .Find(u => u.Username.ToLower() == registerDto.Username.ToLower())
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {
            var rolPredeterminado = _unitOfWork.Roles
                                    .Find(u => u.Name == UserAuthorization.rol_default.ToString())
                                    .First();
            try
            {
                usuario.Rols.Add(rolPredeterminado);
                await _unitOfWork.UserMembers.AddAsync(usuario);
                await _unitOfWork.SaveChangesAsync();

                return $"El usuario  {registerDto.Username} ha sido registrado exitosamente";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return $"Error: {message}";
            }
        }
        else
        {
            return $"El usuario con {registerDto.Username} ya se encuentra registrado.";
        }
    }
    public async Task<DataUserDto> GetTokenAsync(LoginDto model)
    {
        DataUserDto datosUsuarioDto = new DataUserDto();
        if (string.IsNullOrEmpty(model.Username))
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = "El nombre de usuario no puede ser nulo o vacío.";
            return datosUsuarioDto;
        }
        var usuario = await _unitOfWork.UserMembers
                    .GetByUserNameAsync(model.Username);

        if (usuario == null)
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = $"No existe ningún usuario con el username {model.Username}.";
            return datosUsuarioDto;
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = $"La contraseña no puede ser nula o vacía para el usuario {usuario.Username}.";
            return datosUsuarioDto;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, model.Password);

        if (resultado == PasswordVerificationResult.Success)
        {
            datosUsuarioDto.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
            datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            datosUsuarioDto.Email = usuario.Email;
            datosUsuarioDto.UserName = usuario.Username;
            datosUsuarioDto.Roles = usuario.Rols
                                            .Select(u => u.Name)
                                            .ToList();

            if (usuario.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = usuario.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                if (activeRefreshToken != null)
                {
                    datosUsuarioDto.RefreshToken = activeRefreshToken.Token;
                    datosUsuarioDto.RefreshTokenExpiration = activeRefreshToken.Expires;
                }
                else
                {
                    // If no active refresh token is found, create a new one
                    var refreshToken = CreateRefreshToken();
                    datosUsuarioDto.RefreshToken = refreshToken.Token;
                    datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
                    usuario.RefreshTokens.Add(refreshToken);
                    await _unitOfWork.UserMembers.UpdateAsync(usuario);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                var refreshToken = CreateRefreshToken();
                datosUsuarioDto.RefreshToken = refreshToken.Token;
                datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
                usuario.RefreshTokens.Add(refreshToken);
                await _unitOfWork.UserMembers.UpdateAsync(usuario);
                await _unitOfWork.SaveChangesAsync();
            }

            return datosUsuarioDto;
        }
        datosUsuarioDto.IsAuthenticated = false;
        datosUsuarioDto.Message = $"Credenciales incorrectas para el usuario {usuario.Username}.";
        return datosUsuarioDto;
    }
    private JwtSecurityToken CreateJwtToken(UserMember usuario)
    {
        var roles = usuario.Rols;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Name));
        }
        var claims = new[]
        {
                                new Claim(JwtRegisteredClaimNames.Sub, usuario.Username),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                                new Claim("uid", usuario.Id.ToString())
                        }
        .Union(roleClaims);
        if (string.IsNullOrEmpty(_jwt.Key))
        {
            throw new InvalidOperationException("JWT Key cannot be null or empty.");
        }
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }
    }
    public async Task<string> AddRoleAsync(AddRoleDto model)
    {

        if (string.IsNullOrEmpty(model.Username))
        {
            return "Username cannot be null or empty.";
        }
        var user = await _unitOfWork.UserMembers
                    .GetByUserNameAsync(model.Username);
        if (user == null)
        {
            return $"User {model.Username} does not exists.";
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            return $"Password cannot be null or empty.";
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            var rolExists = _unitOfWork.Roles
                                        .Find(u => u.Name.Equals(model.Role, StringComparison.CurrentCultureIgnoreCase))
                                        .FirstOrDefault();

            if (rolExists != null)
            {
                var userHasRole = user.Rols
                                            .Any(u => u.Id == rolExists.Id);

                if (userHasRole == false)
                {
                    user.Rols.Add(rolExists);
                    await _unitOfWork.UserMembers.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }

                return $"Role {model.Role} added to user {model.Username} successfully.";
            }

            return $"Role {model.Role} was not found.";
        }
        return $"Invalid Credentials";
    }
    public async Task<DataUserDto> RefreshTokenAsync(string refreshToken)
    {
        var dataUserDto = new DataUserDto();

        var usuario = await _unitOfWork.UserMembers
                        .GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not assigned to any user.";
            return dataUserDto;
        }

        var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not active.";
            return dataUserDto;
        }
        //Revoque the current refresh token and
        refreshTokenBd.Revoked = DateTime.UtcNow;
        //generate a new refresh token and save it in the database
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        await _unitOfWork.UserMembers.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
        //Generate a new Json Web Token
        dataUserDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        dataUserDto.Email = usuario.Email;
        dataUserDto.UserName = usuario.Username;
        dataUserDto.Roles = usuario.Rols
                                        .Select(u => u.Name)
                                        .ToList();
        dataUserDto.RefreshToken = newRefreshToken.Token;
        dataUserDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return dataUserDto;
    }
}

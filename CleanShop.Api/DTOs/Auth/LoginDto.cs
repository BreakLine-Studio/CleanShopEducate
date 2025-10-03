using System;
using System.ComponentModel.DataAnnotations;

namespace CleanShop.Api.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}

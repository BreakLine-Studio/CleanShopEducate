using System;

namespace CleanShop.Api.Helpers;

public class UserAuthorization
{
    public enum Roles
    {
        Administrator,
        Manager,
        Employee
    }

    public const Roles rol_default = Roles.Employee;
}
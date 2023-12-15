/*using Microsoft.AspNetCore.Mvc;
using Paket;
using Microsoft.AspNetCore.Authorization;
using System;


namespace PasswordManagerApp.Webapi.Controllers;
public class UserLoginController: ControllerBase
{
    // DTO class for the JSON body of the login request
    public record CredentialsDto(string username, string password);

    private readonly PasswordmanagerContext _db;
    private readonly AuthService _authService;
    // DI von AuthService funktioniert nur, wenn es im Service Provider
    // registriert wurde.
    public UserLoginController(PasswordmanagerContext db, AuthService authService)
    {
        _db = db;
        _authService = authService;
    }
}
*/
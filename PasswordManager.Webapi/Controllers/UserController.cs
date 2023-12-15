using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

using PasswordManager.Application.Model;
using PasswordManager.Application.Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;
using System;
using PasswordManagerApp.Applcation.Infastruture;
using System.Linq;

namespace SecAn.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class UserController : ControllerBase
{

    public record CredentialsDto(string username, string password);

    private readonly PasswordmanagerContext _db;
    private readonly IConfiguration _config;  
    public UserController(PasswordmanagerContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

  
    /// POST /api/user/login
  
    [HttpPost("login")]
    public IActionResult Login([FromBody] CredentialsDto credentials)
    {
#pragma warning disable CS8604 
        var secret = Convert.FromBase64String(_config["Secret"]);
#pragma warning restore CS8604 
        var lifetime = TimeSpan.FromHours(3);

        var user = _db.Users.FirstOrDefault(a => a.Username == credentials.username);
        if (user is null) { return Unauthorized(); }
        if (!user.CheckPassword(credentials.password)) { return Unauthorized(); }

        string role = "Admin";  
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username.ToString()),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow + lifetime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new
        {
            user.Username,
            UserGuid = user.Guid,
            Role = role,
            Token = tokenHandler.WriteToken(token)
        });
    }

    /// GET /api/user/me
 
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetUserdata()
    {
        var username = HttpContext?.User.Identity?.Name;
        if (username is null) { return Unauthorized(); }

        var user = _db.Users.FirstOrDefault(a => a.Username == username);
        if (user is null) { return Unauthorized(); }

        return Ok(new
        {
            user.Username,
            UserGuid = user.Guid,
        });
    }

    /// GET /api/user/all
 
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public IActionResult GetAllUsers()
    {
        var users = _db.Users
            .Select(a => new
            {
                a.Username,
                UserGuid = a.Guid,
            })
            .ToList();
        if (users is null) { return BadRequest(); }
        return Ok(users);
    }
}

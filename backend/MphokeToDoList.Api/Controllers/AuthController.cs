using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MphokeToDoList.Api.Data;
using MphokeToDoList.Api.Models;
using MphokeToDoList.Api.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MphokeToDoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // ================================
    // Register
    // ================================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already exists");

        var user = new User
        {
            Username = dto.Username,
            Role = "User",
            PasswordHash = ComputeHash(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }

    // ================================
    // Login (store JWT in HttpOnly cookie)
    // ================================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (dbUser == null || dbUser.PasswordHash != ComputeHash(dto.Password))
            return Unauthorized("Invalid credentials");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSecret"] ?? "YourVeryStrongJWTSecretKey");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, dbUser.Id.ToString()),
            new Claim(ClaimTypes.Name, dbUser.Username),
            new Claim(ClaimTypes.Role, dbUser.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        //  Return token in JSON for SPA
        return Ok(new { token = jwtToken, message = "Login successful" });
    }

    // ================================
    // Logout (delete cookie)
    // ================================
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwtToken");
        return Ok(new { message = "Logged out successfully" });
    }

    // ================================
    // Helper: Hash Password
    // ================================
    private string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
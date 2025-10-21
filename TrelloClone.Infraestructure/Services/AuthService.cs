using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Application.Interfaces; 
using TrelloClone.Infrastructure.Data;      
using TrelloClone.Domain.Entities;
using BCrypt.Net;

namespace TrelloClone.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context; 
    private readonly IConfiguration _config;

    public AuthService(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email);


        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            return null;

        var token = GenerateJwtToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            }
        };
    }

    public async Task<UsuarioDto?> GetUsuarioByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return null;

        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol
        };
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
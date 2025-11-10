using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Application.Interfaces; 
using TrelloClone.Infraestructure.Data;      
using TrelloClone.Domain.Entities;
using BCrypt.Net;

namespace TrelloClone.Infraestructure.Services;

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

    public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Verificar si el email ya existe
        var emailExists = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (emailExists)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // 2. Verificar si el nombre de usuario ya existe (opcional pero recomendado)
        var usernameExists = await _context.Usuarios
            .AnyAsync(u => u.Nombre.ToLower() == request.Username.ToLower());

        if (usernameExists)
        {
            throw new InvalidOperationException("El nombre de usuario ya está en uso");
        }

        // 3. Crear nuevo usuario
        var usuario = new Usuario
        {
            Nombre = request.Username,
            Email = request.Email.ToLower(), // Normalizar email
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = "User", // Por defecto es User, no Admin
            FechaCreacion = DateTime.UtcNow
        };

        // 4. Guardar en base de datos
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // 5. Retornar datos del usuario creado (sin password)
        return new RegisterResponseDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
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
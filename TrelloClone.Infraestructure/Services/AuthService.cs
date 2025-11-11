using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Application.Interfaces; 
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Data;      
using TrelloClone.Infraestructure.Exceptions;

namespace TrelloClone.Infraestructure.Services;

public class AuthService : IAuthService
{
    private readonly UsuarioServices _usuarioServices;
    private readonly IConfiguration _config;
    private readonly IEncoderService _encoderServices;

    public AuthService(UsuarioServices usuarioServices, IConfiguration config, IEncoderService encoderServices)
    {
        _usuarioServices = usuarioServices;
        _config = config;
        _encoderServices = encoderServices;
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
        var user = await _usuarioServices.GetOneByEmailOrUsername(request.Email, request.Username);
        if (user != null)
        {
            throw new HttpResponseError(HttpStatusCode.BadRequest, "User already exists");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new HttpResponseError(HttpStatusCode.BadRequest, "Password doesn't match");
        }

        request.Password = _encoderServices.Encode(request.Password);

        var userCreated = await _usuarioServices.CreateOne(request);

        return new RegisterResponseDto
        {
            Id = userCreated.Id,
            Nombre = userCreated.Nombre,
            Email = userCreated.Email,
            Rol = userCreated.Rol
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
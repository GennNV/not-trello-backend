using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Data;      
using TrelloClone.Infraestructure.Exceptions;

namespace TrelloClone.Infraestructure.Services;

public class AuthService
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

    async public Task<LoginResponseDto?> LoginAsync(LoginRequestDto login)
    {
        bool IsEmail = login.Email.Contains("@");
        Usuario user;
        if (IsEmail)
        {
            user = await _usuarioServices.GetOneByEmailOrUsername(login.Email, null);
        }
        else
        {
            user = await _usuarioServices.GetOneByEmailOrUsername(null, login.Email);
        }

        if (user == null)
        {
            throw new HttpResponseError(HttpStatusCode.BadRequest, "Invalid credentials");
        }

        bool IsPassMatch = _encoderServices.Verify(login.Password, user.PasswordHash);

        if (!IsPassMatch)
        {
            throw new HttpResponseError(HttpStatusCode.BadRequest, "Invalid credentials");
        }

        var token = GenerateJwtToken(user);
        var usuario = new UsuarioDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        };

        return new LoginResponseDto { Token = token, Usuario = usuario  };
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

    public async Task<UsuarioDto?> GetUsuarioByIdAsync(int id)
    {
        var usuario = await _usuarioServices.GetOneById(id);
        if (usuario == null) return null;

        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol
        };
    }
}
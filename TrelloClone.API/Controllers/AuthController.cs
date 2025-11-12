using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Application.Interfaces;
using TrelloClone.Infraestructure.Exceptions;

namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto login)
    {
        try
        {
            // Validar que el request no sea nulo
            if (login == null || string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest(new { message = "Email y contraseña son requeridos." });
            }

            var result = await _authService.LoginAsync(login);

            // ✅ VALIDAR QUE EL TOKEN SE GENERÓ CORRECTAMENTE
            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                _logger.LogWarning("El servicio de autenticación no generó un token válido");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error al generar el token de autenticación." });
            }

            return Ok(new
            {
                message = "Inicio de sesión exitoso",
                data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Usuario/contraseña incorrectos
            _logger.LogWarning($"Intento de login fallido: {ex.Message}");
            return Unauthorized(new { message = "Credenciales inválidas." });
        }
        catch (HttpResponseError ex)
        {
            _logger.LogError($"Error HTTP en login: {ex.Message}");
            return StatusCode((int)ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en login");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error interno del servidor." });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            // Validar request
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Datos de registro inválidos." });
            }

            var result = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Usuario registrado correctamente",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            // Usuario ya existe
            _logger.LogWarning($"Intento de registro duplicado: {ex.Message}");
            return Conflict(new { message = ex.Message });
        }
        catch (HttpResponseError ex)
        {
            _logger.LogError($"Error HTTP en registro: {ex.Message}");
            return StatusCode((int)ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en registro");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error interno del servidor." });
        }
    }

   
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UsuarioDto>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Token válido pero sin claim de NameIdentifier");
                return Unauthorized(new { message = "Token inválido." });
            }

            var usuario = await _authService.GetUsuarioByIdAsync(userId);

            if (usuario == null)
            {
                _logger.LogWarning($"Usuario con ID {userId} no encontrado en BD");
                return NotFound(new { message = "Usuario no encontrado." });
            }

            return Ok(new
            {
                message = "Usuario obtenido correctamente",
                data = usuario
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario actual");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error interno del servidor." });
        }
    }
}
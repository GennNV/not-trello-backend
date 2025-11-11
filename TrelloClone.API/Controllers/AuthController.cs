using Microsoft.AspNetCore.Mvc;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Application.Interfaces;
using TrelloClone.Infraestructure.Exceptions;
using TrelloClone.Infraestructure.Services;
namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto login)
    {
        try
        {
            var res = await _authService.LoginAsync(login);
            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                data = res
            });
        }
        catch (HttpResponseError ex)
        {
            return StatusCode(
                (int)ex.StatusCode,
                new
                {
                    message = "Error en la solicitud HTTP.",
                    details = ex.Message
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Error interno al iniciar sesión.",
                    details = ex.Message
                }
            );
        }
    }


    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(new
            {
                message = "Usuario registrado correctamente.",
                data = result
            });
        }
        catch (HttpResponseError ex)
        {
            return StatusCode(
                (int)ex.StatusCode,
                new
                {
                    message = $"Error HTTP: {ex.Message}"
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Error interno al registrar el usuario.",
                    details = ex.Message
                }
            );
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<UsuarioDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var usuario = await _authService.GetUsuarioByIdAsync(userId);

        if (usuario == null)
            return NotFound();

        return Ok(usuario);
    }
}

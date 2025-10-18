using Microsoft.AspNetCore.Mvc;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Application.Interfaces;

namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(result);
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

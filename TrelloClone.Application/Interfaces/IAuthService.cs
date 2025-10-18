using TrelloClone.Application.DTOs.Auth;

namespace TrelloClone.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<UsuarioDto?> GetUsuarioByIdAsync(int id);
}
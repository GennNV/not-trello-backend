using System.Net;
using TrelloClone.Application.DTOs.Auth;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Exceptions;
using TrelloClone.Infraestructure.Repositories;

namespace TrelloClone.Infraestructure.Services
{
    public class UsuarioServices
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioServices(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        async private Task<Usuario> GetOneByIdOrException(int id)
        {
            var u = await _repo.GetOne(x => x.Id == id);
            if (u == null)
            {
                throw new HttpResponseError(
                    HttpStatusCode.NotFound,
                    $"No se encontro el Usuario con ID = {id}"
                );
            }
            return u;
        }

        async public Task<Usuario> GetOneById(int id) => await GetOneByIdOrException(id);

        async public Task<Usuario> CreateOne(RegisterRequestDto registerDTO)
        {
            var user = new Usuario
            {
                Nombre = registerDTO.Username,
                Email = registerDTO.Email,
                PasswordHash = registerDTO.Password,
                Rol = "User"
            };

            await _repo.CreateOne(user);

            return user;
        }

        async public Task<Usuario> GetOneByEmailOrUsername(string? email, string? userName)
        {
            Usuario usuario;

            if (!string.IsNullOrEmpty(userName))
            {
                usuario = await _repo.GetOne(x => x.Nombre == userName);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                usuario = await _repo.GetOne(x => x.Email == email);
            }
            else
            {
                throw new HttpResponseError(
                    HttpStatusCode.BadRequest,
                    "UserName and email are empty"
                );
            }
            return usuario;
        }


    }
}

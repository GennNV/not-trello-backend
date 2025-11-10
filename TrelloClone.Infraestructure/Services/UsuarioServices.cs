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
            var user = _mapper.Map<Usuario>(registerDTO);

            var rolDefault = await _rolServices.GetOneByName(ROL.USER);

            user.Roles = new List<Rol>() { rolDefault };

            await _repo.CreateOne(user);

            return user;
        }
    }
}

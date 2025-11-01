using TrelloClone.Domain.Entities;

namespace TrelloClone.Application.DTOs.Tableros
{
    public class CrearTableroDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public ICollection<Lista> Listas { get; set; } = new List<Lista>();
    }
}

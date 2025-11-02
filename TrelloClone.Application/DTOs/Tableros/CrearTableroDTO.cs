using System.ComponentModel.DataAnnotations;

namespace TrelloClone.Application.DTOs.Tableros
{
    public class CrearTableroDTO
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#EF4444";

        public int UsuarioId { get; set; }
        }
}

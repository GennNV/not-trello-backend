using System.ComponentModel.DataAnnotations;

namespace TrelloClone.Application.DTOs.Tarjetas
{
    public class MoverTarjetaDTO
    {
        [Required]
        public int NuevaListaId { get; set; }
        [Required]
        public int NuevoOrden { get; set; }
    }
}

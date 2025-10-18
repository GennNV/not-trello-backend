using System.ComponentModel.DataAnnotations;

namespace TrelloClone.Application.DTOs.Tarjetas;

public class CreateTarjetaDto
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public string Prioridad { get; set; } = "Media";

    [Required]
    public int ListaId { get; set; }

    public DateTime? FechaVencimiento { get; set; }
    public int? AsignadoAId { get; set; }
}
namespace TrelloClone.Application.DTOs.Tarjetas;

public class TarjetaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int Orden { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public int ListaId { get; set; }
    public string NombreLista { get; set; } = string.Empty;
    public int? AsignadoAId { get; set; }
    public string? NombreAsignado { get; set; }
}
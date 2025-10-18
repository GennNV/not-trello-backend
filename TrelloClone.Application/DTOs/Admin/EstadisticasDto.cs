namespace TrelloClone.Application.DTOs.Admin;

public class EstadisticasDto
{
    public int TotalUsuarios { get; set; }
    public int TotalTableros { get; set; }
    public int TotalTarjetas { get; set; }
    public List<TarjetasPorEstadoDto> TarjetasPorEstado { get; set; } = new();
    public List<TarjetasPorPrioridadDto> TarjetasPorPrioridad { get; set; } = new();
}
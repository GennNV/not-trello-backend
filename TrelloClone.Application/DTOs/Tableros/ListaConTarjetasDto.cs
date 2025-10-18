using TrelloClone.Application.DTOs.Tarjetas;

namespace TrelloClone.Application.DTOs.Tableros;

public class ListaConTarjetasDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Orden { get; set; }
    public List<TarjetaDto> Tarjetas { get; set; } = new();
}
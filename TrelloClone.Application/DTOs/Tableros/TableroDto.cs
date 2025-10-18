namespace TrelloClone.Application.DTOs.Tableros;

public class TableroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public List<ListaConTarjetasDto> Listas { get; set; } = new();
}
namespace TrelloClone.Domain.Entities
{
    public class Tablero
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#0079BF";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public Usuario Usuario { get; set; } = null!;

        public ICollection<Lista> Listas { get; set; } = new List<Lista>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloClone.Domain.Entities
{
    public class Tarjeta
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media"; // Baja, Media, Alta
        public string Estado { get; set; } = "Todo"; // Todo, InProgress, Done
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaVencimiento { get; set; }

        public int ListaId { get; set; }
        public Lista Lista { get; set; } = null!;

        public int? AsignadoAId { get; set; }
        public Usuario? AsignadoA { get; set; }
    }
}

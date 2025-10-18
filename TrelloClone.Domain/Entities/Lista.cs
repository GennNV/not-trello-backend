using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloClone.Domain.Entities
{
    public class Lista
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int Orden { get; set; }

        public int TableroId { get; set; }
        public Tablero Tablero { get; set; } = null!;

        public ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
    }
}

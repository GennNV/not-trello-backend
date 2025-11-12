using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrelloClone.Domain.Entities;

namespace TrelloClone.Application.DTOs.Tableros
{
    public class ListaDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public int Orden { get; set; }
        public ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloClone.Application.DTOs.Tableros
{
    public class ReorderListasDto
    {
        public List<int> ListaIds { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrelloClone.Domain.Entities;

namespace TrelloClone.Application.DTOs.Tableros
{
    public class CreateListaDTO // Por defecto va a estar vacía (regla de negocio)
    {
        [Required]
        public string Titulo { get; set; } = string.Empty;
        public int Orden { get; set; }
    }
}

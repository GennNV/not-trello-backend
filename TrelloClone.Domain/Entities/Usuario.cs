using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloClone.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "User"; // Admin o User
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<Tablero> Tableros { get; set; } = new List<Tablero>();
    }
}

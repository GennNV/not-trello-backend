using TrelloClone.Domain.Entities;
using BCrypt.Net;

namespace TrelloClone.Infraestructure.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Asegurar que la base de datos existe
        context.Database.EnsureCreated();

        // Si ya hay usuarios, no seed
        if (context.Usuarios.Any())
        {
            return; // La base de datos ya fue inicializada
        }

        // Crear usuarios de prueba
        var usuarios = new Usuario[]
        {
            new Usuario
            {
                Nombre = "Admin User",
                Email = "admin@trello.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Rol = "Admin",
                FechaCreacion = DateTime.UtcNow
            },
            new Usuario
            {
                Nombre = "Juan Perez",
                Email = "user@trello.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Rol = "User",
                FechaCreacion = DateTime.UtcNow
            },
            new Usuario
            {
                Nombre = "Jose Paz",
                Email = "jose@trello.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Rol = "User",
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Usuarios.AddRange(usuarios);
        context.SaveChanges();

        // Crear tableros de ejemplo
        var tableros = new Tablero[]
        {
            new Tablero
            {
                Titulo = "Proyecto de Ejemplo",
                Descripcion = "Tablero demo para probar la aplicación",
                Color = "#0079BF",
                UsuarioId = usuarios[0].Id,
                FechaCreacion = DateTime.UtcNow
            },
            new Tablero
            {
                Titulo = "Desarrollo Web",
                Descripcion = "Proyecto de desarrollo fullstack",
                Color = "#00C7E5",
                UsuarioId = usuarios[0].Id,
                FechaCreacion = DateTime.UtcNow
            },
            new Tablero
            {
                Titulo = "Tareas Personales",
                Descripcion = "Organización personal",
                Color = "#7BC86C",
                UsuarioId = usuarios[1].Id,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Tableros.AddRange(tableros);
        context.SaveChanges();

        // Crear listas para el primer tablero
        var listas = new Lista[]
        {
            new Lista
            {
                Titulo = "Por Hacer",
                Orden = 1,
                TableroId = tableros[0].Id
            },
            new Lista
            {
                Titulo = "En Progreso",
                Orden = 2,
                TableroId = tableros[0].Id
            },
            new Lista
            {
                Titulo = "En Revisión",
                Orden = 3,
                TableroId = tableros[0].Id
            },
            new Lista
            {
                Titulo = "Completado",
                Orden = 4,
                TableroId = tableros[0].Id
            }
        };

        context.Listas.AddRange(listas);
        context.SaveChanges();

        // Crear listas para el segundo tablero
        var listasDesarrollo = new Lista[]
        {
            new Lista
            {
                Titulo = "Backlog",
                Orden = 1,
                TableroId = tableros[1].Id
            },
            new Lista
            {
                Titulo = "Sprint Actual",
                Orden = 2,
                TableroId = tableros[1].Id
            },
            new Lista
            {
                Titulo = "Testing",
                Orden = 3,
                TableroId = tableros[1].Id
            },
            new Lista
            {
                Titulo = "Producción",
                Orden = 4,
                TableroId = tableros[1].Id
            }
        };

        context.Listas.AddRange(listasDesarrollo);
        context.SaveChanges();

        // Crear tarjetas de ejemplo para el primer tablero
        var tarjetas = new Tarjeta[]
        {
            new Tarjeta
            {
                Titulo = "Implementar autenticación JWT",
                Descripcion = "Crear sistema de login con tokens JWT para autenticación segura. Incluir refresh tokens y validación de roles.",
                Prioridad = "Alta",
                Estado = "InProgress",
                ListaId = listas[1].Id,
                Orden = 1,
                AsignadoAId = usuarios[0].Id,
                FechaCreacion = DateTime.UtcNow,
                FechaVencimiento = DateTime.UtcNow.AddDays(7)
            },
            new Tarjeta
            {
                Titulo = "Diseñar interfaz de tableros",
                Descripcion = "Crear componentes React para la visualización de tableros estilo Kanban con drag & drop",
                Prioridad = "Alta",
                Estado = "Todo",
                ListaId = listas[0].Id,
                Orden = 1,
                AsignadoAId = usuarios[1].Id,
                FechaCreacion = DateTime.UtcNow,
                FechaVencimiento = DateTime.UtcNow.AddDays(10)
            },
            new Tarjeta
            {
                Titulo = "Configurar base de datos",
                Descripcion = "Setup completo de Entity Framework Core con migraciones y seed de datos iniciales",
                Prioridad = "Alta",
                Estado = "Done",
                ListaId = listas[3].Id,
                Orden = 1,
                FechaCreacion = DateTime.UtcNow.AddDays(-5)
            },
            new Tarjeta
            {
                Titulo = "Crear API REST para tarjetas",
                Descripcion = "Endpoints CRUD completos para gestión de tarjetas con validación",
                Prioridad = "Media",
                Estado = "InProgress",
                ListaId = listas[1].Id,
                Orden = 2,
                AsignadoAId = usuarios[0].Id,
                FechaCreacion = DateTime.UtcNow.AddDays(-2)
            },
            new Tarjeta
            {
                Titulo = "Implementar búsqueda y filtros",
                Descripcion = "Agregar funcionalidad de búsqueda por texto y filtros por estado/prioridad",
                Prioridad = "Media",
                Estado = "Todo",
                ListaId = listas[0].Id,
                Orden = 2,
                FechaCreacion = DateTime.UtcNow
            },
            new Tarjeta
            {
                Titulo = "Documentar API con Swagger",
                Descripcion = "Agregar documentación completa de todos los endpoints con ejemplos",
                Prioridad = "Baja",
                Estado = "Todo",
                ListaId = listas[0].Id,
                Orden = 3,
                FechaCreacion = DateTime.UtcNow
            },
            new Tarjeta
            {
                Titulo = "Testing unitario de servicios",
                Descripcion = "Crear tests unitarios para AuthService y TarjetaService usando xUnit",
                Prioridad = "Media",
                Estado = "EnRevision",
                ListaId = listas[2].Id,
                Orden = 1,
                AsignadoAId = usuarios[2].Id,
                FechaCreacion = DateTime.UtcNow.AddDays(-1)
            },
            new Tarjeta
            {
                Titulo = "Configurar CI/CD",
                Descripcion = "Setup de pipeline de integración continua con GitHub Actions",
                Prioridad = "Baja",
                Estado = "Todo",
                ListaId = listas[0].Id,
                Orden = 4,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Tarjetas.AddRange(tarjetas);
        context.SaveChanges();

        // Crear tarjetas para el segundo tablero
        var tarjetasDesarrollo = new Tarjeta[]
        {
            new Tarjeta
            {
                Titulo = "Migrar a .NET 9",
                Descripcion = "Actualizar proyecto a la última versión de .NET",
                Prioridad = "Media",
                Estado = "Todo",
                ListaId = listasDesarrollo[0].Id,
                Orden = 1,
                FechaCreacion = DateTime.UtcNow
            },
            new Tarjeta
            {
                Titulo = "Optimizar queries de EF Core",
                Descripcion = "Revisar y optimizar queries N+1 con Include apropiados",
                Prioridad = "Alta",
                Estado = "InProgress",
                ListaId = listasDesarrollo[1].Id,
                Orden = 1,
                AsignadoAId = usuarios[0].Id,
                FechaCreacion = DateTime.UtcNow
            },
            new Tarjeta
            {
                Titulo = "Implementar cache con Redis",
                Descripcion = "Agregar capa de caché para mejorar performance",
                Prioridad = "Baja",
                Estado = "Todo",
                ListaId = listasDesarrollo[0].Id,
                Orden = 2,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Tarjetas.AddRange(tarjetasDesarrollo);
        context.SaveChanges();
    }
}
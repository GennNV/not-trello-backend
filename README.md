# 🎯 TrelloClone - Backend API

API REST desarrollada con ASP.NET Core para gestión de tableros, listas y tarjetas tipo Trello con sistema de autenticación y roles.

## 📋 Tabla de Contenidos

- [Tecnologías](#tecnologías)
- [Arquitectura](#arquitectura)
- [Características](#características)
- [Requisitos](#requisitos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Credenciales de Prueba](#credenciales-de-prueba)
- [Seguridad](#seguridad)
- [Base de Datos](#base-de-datos)

---

## 🛠️ Tecnologías

- **.NET 9.0** - Framework principal
- **ASP.NET Core Web API** - Framework web
- **Entity Framework Core 9.0** - ORM para acceso a datos
- **SQL Server** - Base de datos relacional
- **JWT (JSON Web Tokens)** - Autenticación y autorización
- **BCrypt.Net-Next** - Hash seguro de contraseñas
- **Swagger/OpenAPI** - Documentación interactiva de API
- **CORS** - Integración con frontend

---

## 🏗️ Arquitectura

El proyecto implementa una **arquitectura en capas (Clean Architecture/Onion Architecture)** con separación clara de responsabilidades y el **patrón Repository** para acceso a datos.

```
TrelloClone.Backend/
├── TrelloClone.Domain/               # 🔷 Capa de Dominio
│   └── Entities/                     # Entidades del negocio
│       ├── Usuario.cs                # Usuario con rol (Admin/User)
│       ├── Tablero.cs                # Tablero con color y descripción
│       ├── Lista.cs                  # Lista ordenada dentro de tablero
│       └── Tarjeta.cs                # Tarjeta con prioridad y estado
│
├── TrelloClone.Application/          # 🔷 Capa de Aplicación
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Auth/                     # Login, Register, Usuario
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── LoginResponseDto.cs
│   │   │   ├── RegisterRequestDto.cs
│   │   │   ├── RegisterResponseDto.cs
│   │   │   └── UsuarioDto.cs
│   │   ├── Tableros/                 # Tableros y Listas
│   │   │   ├── TableroDto.cs
│   │   │   ├── CrearTableroDTO.cs
│   │   │   ├── ListaDTO.cs
│   │   │   ├── CreateListaDTO.cs
│   │   │   ├── ListaConTarjetasDto.cs
│   │   │   └── ReordenListaDTO.cs
│   │   ├── Tarjetas/                 # Tarjetas
│   │   │   ├── TarjetaDto.cs
│   │   │   ├── CreateTarjetaDto.cs
│   │   │   └── MoverTarjetaDTO.cs
│   │   └── Admin/                    # Dashboard Admin
│   │       ├── EstadisticasDto.cs
│   │       ├── TarjetasPorEstadoDto.cs
│   │       └── TarjetasPorPrioridadDto.cs
│   └── Interfaces/                   # Contratos de servicios
│       ├── IAuthService.cs
│       ├── ITarjetaService.cs
│       └── ITablerosService.cs
│
├── TrelloClone.Infrastructure/       # 🔷 Capa de Infraestructura
│   ├── Data/                         # Contexto y configuración BD
│   │   ├── ApplicationDbContext.cs   # DbContext de EF Core
│   │   ├── DbInitializer.cs          # Seed de datos iniciales
│   │   └── ApplicationDbContextFactory.cs
│   ├── Repositories/                 # Patrón Repository
│   │   ├── Repository.cs             # Repository genérico
│   │   ├── UsuarioRepository.cs
│   │   ├── TableroRepository.cs
│   │   └── TarjetaRepository.cs
│   ├── Services/                     # Implementaciones
│   │   ├── AuthService.cs            # Login, Register, JWT
│   │   ├── TarjetaService.cs         # CRUD Tarjetas
│   │   ├── TablerosService.cs        # CRUD Tableros y Listas
│   │   ├── UsuarioServices.cs        # Gestión de usuarios
│   │   └── EncoderServices.cs        # Hash de contraseñas
│   └── Migrations/                   # Migraciones de BD
│
└── TrelloClone.API/                  # 🔷 Capa de Presentación
    ├── Controllers/                  # Endpoints REST
    │   ├── AuthController.cs         # Login, Register, /me
    │   ├── TablerosController.cs     # CRUD Tableros y Listas
    │   ├── TarjetasController.cs     # CRUD Tarjetas
    │   └── AdminController.cs        # Estadísticas (Admin only)
    ├── Program.cs                    # Configuración y DI
    └── appsettings.json              # Variables de configuración
```

### 🔄 Flujo de Dependencias

```
API Layer (Controllers)
        ↓
Application Layer (Interfaces + DTOs)
        ↓
Infrastructure Layer (Services + Repositories)
        ↓
Domain Layer (Entities)
        ↓
    Database
```

**Principios SOLID aplicados:**

- ✅ **Single Responsibility** - Cada clase tiene una única responsabilidad
- ✅ **Open/Closed** - Extensible sin modificar código existente
- ✅ **Liskov Substitution** - Interfaces y abstracciones
- ✅ **Interface Segregation** - Interfaces específicas
- ✅ **Dependency Inversion** - Dependencias mediante interfaces

**Patrones de diseño:**

- ✅ **Repository Pattern** - Abstracción de acceso a datos
- ✅ **Dependency Injection** - Inyección de dependencias nativa
- ✅ **DTO Pattern** - Separación entre entidades y datos transferidos

---

## ⚡ Características

### 🔐 Autenticación y Autorización

- ✅ Registro de usuarios con validaciones
- ✅ Login con JWT
- ✅ Sistema de roles (Admin/User)
- ✅ Endpoints protegidos por rol
- ✅ Hash seguro de contraseñas con BCrypt
- ✅ Tokens con expiración de 24 horas

### 📊 Gestión de Tableros

- ✅ Crear tableros (Admin)
- ✅ Listar todos los tableros
- ✅ Obtener tablero por ID con listas y tarjetas
- ✅ Eliminar tableros (Admin)
- ✅ Tableros con colores personalizables

### 📝 Gestión de Listas

- ✅ Crear listas en tableros (Admin)
- ✅ Reordenar listas dentro de un tablero (Admin)
- ✅ Eliminar listas (Admin)
- ✅ Listas ordenadas por posición

### 🎯 Gestión de Tarjetas

- ✅ CRUD completo de tarjetas (Admin)
- ✅ Mover tarjetas entre listas
- ✅ Filtrar por búsqueda de texto
- ✅ Filtrar por estado (Todo/InProgress/Done)
- ✅ Prioridades (Baja/Media/Alta)
- ✅ Asignación de tarjetas a usuarios
- ✅ Fechas de vencimiento
- ✅ Ordenamiento automático

### 📈 Panel de Administración

- ✅ Estadísticas del sistema
- ✅ Total de usuarios, tableros y tarjetas
- ✅ Distribución de tarjetas por estado
- ✅ Distribución de tarjetas por prioridad

### 🌐 Integración Frontend

- ✅ CORS configurado para React/Vue
- ✅ Swagger UI integrado
- ✅ Respuestas JSON estandarizadas
- ✅ Manejo de errores HTTP

---

## 📋 Requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express o instancia completa)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/) (opcional)

---

## 🚀 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/trelloclone-backend.git
cd trelloclone-backend
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Verificar la instalación

```bash
dotnet --version
# Debe mostrar: 9.0.xxx o superior
```

---

## ⚙️ Configuración

### 1. Configurar la Base de Datos

Editar `TrelloClone.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TrelloCloneDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SuperSecretKeyForJwtTokenGeneration123456789",
    "Issuer": "TrelloCloneAPI",
    "Audience": "TrelloCloneClient"
  }
}
```

**Opciones de Connection String:**

**SQL Server Express (recomendado):**

```
Server=localhost\\SQLEXPRESS;Database=TrelloCloneDb;Trusted_Connection=True;TrustServerCertificate=True;
```

**LocalDB:**

```
Server=(localdb)\\mssqllocaldb;Database=TrelloCloneDb;Trusted_Connection=true;MultipleActiveResultSets=true
```

**SQL Server con credenciales:**

```
Server=localhost;Database=TrelloCloneDb;User Id=sa;Password=TuPassword;TrustServerCertificate=true;
```

**Docker:**

```bash
# Levantar SQL Server en Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Connection String
Server=localhost,1433;Database=TrelloCloneDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
```

### 2. Configurar CORS (Opcional)

Si tu frontend está en otro puerto, editar `Program.cs`:

```csharp
policy.WithOrigins(
    "http://localhost:5173",  // Vite default
    "http://localhost:5174",
    "http://localhost:3000"   // React default
)
```

### 3. Configurar JWT (Producción)

**En producción**, usar variables de entorno:

```bash
# Linux/Mac
export Jwt__Key="TuClaveSecretaSuperSeguraYLarga123456789"

# Windows PowerShell
$env:Jwt__Key="TuClaveSecretaSuperSeguraYLarga123456789"
```

---

## ▶️ Ejecución

### Modo Desarrollo

```bash
cd TrelloClone.API
dotnet run
```

La API estará disponible en:

- **Swagger UI:** http://localhost:5000
- **API Base:** http://localhost:5000/api
- **Health Check:** http://localhost:5000/health

### Modo Producción

```bash
dotnet build -c Release
dotnet run -c Release --no-build
```

### Con Hot Reload (Recomendado para desarrollo)

```bash
dotnet watch run
```

---

## 📊 Estructura del Proyecto

### Entidades Principales

#### 👤 Usuario

```json
{
  "id": 1,
  "nombre": "Admin User",
  "email": "admin@trello.com",
  "rol": "Admin",
  "fechaCreacion": "2025-11-12T00:00:00Z"
}
```

**Propiedades:**

- `Id`: Identificador único
- `Nombre`: Nombre del usuario
- `Email`: Email único (usado para login)
- `PasswordHash`: Contraseña hasheada con BCrypt
- `Rol`: "Admin" o "User"
- `FechaCreacion`: Fecha de registro
- `Tableros`: Lista de tableros del usuario

#### 📋 Tablero

```json
{
  "id": 1,
  "titulo": "Proyecto Backend",
  "descripcion": "Desarrollo de la API REST",
  "color": "#EF4444",
  "fechaCreacion": "2025-11-12T00:00:00Z",
  "usuarioId": 1,
  "nombreUsuario": "Admin User",
  "listas": [...]
}
```

**Propiedades:**

- `Id`: Identificador único
- `Titulo`: Nombre del tablero
- `Descripcion`: Descripción opcional
- `Color`: Color en formato HEX
- `FechaCreacion`: Fecha de creación
- `UsuarioId`: ID del propietario
- `Listas`: Colección de listas

#### 📝 Lista

```json
{
  "id": 1,
  "titulo": "Por Hacer",
  "orden": 1,
  "tarjetas": [...]
}
```

**Propiedades:**

- `Id`: Identificador único
- `Titulo`: Nombre de la lista
- `Orden`: Posición en el tablero
- `TableroId`: ID del tablero padre
- `Tarjetas`: Colección de tarjetas

#### 🎯 Tarjeta

```json
{
  "id": 1,
  "titulo": "Implementar autenticación",
  "descripcion": "Sistema de login con JWT",
  "prioridad": "Alta",
  "estado": "InProgress",
  "orden": 1,
  "fechaCreacion": "2025-11-12T00:00:00Z",
  "fechaVencimiento": "2025-12-31T00:00:00Z",
  "listaId": 1,
  "nombreLista": "En Progreso",
  "asignadoAId": 2,
  "nombreAsignado": "Juan Pérez"
}
```

**Propiedades:**

- `Id`: Identificador único
- `Titulo`: Título de la tarjeta
- `Descripcion`: Descripción detallada
- `Prioridad`: "Baja", "Media" o "Alta"
- `Estado`: "Todo", "InProgress" o "Done"
- `Orden`: Posición en la lista
- `FechaCreacion`: Fecha de creación
- `FechaVencimiento`: Fecha límite (opcional)
- `ListaId`: ID de la lista contenedora
- `AsignadoAId`: ID del usuario asignado (opcional)

---

## 🔌 API Endpoints

### 🔐 Autenticación (`/api/auth`)

#### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@trello.com",
  "password": "admin123"
}
```

**Respuesta exitosa (200):**

```json
{
  "message": "Inicio de sesión exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "usuario": {
      "id": 1,
      "nombre": "Admin User",
      "email": "admin@trello.com",
      "rol": "Admin"
    }
  }
}
```

#### Registro

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "nuevo_usuario",
  "email": "nuevo@example.com",
  "password": "Password123",
  "confirmPassword": "Password123"
}
```

**Validaciones:**

- Email válido y único
- Username: 3-50 caracteres
- Password: Mínimo 6 caracteres, debe contener mayúscula, minúscula y número

**Respuesta exitosa (201):**

```json
{
  "message": "Usuario registrado correctamente",
  "data": {
    "id": 3,
    "nombre": "nuevo_usuario",
    "email": "nuevo@example.com",
    "rol": "User"
  }
}
```

#### Usuario Actual

```http
GET /api/auth/me
Authorization: Bearer {token}
```

**Respuesta (200):**

```json
{
  "message": "Usuario obtenido correctamente",
  "data": {
    "id": 1,
    "nombre": "Admin User",
    "email": "admin@trello.com",
    "rol": "Admin"
  }
}
```

---

### 📋 Tableros (`/api/tableros`)

#### Listar todos los tableros

```http
GET /api/tableros
Authorization: Bearer {token}
```

**Respuesta (200):**

```json
[
  {
    "id": 1,
    "titulo": "Proyecto Backend",
    "descripcion": "API REST",
    "color": "#EF4444",
    "fechaCreacion": "2025-11-12T00:00:00Z",
    "usuarioId": 1,
    "nombreUsuario": "Admin",
    "listas": [...]
  }
]
```

#### Obtener tablero por ID

```http
GET /api/tableros/{id}
Authorization: Bearer {token}
```

#### Crear tablero (Admin)

```http
POST /api/tableros
Authorization: Bearer {token}
Content-Type: application/json

{
  "titulo": "Nuevo Tablero",
  "descripcion": "Descripción del tablero",
  "color": "#3B82F6"
}
```

**Nota:** El `usuarioId` se obtiene automáticamente del token JWT.

#### Eliminar tablero (Admin)

```http
DELETE /api/tableros/{id}
Authorization: Bearer {token}
```

---

### 📝 Listas (`/api/tableros`)

#### Crear lista en un tablero (Admin)

```http
POST /api/tableros/{tableroId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "titulo": "Nueva Lista",
  "orden": 1
}
```

#### Reordenar listas (Admin)

```http
PATCH /api/tableros/{tableroId}/listas/reorder
Authorization: Bearer {token}
Content-Type: application/json

{
  "listaIds": [3, 1, 2, 4]
}
```

#### Eliminar lista (Admin)

```http
DELETE /api/tableros/{listaId}/listas/delete
Authorization: Bearer {token}
```

---

### 🎯 Tarjetas (`/api/tarjetas`)

#### Listar todas las tarjetas

```http
GET /api/tarjetas
Authorization: Bearer {token}

# Con filtros opcionales
GET /api/tarjetas?search=autenticación&estado=InProgress
```

**Parámetros de query:**

- `search`: Búsqueda por texto en título o descripción
- `estado`: Filtrar por estado (Todo, InProgress, Done)

#### Obtener tarjeta por ID

```http
GET /api/tarjetas/{id}
Authorization: Bearer {token}
```

#### Crear tarjeta (Admin)

```http
POST /api/tarjetas
Authorization: Bearer {token}
Content-Type: application/json

{
  "titulo": "Nueva Tarea",
  "descripcion": "Descripción detallada",
  "prioridad": "Alta",
  "listaId": 1,
  "fechaVencimiento": "2025-12-31T00:00:00Z",
  "asignadoAId": 2
}
```

#### Actualizar tarjeta (Admin)

```http
PUT /api/tarjetas/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "titulo": "Tarea Actualizada",
  "descripcion": "Nueva descripción",
  "prioridad": "Media",
  "listaId": 1,
  "fechaVencimiento": "2025-12-31T00:00:00Z",
  "asignadoAId": 2
}
```

#### Eliminar tarjeta (Admin)

```http
DELETE /api/tarjetas/{id}
Authorization: Bearer {token}
```

#### Mover tarjeta entre listas

```http
PATCH /api/tarjetas/{id}/mover
Authorization: Bearer {token}
Content-Type: application/json

{
  "listaDestinoId": 2,
  "nuevaPosicion": 0
}
```

---

### 📈 Administración (`/api/admin`)

#### Obtener estadísticas (Admin)

```http
GET /api/admin/estadisticas
Authorization: Bearer {token}
```

**Respuesta (200):**

```json
{
  "totalUsuarios": 5,
  "totalTableros": 3,
  "totalTarjetas": 15,
  "tarjetasPorEstado": [
    { "estado": "Todo", "cantidad": 5 },
    { "estado": "InProgress", "cantidad": 7 },
    { "estado": "Done", "cantidad": 3 }
  ],
  "tarjetasPorPrioridad": [
    { "prioridad": "Baja", "cantidad": 4 },
    { "prioridad": "Media", "cantidad": 8 },
    { "prioridad": "Alta", "cantidad": 3 }
  ]
}
```

---

### 🏥 Health Check

```http
GET /health
```

**Respuesta (200):**

```json
{
  "status": "Healthy",
  "timestamp": "2025-11-12T10:30:00Z",
  "environment": "Development"
}
```

---

## 👤 Credenciales de Prueba

La base de datos se inicializa automáticamente con usuarios de prueba:

### 👨‍💼 Usuario Administrador

```
Email: admin@trello.com
Password: admin123
Rol: Admin
```

**Permisos:**

- ✅ CRUD completo de tableros
- ✅ CRUD completo de listas
- ✅ CRUD completo de tarjetas
- ✅ Acceso a estadísticas
- ✅ Reordenar listas
- ✅ Eliminar cualquier elemento

### 👤 Usuario Regular

```
Email: user@trello.com
Password: user123
Rol: User
```

**Permisos:**

- ✅ Ver tableros y tarjetas
- ✅ Mover tarjetas entre listas
- ❌ No puede crear/editar/eliminar

---

## 🔐 Seguridad

### Autenticación JWT

1. **Login** en `/api/auth/login` con email y contraseña
2. **Guardar el token** recibido en la respuesta
3. **Incluir en headers** de todas las peticiones:
   ```
   Authorization: Bearer {token}
   ```
4. Los tokens **expiran en 24 horas**
5. Si el token expira, el header incluye `Token-Expired: true`

### Hash de Contraseñas

- Las contraseñas se hashean con **BCrypt** (factor 12)
- Nunca se almacenan en texto plano
- El hash es **irreversible**

### Roles y Permisos

| Endpoint                       | User | Admin |
| ------------------------------ | ---- | ----- |
| GET /api/tableros              | ✅   | ✅    |
| POST /api/tableros             | ❌   | ✅    |
| GET /api/tarjetas              | ✅   | ✅    |
| POST /api/tarjetas             | ❌   | ✅    |
| PATCH /api/tarjetas/{id}/mover | ✅   | ✅    |
| GET /api/admin/\*              | ❌   | ✅    |

### CORS

Configurado para permitir:

- `http://localhost:5173` (Vite)
- `http://localhost:5174`
- `http://localhost:3000` (React)

---

## 🗄️ Base de Datos

### Inicialización Automática

Al ejecutar por primera vez, el sistema:

1. ✅ Crea la base de datos automáticamente
2. ✅ Aplica todas las migraciones
3. ✅ Ejecuta el seed con datos de prueba:
   - 2 usuarios (admin y user)
   - 3 tableros
   - 8 listas
   - 15+ tarjetas de ejemplo

### Migraciones con Entity Framework

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion -p TrelloClone.Infrastructure -s TrelloClone.API

# Aplicar migraciones pendientes
dotnet ef database update -p TrelloClone.Infrastructure -s TrelloClone.API

# Ver migraciones aplicadas
dotnet ef migrations list -p TrelloClone.Infrastructure -s TrelloClone.API

# Eliminar última migración (si no se aplicó)
dotnet ef migrations remove -p TrelloClone.Infrastructure -s TrelloClone.API

# Eliminar base de datos
dotnet ef database drop -p TrelloClone.Infrastructure -s TrelloClone.API
```

### Reiniciar Base de Datos (desarrollo)

```bash
# Eliminar BD
dotnet ef database drop -f -p TrelloClone.Infrastructure -s TrelloClone.API

# Volver a ejecutar
cd TrelloClone.API
dotnet run
```

### Esquema de Base de Datos

```sql
Usuarios (1) ──────┐
                   │
                   ├─── (1:N) ──→ Tableros (N)
                   │                    │
                   │                    ├─── (1:N) ──→ Listas (N)
                   │                                       │
                   └─── (Asignado) ──→ Tarjetas (N) ─────┘
                                            (N:1)
```

---

## 🐛 Troubleshooting

### Error: "No se puede conectar a la base de datos"

```bash
# Verificar que SQL Server esté corriendo
# Windows: Services.msc → SQL Server (SQLEXPRESS)

# Verificar connection string en appsettings.json
# Para SQL Express:
Server=localhost\\SQLEXPRESS;Database=TrelloCloneDb;...

# Para LocalDB:
Server=(localdb)\\mssqllocaldb;Database=TrelloCloneDb;...
```

### Error: "JWT Key not configured"

```bash
# Verificar que appsettings.json tenga la clave JWT
# La clave debe tener MÍNIMO 32 caracteres
"Jwt": {
  "Key": "SuperSecretKeyForJwtTokenGeneration123456789"
}
```

### Error: "dotnet no reconocido"

```bash
# Asegurarse de que .NET SDK esté en el PATH
# Reiniciar la terminal después de instalar

# Verificar instalación
dotnet --version
```

### Puerto en uso

```bash
# Cambiar puerto en Properties/launchSettings.json
"applicationUrl": "http://localhost:5001"

# O en appsettings.json (si se configura manualmente)
```

### CORS Error en Frontend

```csharp
// Verificar que el origen del frontend esté en Program.cs
policy.WithOrigins("http://localhost:TU_PUERTO")
```

### Token JWT no válido

```bash
# Verificar que el token se envíe correctamente:
Authorization: Bearer eyJhbGci...

# NO:
Authorization: eyJhbGci...
Authorization: JWT eyJhbGci...
```

---

## 📚 Recursos Adicionales

- [Documentación .NET 9](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-9)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api)
- [JWT en ASP.NET Core](https://jwt.io)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

---

## 📝 Notas de Desarrollo

### Agregar nuevo endpoint

1. Crear DTO en `Application/DTOs`
2. Agregar método en interfaz (`Application/Interfaces`)
3. Implementar en servicio (`Infrastructure/Services`)
4. Crear endpoint en controller (`API/Controllers`)

### Agregar nueva entidad

1. Crear clase en `Domain/Entities`
2. Agregar DbSet en `ApplicationDbContext`
3. Configurar relaciones en `OnModelCreating` (si aplica)
4. Crear migración:
   ```bash
   dotnet ef migrations add NuevaEntidad -p TrelloClone.Infrastructure -s TrelloClone.API
   ```
5. Aplicar:
   ```bash
   dotnet ef database update -p TrelloClone.Infrastructure -s TrelloClone.API
   ```

### Agregar validaciones

```csharp
// En el DTO
public class MiDto
{
    [Required(ErrorMessage = "El campo es requerido")]
    [MaxLength(200, ErrorMessage = "Máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;
}
```

---

## 📄 Licencia

Este proyecto es parte de un trabajo académico para **Programación IV** en la **Tecnicatura Universitaria de Programación** de la **UTN FRSN** (Universidad Tecnológica Nacional - Facultad Regional San Nicolas).

---

## ✅ Checklist de Instalación

- [ ] .NET 9.0 SDK instalado
- [ ] SQL Server corriendo (SQLEXPRESS/LocalDB/Docker)
- [ ] Repositorio clonado
- [ ] Dependencias restauradas (`dotnet restore`)
- [ ] Connection string configurado correctamente
- [ ] JWT Key configurado (mínimo 32 caracteres)
- [ ] Proyecto ejecutándose (`dotnet run`)
- [ ] Swagger accesible en http://localhost:5000
- [ ] Login funcionando con `admin@trello.com` / `admin123`
- [ ] Base de datos creada con datos de prueba

**¡Proyecto listo! 🎉**

---

**Versión:** 1.0.0  
**Última actualización:** Noviembre 2025

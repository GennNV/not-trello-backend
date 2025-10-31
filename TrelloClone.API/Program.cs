// ============================================
// TrelloClone.API/Program.cs
// ============================================
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TrelloClone.Application.Interfaces;
using TrelloClone.Infrastructure.Services;
using TrelloClone.Infrastructure.Data;
using TrelloClone.Infraestructure.Services;
using TrelloClone.Infraestructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACIÓN DE SERVICIOS
// ============================================

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios (Inyección de Dependencias)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITarjetaService, TarjetaService>();

//Registrar repositorios
builder.Services.AddScoped<ITarjetaRepository, TarjetaRepository>();

// Configurar autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Eliminar delay de 5 minutos por defecto
    };

    // Logging de errores de autenticación (útil para debugging)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configurar CORS para permitir el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Agregar controladores
builder.Services.AddControllers();

// Configurar Swagger con soporte para JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TrelloClone API",
        Version = "v1",
        Description = "API REST para gestión de tableros y tarjetas tipo Trello",
        Contact = new OpenApiContact
        {
            Name = "TrelloClone Team",
            Email = "soporte@trelloclone.com"
        }
    });

    // Configurar autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando el esquema Bearer. 
                      Ingresa 'Bearer' [espacio] y luego tu token.
                      Ejemplo: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// ============================================
// INICIALIZAR BASE DE DATOS CON SEED
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Aplicar migraciones pendientes (si existen)
        context.Database.Migrate();

        // Seed de datos iniciales
        DbInitializer.Initialize(context);

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Base de datos inicializada correctamente");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

// ============================================
// CONFIGURAR PIPELINE HTTP
// ============================================

// Configurar Swagger en desarrollo y producción
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrelloClone API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz (http://localhost:5000)
        c.DocumentTitle = "TrelloClone API - Documentación";
    });
}

// Habilitar HTTPS redirect (opcional en desarrollo)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// IMPORTANTE: El orden importa
app.UseCors("AllowFrontend");      // 1. CORS primero

app.UseAuthentication();            // 2. Autenticación (leer token)
app.UseAuthorization();             // 3. Autorización (validar permisos)

app.MapControllers();               // 4. Mapear controladores

// Endpoint de salud (health check)
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}))
.WithName("HealthCheck")
.WithOpenApi();

// Logging de inicio
app.Logger.LogInformation("TrelloClone API iniciada");
app.Logger.LogInformation("Swagger UI disponible en: http://localhost:5000");
app.Logger.LogInformation("JWT configurado correctamente");

app.Run();

// ============================================
// NOTAS:
// ============================================
// 1. La base de datos se crea automáticamente con EnsureCreated()
// 2. Si quieres usar migraciones, cambia EnsureCreated() por Migrate()
// 3. El seed se ejecuta solo si la DB está vacía
// 4. Swagger está en la raíz: http://localhost:5000
// 5. CORS permite localhost:5173 y localhost:3000
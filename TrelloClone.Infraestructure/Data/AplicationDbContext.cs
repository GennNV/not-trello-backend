using Microsoft.EntityFrameworkCore;
using TrelloClone.Domain.Entities;

namespace TrelloClone.Infraestructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Tablero> Tableros { get; set; }
    public DbSet<Lista> Listas { get; set; }
    public DbSet<Tarjeta> Tarjetas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Rol).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");
        });

        // Configuración de Tablero
        modelBuilder.Entity<Tablero>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.Color).HasMaxLength(7).HasDefaultValue("#0079BF");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Tableros)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Lista
        modelBuilder.Entity<Lista>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Orden).IsRequired();

            entity.HasOne(e => e.Tablero)
                .WithMany(t => t.Listas)
                .HasForeignKey(e => e.TableroId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Tarjeta
        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(2000);
            entity.Property(e => e.Prioridad).HasMaxLength(20).HasDefaultValue("Media");
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Todo");
            entity.Property(e => e.Orden).IsRequired();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Lista)
                .WithMany(l => l.Tarjetas)
                .HasForeignKey(e => e.ListaId)
                .OnDelete(DeleteBehavior.Restrict);


            entity.HasOne(e => e.AsignadoA)
                .WithMany()
                .HasForeignKey(e => e.AsignadoAId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.AsignadoA)
                .WithMany()
                .HasForeignKey(e => e.AsignadoAId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }
}
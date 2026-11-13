using Microsoft.EntityFrameworkCore;
using BiciTaller.Domain.Core;
using BiciTaller.Domain.Entities;

namespace BiciTaller.Infrastructure.Context;

public sealed class BiciTallerContext(DbContextOptions<BiciTallerContext> options)
    : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Persona>()
            .HasDiscriminator<string>("TipoPersona")
            .HasValue<Cliente>("Cliente")
            .HasValue<Tecnico>("Tecnico");
        modelBuilder.Entity<Persona>()
            .HasIndex(persona => persona.Correo)
            .IsUnique();

        modelBuilder.Entity<Categoria>()
            .HasIndex(categoria => categoria.Codigo)
            .IsUnique();
        modelBuilder.Entity<Categoria>()
            .Property(categoria => categoria.Codigo)
            .HasMaxLength(20)
            .IsRequired();
        modelBuilder.Entity<Categoria>()
            .Property(categoria => categoria.Nombre)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<Servicio>()
            .HasIndex(servicio => servicio.Codigo)
            .IsUnique();
        modelBuilder.Entity<Servicio>()
            .HasOne(servicio => servicio.Categoria)
            .WithMany(categoria => categoria.Servicios)
            .HasForeignKey(servicio => servicio.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Servicio>()
            .HasOne(servicio => servicio.Tecnico)
            .WithMany(tecnico => tecnico.Servicios)
            .HasForeignKey(servicio => servicio.TecnicoId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Servicio>()
            .HasMany(servicio => servicio.Clientes)
            .WithMany(cliente => cliente.Servicios)
            .UsingEntity("ClienteServicios");

        modelBuilder.Entity<Cliente>()
            .HasIndex(cliente => cliente.NumeroCliente)
            .IsUnique();
        modelBuilder.Entity<Cliente>()
            .HasOne(cliente => cliente.Categoria)
            .WithMany(categoria => categoria.Clientes)
            .HasForeignKey(cliente => cliente.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tecnico>()
            .HasIndex(tecnico => tecnico.NumeroEmpleado)
            .IsUnique();
        modelBuilder.Entity<Tecnico>()
            .HasOne(tecnico => tecnico.Categoria)
            .WithMany(categoria => categoria.Tecnicos)
            .HasForeignKey(tecnico => tecnico.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkUpdated();
            }
        }
    }
}

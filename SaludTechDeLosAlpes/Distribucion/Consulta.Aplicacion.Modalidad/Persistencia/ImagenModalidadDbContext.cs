using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Consulta.Aplicacion.Modalidad.Persistencia;

public class ImagenModalidadDbContext : DbContext
{
    public ImagenModalidadDbContext(DbContextOptions<ImagenModalidadDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString ?? "Host=localhost;Database=SaludTechModalidadDb;Username=postgres;Password=postgres");
        }
    }

    public DbSet<ImagenModalidadEntity> ImagenesModalidad { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImagenModalidadEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImagenId).IsRequired();
            entity.HasIndex(e => e.ImagenId).IsUnique();
            entity.Property(e => e.Nombre).IsRequired();
            entity.Property(e => e.Descripcion).IsRequired();
            entity.Property(e => e.RegionAnatomica).IsRequired();
            entity.Property(e => e.RegionDescripcion).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
        });
    }
}

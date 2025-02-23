using Consulta.Aplicacion.Demografia.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Consulta.Aplicacion.Demografia.Persistencia;

public class ImagenDemografiaDbContext : DbContext
{
    public DbSet<ImagenDemografiaEntity> ImagenesDemograficas { get; set; }

    public ImagenDemografiaDbContext(DbContextOptions<ImagenDemografiaDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString ?? "Host=localhost;Database=SaludTechDemografiaDb;Username=postgres;Password=postgres");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImagenDemografiaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImagenId).IsRequired();
            entity.HasIndex(e => e.ImagenId).IsUnique();
            entity.Property(e => e.GrupoEdad);
            entity.Property(e => e.Sexo);
            entity.Property(e => e.Etnicidad);
            
            // Create GIN indexes for text columns
            entity.HasIndex(e => e.GrupoEdad).HasMethod("GIN").HasOperators("gin_trgm_ops");
            entity.HasIndex(e => e.Sexo).HasMethod("GIN").HasOperators("gin_trgm_ops");
            entity.HasIndex(e => e.Etnicidad).HasMethod("GIN").HasOperators("gin_trgm_ops");
        });
            
    }
}

using Ingestion.Aplicacion.Anonimizacion.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Aplicacion.Anonimizacion.Persistencia
{
    public class AnonimizarDbContext : DbContext
    {

        public AnonimizarDbContext()
        {
            
        }
        public AnonimizarDbContext(DbContextOptions<AnonimizarDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString ?? "Host=localhost;Database=SaludTechAnonimizarDb;Username=postgres;Password=postgres");
            }
        }

        public DbSet<ImagenAnonimizadaEntity> AnonimizacionImagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImagenAnonimizadaEntity>(entity =>
            {
                entity.ToTable("AnonimizacionImagenes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Version).IsRequired();
                entity.Property(e => e.ImagenId).IsRequired();
                entity.Property(e => e.DetalleAnonimizacion).IsRequired();
                entity.Property(e => e.NombreModalidad);
                entity.Property(e => e.DescripcionModalidad);
                entity.Property(e => e.RegionAnatomica);
                entity.Property(e => e.DescripcionRegionAnatomica);
                entity.Property(e => e.Resolucion);
                entity.Property(e => e.Contraste);
                entity.Property(e => e.Es3D);
                entity.Property(e => e.FaseEscaner);
                entity.Property(e => e.UbicacionImagen);
                entity.Property(e => e.UbicacionImagenProcesada);
                entity.Property(e => e.Timestamp);
            });
        }
    }
}
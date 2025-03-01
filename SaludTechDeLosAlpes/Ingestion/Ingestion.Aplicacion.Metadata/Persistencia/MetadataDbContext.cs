using Ingestion.Aplicacion.Metadata.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Aplicacion.Metadata.Persistencia;

public class MetadataDbContext : DbContext
{
	public MetadataDbContext()
	{
		
	}
	
	public MetadataDbContext(DbContextOptions<MetadataDbContext> options) : base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
		{
			var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection");
			optionsBuilder.UseNpgsql(connectionString ??
									 "Host=localhost;Database=SaludTechMetadataDb;Username=postgres;Password=postgres");
		}
	}

	public DbSet<MetadataImagenEntity> Imagenes { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MetadataImagenEntity>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.ImagenId).IsRequired();
			entity.HasIndex(e => e.ImagenId).IsUnique();
			entity.Property(e => e.Version);
			entity.Property(e => e.VersionServicio);
			entity.Property(e => e.NombreModalidad);
			entity.Property(e => e.DescripcionModalidad);
			entity.Property(e => e.RegionAnatomica);
			entity.Property(e => e.DescripcionRegionAnatomica);
			entity.Property(e => e.DescripcionPatologia);
			entity.Property(e => e.Resolucion);
			entity.Property(e => e.Contraste);
			entity.Property(e => e.Es3D);
			entity.Property(e => e.FaseEscaner);
			entity.Property(e => e.EtapaContextoProcesal);
			entity.Property(e => e.GrupoEdad);
			entity.Property(e => e.Sexo);
			entity.Property(e => e.Etnicidad);
			entity.Property(e => e.Fumador);
			entity.Property(e => e.Diabetico);
			entity.Property(e => e.CondicionesPrevias);
			entity.Property(e => e.TipoAmbiente);
			entity.Property(e => e.Sintomas);
			entity.Property(e => e.Timestamp);
		});
		
	}
}
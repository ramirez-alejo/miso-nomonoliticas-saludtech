using Ingestion.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Infraestructura.Persistencia;

public class ImagenDbContext : DbContext
{
    public ImagenDbContext(DbContextOptions<ImagenDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString ?? "Host=localhost;Database=SaludTechDb;Username=postgres;Password=postgres");
        }
    }

    public DbSet<ImagenEntity> Imagenes { get; set; }
    public DbSet<TipoImagenEntity> TiposImagen { get; set; }
    public DbSet<ModalidadEntity> Modalidades { get; set; }
    public DbSet<RegionAnatomicaEntity> RegionesAnatomicas { get; set; }
    public DbSet<PatologiaEntity> Patologias { get; set; }
    public DbSet<AtributosImagenEntity> AtributosImagen { get; set; }
    public DbSet<ContextoProcesalEntity> ContextosProcesales { get; set; }
    public DbSet<MetadatosEntity> Metadatos { get; set; }
    public DbSet<PacienteEntity> Pacientes { get; set; }
    public DbSet<DemografiaEntity> Demografias { get; set; }
    public DbSet<HistorialEntity> Historiales { get; set; }
    public DbSet<EntornoClinicoEntity> EntornosClinicos { get; set; }
    public DbSet<SintomaEntity> Sintomas { get; set; }
    public DbSet<ImagenAnonimizadaEntity> ImagenAnonimizadas { get; set; }
    public DbSet<MetadatoGeneradosEntity> MetadatosGenerados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Imagen configuration
        modelBuilder.Entity<ImagenEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRequired();
            entity.HasOne(e => e.TipoImagen).WithMany().IsRequired();
            entity.HasOne(e => e.AtributosImagen).WithMany().IsRequired();
            entity.HasOne(e => e.ContextoProcesal).WithMany();
            entity.HasOne(e => e.Metadatos).WithMany();
            entity.HasOne(e => e.Paciente).WithMany();
            entity.HasOne(e => e.EntornoClinico).WithMany();
        });

        // TipoImagen configuration
        modelBuilder.Entity<TipoImagenEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Modalidad).WithMany().IsRequired();
            entity.HasOne(e => e.RegionAnatomica).WithMany().IsRequired();
            entity.HasOne(e => e.Patologia).WithMany();
        });

        // Modalidad configuration
        modelBuilder.Entity<ModalidadEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
            entity.Property(e => e.Descripcion);
        });

        // RegionAnatomica configuration
        modelBuilder.Entity<RegionAnatomicaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
            entity.Property(e => e.Descripcion);
        });

        // Patologia configuration
        modelBuilder.Entity<PatologiaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).IsRequired();
        });

        // AtributosImagen configuration
        modelBuilder.Entity<AtributosImagenEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Resolucion).IsRequired();
            entity.Property(e => e.Contraste).IsRequired();
            entity.Property(e => e.Es3D).IsRequired();
            entity.Property(e => e.FaseEscaner).IsRequired();
        });

        // ContextoProcesal configuration
        modelBuilder.Entity<ContextoProcesalEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Etapa).IsRequired();
        });

        // Metadatos configuration
        modelBuilder.Entity<MetadatosEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.EntornoClinico).WithMany();
            entity.HasMany(e => e.Sintomas)
                  .WithMany(e => e.Metadatos)
                  .UsingEntity(j => j.ToTable("MetadatosSintomas"));
        });

        // Paciente configuration
        modelBuilder.Entity<PacienteEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TokenAnonimo).IsRequired();
            entity.HasOne(e => e.Demografia).WithMany().IsRequired();
            entity.HasOne(e => e.Historial).WithMany().IsRequired();
        });

        // Demografia configuration
        modelBuilder.Entity<DemografiaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GrupoEdad).IsRequired();
            entity.Property(e => e.Sexo).IsRequired();
            entity.Property(e => e.Etnicidad).IsRequired();
        });

        // Historial configuration
        modelBuilder.Entity<HistorialEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fumador).IsRequired();
            entity.Property(e => e.Diabetico).IsRequired();
            entity.Property(e => e.CondicionesPrevias).IsRequired();
        });

        // EntornoClinico configuration
        modelBuilder.Entity<EntornoClinicoEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoAmbiente).IsRequired();
        });

        // Sintoma configuration
        modelBuilder.Entity<SintomaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).IsRequired();
        });
        
        // ImagenAnonimizada configuration
        modelBuilder.Entity<ImagenAnonimizadaEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImagenProcesadaPath).IsRequired();
            entity.HasOne(e => e.Imagen)
                .WithMany()
                .HasForeignKey(e => e.ImagenId)
                .IsRequired();

        });
        
        // MetadatoGenerados configuration
        modelBuilder.Entity<MetadatoGeneradosEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Imagen)
                .WithMany()
                .HasForeignKey(e => e.ImagenId)
                .IsRequired();
            entity.Property(e => e.Key).IsRequired();
            entity.Property(e => e.Value).IsRequired();
        });
    }
}

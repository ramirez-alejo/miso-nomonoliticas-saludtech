namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class ImagenEntity
{
    public Guid Id { get; set; }
    public string Version { get; set; }
    public TipoImagenEntity TipoImagen { get; set; }
    public AtributosImagenEntity AtributosImagen { get; set; }
    public ContextoProcesalEntity ContextoProcesal { get; set; }
    public MetadatosEntity Metadatos { get; set; }
    public PacienteEntity Paciente { get; set; }
    public EntornoClinicoEntity EntornoClinico { get; set; }
}

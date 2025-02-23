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

public class TipoImagenEntity
{
    public Guid Id { get; set; }
    public ModalidadEntity Modalidad { get; set; }
    public RegionAnatomicaEntity RegionAnatomica { get; set; }
    public PatologiaEntity Patologia { get; set; }
}

public class ModalidadEntity
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
}

public class RegionAnatomicaEntity
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
}

public class PatologiaEntity
{
    public Guid Id { get; set; }
    public string Descripcion { get; set; }
}

public class AtributosImagenEntity
{
    public Guid Id { get; set; }
    public string Resolucion { get; set; }
    public string Contraste { get; set; }
    public bool Es3D { get; set; }
    public string FaseEscaner { get; set; }
}

public class ContextoProcesalEntity
{
    public Guid Id { get; set; }
    public string Etapa { get; set; }
}

public class MetadatosEntity
{
    public Guid Id { get; set; }
    public EntornoClinicoEntity EntornoClinico { get; set; }
    public ICollection<SintomaEntity> Sintomas { get; set; } = new List<SintomaEntity>();
}

public class PacienteEntity
{
    public Guid Id { get; set; }
    public DemografiaEntity Demografia { get; set; }
    public HistorialEntity Historial { get; set; }
    public string TokenAnonimo { get; set; }
}

public class DemografiaEntity
{
    public Guid Id { get; set; }
    public string GrupoEdad { get; set; }
    public string Sexo { get; set; }
    public string Etnicidad { get; set; }
}

public class HistorialEntity
{
    public Guid Id { get; set; }
    public bool Fumador { get; set; }
    public bool Diabetico { get; set; }
    public string CondicionesPrevias { get; set; }
}

public class EntornoClinicoEntity
{
    public Guid Id { get; set; }
    public string TipoAmbiente { get; set; }
}

public class SintomaEntity
{
    public Guid Id { get; set; }
    public string Descripcion { get; set; }
    public ICollection<MetadatosEntity> Metadatos { get; set; } = new List<MetadatosEntity>();
}

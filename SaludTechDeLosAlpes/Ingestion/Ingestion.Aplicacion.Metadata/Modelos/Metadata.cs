using Core.Dominio;

namespace Ingestion.Aplicacion.Metadata.Modelos;

public class Metadata
{
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public string Resolucion { get; set; }
	public string Contraste { get; set; }
	public bool Es3D { get; set; }
	public string FaseEscaner { get; set; }
	public ContextoProcesal ContextoProcesal { get; set; }
	public Demografia Demografia { get; set; }
	public Historial Historial { get; set; }
	public EntornoClinico EntornoClinico { get; set; }
	public List<Sintoma> Sintomas { get; set; } = new List<Sintoma>();
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	public Dictionary<string,string> Tags { get; set; }
	
	
	public void GenerarTags()
	{
	    Tags = new Dictionary<string, string>();
	
	    if (!string.IsNullOrEmpty(Version))
	    {
	        Tags["Version"] = Version;
	    }
	
	    if (ImagenId != Guid.Empty)
	    {
	        Tags["ImagenId"] = ImagenId.ToString();
	    }
	
	    if (TipoImagen != null)
	    {
	        if (TipoImagen.Modalidad != null)
	        {
	            if (!string.IsNullOrEmpty(TipoImagen.Modalidad.Nombre))
	            {
	                Tags["TipoImagen.Modalidad.Nombre"] = TipoImagen.Modalidad.Nombre;
	            }
	            if (!string.IsNullOrEmpty(TipoImagen.Modalidad.Descripcion))
	            {
	                Tags["TipoImagen.Modalidad.Descripcion"] = TipoImagen.Modalidad.Descripcion;
	            }
	        }
	        if (TipoImagen.RegionAnatomica != null)
	        {
	            if (!string.IsNullOrEmpty(TipoImagen.RegionAnatomica.Nombre))
	            {
	                Tags["TipoImagen.RegionAnatomica.Nombre"] = TipoImagen.RegionAnatomica.Nombre;
	            }
	            if (!string.IsNullOrEmpty(TipoImagen.RegionAnatomica.Descripcion))
	            {
	                Tags["TipoImagen.RegionAnatomica.Descripcion"] = TipoImagen.RegionAnatomica.Descripcion;
	            }
	        }
	        if (TipoImagen.Patologia != null)
	        {
	            if (!string.IsNullOrEmpty(TipoImagen.Patologia.Descripcion))
	            {
	                Tags["TipoImagen.Patologia.Descripcion"] = TipoImagen.Patologia.Descripcion;
	            }
	        }
	    }
	
	    if (AtributosImagen != null)
	    {
	        if (!string.IsNullOrEmpty(AtributosImagen.Resolucion))
	        {
	            Tags["AtributosImagen.Resolucion"] = AtributosImagen.Resolucion;
	        }
	        if (!string.IsNullOrEmpty(AtributosImagen.Contraste))
	        {
	            Tags["AtributosImagen.Contraste"] = AtributosImagen.Contraste;
	        }
	        if (AtributosImagen.Es3D)
	        {
	            Tags["AtributosImagen.Es3D"] = AtributosImagen.Es3D.ToString();
	        }
	        if (!string.IsNullOrEmpty(AtributosImagen.FaseEscaner))
	        {
	            Tags["AtributosImagen.FaseEscaner"] = AtributosImagen.FaseEscaner;
	        }
	    }
	
	    if (!string.IsNullOrEmpty(Resolucion))
	    {
	        Tags["Resolucion"] = Resolucion;
	    }
	
	    if (!string.IsNullOrEmpty(Contraste))
	    {
	        Tags["Contraste"] = Contraste;
	    }
	
	    if (Es3D)
	    {
	        Tags["Es3D"] = Es3D.ToString();
	    }
	
	    if (!string.IsNullOrEmpty(FaseEscaner))
	    {
	        Tags["FaseEscaner"] = FaseEscaner;
	    }
	
	    if (ContextoProcesal != null)
	    {
	        if (!string.IsNullOrEmpty(ContextoProcesal.Etapa))
	        {
	            Tags["ContextoProcesal.Etapa"] = ContextoProcesal.Etapa;
	        }
	    }
	
	    if (Demografia != null)
	    {
	        if (!string.IsNullOrEmpty(Demografia.GrupoEdad))
	        {
	            Tags["Demografia.GrupoEdad"] = Demografia.GrupoEdad;
	        }
	        if (!string.IsNullOrEmpty(Demografia.Sexo))
	        {
	            Tags["Demografia.Sexo"] = Demografia.Sexo;
	        }
	        if (!string.IsNullOrEmpty(Demografia.Etnicidad))
	        {
	            Tags["Demografia.Etnicidad"] = Demografia.Etnicidad;
	        }
	    }
	
	    if (Historial != null)
	    {
	        Tags["Historial.Fumador"] = Historial.Fumador.ToString();
	        Tags["Historial.Diabetico"] = Historial.Diabetico.ToString();
	        if (Historial.CondicionesPrevias != null && Historial.CondicionesPrevias.Count > 0)
	        {
	            Tags["Historial.CondicionesPrevias"] = string.Join(", ", Historial.CondicionesPrevias);
	        }
	    }
	
	    if (EntornoClinico != null)
	    {
	        if (!string.IsNullOrEmpty(EntornoClinico.TipoAmbiente))
	        {
	            Tags["EntornoClinico.TipoAmbiente"] = EntornoClinico.TipoAmbiente;
	        }
	    }
	
	    if (Sintomas != null && Sintomas.Count > 0)
	    {
	        Tags["Sintomas"] = string.Join(", ", Sintomas.Select(s => s.Descripcion));
	    }
	}
	
}
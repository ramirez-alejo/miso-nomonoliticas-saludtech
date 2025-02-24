namespace Consulta.Aplicacion.Dtos;

// Siguiendo el principio de caminos separados, tenemos aca una copia que nos permite deserializar la respuesta del datawahrehouse
public class ImagenDto
{
	public Guid Id { get; set; }
	public string Version { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public ContextoProcesal ContextoProcesal { get; set; }
	public Metadatos Metadatos { get; set; }	
	public Paciente Paciente { get; set; }
}

public class TipoImagen
{
	public Modalidad Modalidad { get; set; }
	public RegionAnatomica RegionAnatomica { get; set; }
	public Patologia Patologia { get; set; }
}

public class Modalidad
{
	public string Nombre { get; set; }
	public string Descripcion { get; set; }
}

public class RegionAnatomica
{
	// E.g., "Cabeza y cuello", "Tórax", "Abdomen", etc.
	public string Nombre { get; set; }
	public string Descripcion { get; set; }
}

public class Patologia
{
	// E.g., "Normal vs Anormal", "Benigno vs Maligno", or specific conditions like "Neumonía"
	public string Descripcion { get; set; }
}

public class AtributosImagen
{
	public string Resolucion { get; set; }
	public string Contraste { get; set; }
	public bool Es3D { get; set; }
	// E.g., "pre-tratamiento", "post-tratamiento", "seguimiento"
	public string FaseEscaner { get; set; }
}

public class ContextoProcesal
{
	// E.g., "Pre-operatorio", "Post-operatorio", "Examen de rutina"
	public string Etapa { get; set; }
}

public class Metadatos
{
	public EntornoClinico EntornoClinico { get; set; }
	public List<Sintoma> Sintomas { get; set; } = new List<Sintoma>();
}

public class Paciente
{
	public Demografia Demografia { get; set; }
	public Historial Historial { get; set; }
	public string TokenAnonimo { get; set; }
}

public class Demografia
{
	// E.g., "Neonatal", "Pediátrico", "Adulto", "Geriátrico"
	public string GrupoEdad { get; set; }

	// E.g., "Masculino", "Femenino", "Intersexual"
	public string Sexo { get; set; }

	// E.g., "Latino", "Caucásico", "Afro", "Asiático"
	public string Etnicidad { get; set; }
}

public class Historial
{
	public bool Fumador { get; set; }
	public bool Diabetico { get; set; }
	public List<string> CondicionesPrevias { get; set; } = new List<string>();
}

public class EntornoClinico
{
	// E.g., "Ambulatorio", "Interno", "UCI", etc.
	public string TipoAmbiente { get; set; }
}

public class Sintoma
{
	// E.g., "Fiebre", "Dolor", "Tos", etc.
	public string Descripcion { get; set; }
}
namespace Consulta.Aplicacion.Demografia.Dtos;

public class ImagenDemografia
{
	public Guid ImagenId { get; set; }
	// E.g., "Neonatal", "Pediátrico", "Adulto", "Geriátrico"
	public string GrupoEdad { get; set; }

	// E.g., "Masculino", "Femenino", "Intersexual"
	public string Sexo { get; set; }

	// E.g., "Latino", "Caucásico", "Afro", "Asiático"
	public string Etnicidad { get; set; }
}
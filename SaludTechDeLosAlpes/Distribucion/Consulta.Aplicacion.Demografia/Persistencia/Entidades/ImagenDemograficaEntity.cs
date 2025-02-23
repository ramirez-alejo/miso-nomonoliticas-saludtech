namespace Consulta.Aplicacion.Demografia.Persistencia.Entidades;

public class ImagenDemografiaEntity
{
    public Guid Id { get; set; }
    public Guid ImagenId { get; set; }
    public string GrupoEdad { get; set; }
    public string Sexo { get; set; }
    public string Etnicidad { get; set; }
}

namespace Ingestion.Aplicacion.Anonimizacion.Persistencia.Entidades
{
	public class ImagenAnonimizadaEntity
	{
		public Guid Id { get; set; }
		public string Version { get; set; }
		public Guid ImagenId { get; set; }
		public string DetalleAnonimizacion { get; set; }
		public string NombreModalidad { get; set; }
		public string DescripcionModalidad { get; set; }
		public string RegionAnatomica { get; set; }
		public string DescripcionRegionAnatomica { get; set; }
		public string Resolucion { get; set; }
		public string Contraste { get; set; }
		public bool Es3D { get; set; }
		public string FaseEscaner { get; set; }
		public string UbicacionImagen { get; set; }
		public string UbicacionImagenProcesada { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}

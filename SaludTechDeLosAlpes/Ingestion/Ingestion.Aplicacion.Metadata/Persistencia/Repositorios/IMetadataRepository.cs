namespace Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;

public interface IMetadataRepository
{
	 Task InsertMetadataGenerada(Modelos.Metadata modelo, CancellationToken cancellationToken);
	 
	 Task DeleteMetadataGenerada(Guid imagenId, CancellationToken cancellationToken);
}

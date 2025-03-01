namespace Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;

public interface IMetadataRepository
{
	 Task UpsertMetadataGenerada(Modelos.Metadata modelo, CancellationToken cancellationToken);
}
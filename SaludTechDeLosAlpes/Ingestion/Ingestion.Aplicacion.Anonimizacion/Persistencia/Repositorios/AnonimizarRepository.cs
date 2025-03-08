using Ingestion.Aplicacion.Anonimizacion.Mapeo;
using Ingestion.Dominio.Comandos;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios
{
    public class AnonimizarRepository : IAnonimizarRepository
    {
        private readonly AnonimizarDbContext _context;
        private readonly ILogger<AnonimizarRepository> _logger;

        public AnonimizarRepository(AnonimizarDbContext context, ILogger<AnonimizarRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        public async Task SaveAsync(Modelos.Anonimizar anonimizar,
            CancellationToken cancellationToken)
        {

            var entity = MapeoAnonimizarImagen.MapFromModel(anonimizar);
            await _context.AnonimizacionImagenes.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        public async Task DeleteAsync(Guid imagenId, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.AnonimizacionImagenes
                    .FirstOrDefaultAsync(x => x.ImagenId == imagenId, cancellationToken);
                
                if (entity != null)
                {
                    _context.AnonimizacionImagenes.Remove(entity);
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Deleted anonimization data for imagen {ImagenId}", imagenId);
                }
                else
                {
                    _logger.LogWarning("No anonimization data found for imagen {ImagenId} to delete", imagenId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting anonimization data for imagen {ImagenId}", imagenId);
                throw;
            }
        }
    }
}

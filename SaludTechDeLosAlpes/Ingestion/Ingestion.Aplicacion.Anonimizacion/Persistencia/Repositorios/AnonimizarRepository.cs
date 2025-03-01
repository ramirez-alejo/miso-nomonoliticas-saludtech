using Ingestion.Aplicacion.Anonimizacion.Mapeo;
using Ingestion.Dominio.Comandos;

namespace Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios
{
    public class AnonimizarRepository : IAnonimizarRepository
    {
        private readonly AnonimizarDbContext _context;

        public AnonimizarRepository(AnonimizarDbContext context)
        {
            _context = context;
        }

        
        public async Task SaveAsync(Modelos.Anonimizar anonimizar,
            CancellationToken cancellationToken)
        {

            var entity = MapeoAnonimizarImagen.MapFromModel(anonimizar);
            await _context.AnonimizacionImagenes.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
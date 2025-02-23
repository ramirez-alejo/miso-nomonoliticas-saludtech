using Core.Dominio;
using Ingestion.Infraestructura.Mapeo;
using Ingestion.Infraestructura.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Infraestructura.Persistencia.Repositorios;

public class ImagenRepository(ImagenDbContext context) : IImagenRepository
{
    public async Task<Imagen> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var imagen = await context.Imagenes
            .Include(i => i.TipoImagen)
            .ThenInclude(t => t.Modalidad)
            .Include(i => i.TipoImagen)
            .ThenInclude(t => t.RegionAnatomica)
            .Include(i => i.TipoImagen)
            .ThenInclude(t => t.Patologia)
            .Include(i => i.AtributosImagen)
            .Include(i => i.ContextoProcesal)
            .Include(i => i.Metadatos)
            .ThenInclude(m => m.EntornoClinico)
            .Include(i => i.Metadatos)
            .ThenInclude(m => m.Sintomas)
            .Include(i => i.Paciente)
            .ThenInclude(p => p.Demografia)
            .Include(i => i.Paciente)
            .ThenInclude(p => p.Historial)
            .Include(i => i.EntornoClinico)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken: cancellationToken);

        return MapeoImagen.MapearImagenEntityADominio(imagen);

    }

    public async Task<Imagen> AddAsync(Imagen imagen, CancellationToken cancellationToken)
    {
        var imagenEntity = MapeoImagen.MapearImagenDominioAEntity(imagen);

        await context.Imagenes.AddAsync(imagenEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        imagen.Id = imagenEntity.Id;
        return imagen;
    }

    public async Task UpdateAsync(Imagen imagen, CancellationToken cancellationToken)
    {
        var exists = await context.Imagenes.AnyAsync(i => i.Id == imagen.Id, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException($"Imagen with id {imagen.Id} not found");
        }
        
        var imagenEntity = await context.Imagenes
            .Include(i => i.TipoImagen)
            .ThenInclude(t => t.Modalidad)
            .Include(i => i.TipoImagen)
            .ThenInclude(t => t.RegionAnatomica)
            .Include(i => i.TipoImagen)
            .ThenInclude(t => t.Patologia)
            .Include(i => i.AtributosImagen)
            .Include(i => i.ContextoProcesal)
            .Include(i => i.Metadatos)
            .ThenInclude(m => m.EntornoClinico)
            .Include(i => i.Metadatos)
            .ThenInclude(m => m.Sintomas)
            .Include(i => i.Paciente)
            .ThenInclude(p => p.Demografia)
            .Include(i => i.Paciente)
            .ThenInclude(p => p.Historial)
            .Include(i => i.EntornoClinico)
            .FirstOrDefaultAsync(i => i.Id == imagen.Id, cancellationToken: cancellationToken);
        
        imagenEntity.Version = imagen.Version;
        
        imagenEntity.TipoImagen.Modalidad.Nombre = imagen.TipoImagen.Modalidad.Nombre;
        imagenEntity.TipoImagen.Modalidad.Descripcion = imagen.TipoImagen.Modalidad.Descripcion;
        imagenEntity.TipoImagen.RegionAnatomica.Nombre = imagen.TipoImagen.RegionAnatomica.Nombre;
        imagenEntity.TipoImagen.RegionAnatomica.Descripcion = imagen.TipoImagen.RegionAnatomica.Descripcion;
        imagenEntity.TipoImagen.Patologia.Descripcion = imagen.TipoImagen.Patologia.Descripcion;

        imagenEntity.AtributosImagen.Resolucion = imagen.AtributosImagen.Resolucion;
        imagenEntity.AtributosImagen.Contraste = imagen.AtributosImagen.Contraste;
        imagenEntity.AtributosImagen.Es3D = imagen.AtributosImagen.Es3D;
        imagenEntity.AtributosImagen.FaseEscaner = imagen.AtributosImagen.FaseEscaner;

        imagenEntity.ContextoProcesal.Etapa = imagen.ContextoProcesal.Etapa;

        imagenEntity.Metadatos.EntornoClinico.TipoAmbiente = imagen.Metadatos.EntornoClinico.TipoAmbiente;
        imagenEntity.Metadatos.Sintomas = imagen.Metadatos.Sintomas.Select(s => new SintomaEntity
        {
            Descripcion = s.Descripcion
        }).ToList();

        imagenEntity.Paciente.Demografia.GrupoEdad = imagen.Paciente.Demografia.GrupoEdad;
        imagenEntity.Paciente.Demografia.Sexo = imagen.Paciente.Demografia.Sexo;
        imagenEntity.Paciente.Demografia.Etnicidad = imagen.Paciente.Demografia.Etnicidad;
        imagenEntity.Paciente.Historial.Fumador = imagen.Paciente.Historial.Fumador;
        imagenEntity.Paciente.Historial.Diabetico = imagen.Paciente.Historial.Diabetico;
        imagenEntity.Paciente.Historial.CondicionesPrevias = string.Join(',', imagen.Paciente.Historial.CondicionesPrevias);
        imagenEntity.Paciente.TokenAnonimo = imagen.Paciente.TokenAnonimo;

        await context.SaveChangesAsync(cancellationToken);
        
        
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var imagen = await context.Imagenes.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (imagen != null)
        {
            context.Imagenes.Remove(imagen);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Imagen>> GetByDemografiaAsync(string grupoEdad, string sexo, string etnicidad, CancellationToken cancellationToken)
    {
        var imagenes = await context.Imagenes
            .Include(i => i.TipoImagen)
                .ThenInclude(t => t.Modalidad)
            .Include(i => i.TipoImagen)
                .ThenInclude(t => t.RegionAnatomica)
            .Where(i => i.Paciente.Demografia.GrupoEdad == grupoEdad &&
                        i.Paciente.Demografia.Sexo == sexo &&
                        i.Paciente.Demografia.Etnicidad == etnicidad)
            .Take(1000)
            .ToListAsync(cancellationToken: cancellationToken);
        
        return imagenes.Select(MapeoImagen.MapearImagenEntityADominio);
    }

    public async Task<IEnumerable<Imagen>> GetByModalidadAsync(string modalidad, CancellationToken cancellationToken)
    {
        var imagenes = await context.Imagenes
            .Include(i => i.TipoImagen)
                .ThenInclude(t => t.Modalidad)
            .Include(i => i.TipoImagen)
                .ThenInclude(t => t.RegionAnatomica)
            .Where(i => i.TipoImagen.Modalidad.Nombre == modalidad)
            .Take(1000)
            .ToListAsync(cancellationToken: cancellationToken);
        
        return imagenes.Select(MapeoImagen.MapearImagenEntityADominio);
    }

    public async Task<IEnumerable<Imagen>> GetByRegionAnatomicaAsync(string regionAnatomica, CancellationToken cancellationToken)
    {
        var imagenes = await context.Imagenes
            .Include(i => i.TipoImagen)
                .ThenInclude(t => t.Modalidad)
            .Include(i => i.TipoImagen)
                .ThenInclude(t => t.RegionAnatomica)
            .Where(i => i.TipoImagen.RegionAnatomica.Nombre == regionAnatomica)
            .ToListAsync(cancellationToken: cancellationToken);
        
        return imagenes.Select(MapeoImagen.MapearImagenEntityADominio);
        
    }
}

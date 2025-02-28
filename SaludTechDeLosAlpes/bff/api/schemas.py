import strawberry
from typing import List, Optional
import typing

# Definición de tipos
@strawberry.type
class Modalidad:
    nombre: str
    descripcion: str

@strawberry.type
class RegionAnatomica:
    nombre: str
    descripcion: str

@strawberry.type
class Patologia:
    descripcion: str

@strawberry.type
class AtributosImagen:
    resolucion: str
    contraste: str
    es3D: bool
    faseEscaner: Optional[str] = None

@strawberry.type
class ContextoProcesal:
    etapa: str

@strawberry.type
class EntornoClinico:
    tipoAmbiente: str

@strawberry.type
class Sintomas:
    descripcion: str

@strawberry.type
class Demografia:
    grupoEdad: str
    sexo: str
    etnicidad: str

@strawberry.type
class HistorialPaciente:
    fumador: bool
    diabetico: bool
    condicionesPrevias: Optional[List[str]] = None

@strawberry.type
class Paciente:
    demografia: Demografia
    historial: HistorialPaciente
    tokenAnonimo: str

@strawberry.type
class Imagen:
    id: str
    version: int
    tipoImagen: Modalidad
    regionAnatomica: RegionAnatomica
    patologia: Patologia
    atributosImagen: AtributosImagen
    contextoProcesal: ContextoProcesal
    metadatos: EntornoClinico
    sintomas: Sintomas
    paciente: Paciente

@strawberry.type
class ImagenRespuesta:
    mensaje: str
    codigo: int

# Resolver para obtener imágenes (ejemplo con datos dummy)
@strawberry.type
class Query:
    @strawberry.field
    def obtener_imagenes(self) -> List[Imagen]:
        return [
            Imagen(
                id="123",
                version=1,
                tipoImagen=Modalidad(nombre="Rayos X", descripcion="Imagen de rayos X"),
                regionAnatomica=RegionAnatomica(nombre="Tórax", descripcion="Imagen del tórax"),
                patologia=Patologia(descripcion="Neumonía"),
                atributosImagen=AtributosImagen(resolucion="1080p", contraste="Alto", es3D=False),
                contextoProcesal=ContextoProcesal(etapa="Diagnóstico"),
                metadatos=EntornoClinico(tipoAmbiente="Hospital"),
                sintomas=Sintomas(descripcion="Fiebre y tos"),
                paciente=Paciente(
                    demografia=Demografia(grupoEdad="Adulto", sexo="Masculino", etnicidad="Latino"),
                    historial=HistorialPaciente(fumador=False, diabetico=True, condicionesPrevias=["Hipertensión"]),
                    tokenAnonimo="token-123"
                )
            )
        ]

# Resolver para subir imágenes desde CSV
@strawberry.type
class Mutation:
    @strawberry.mutation
    def subir_imagen_csv(self, file: str) -> ImagenRespuesta:
        # Aquí se procesaría el CSV y se almacenarían los datos en la BD
        return ImagenRespuesta(mensaje="CSV procesado correctamente", codigo=200)

# Configuración del esquema
schema = strawberry.Schema(query=Query, mutation=Mutation)

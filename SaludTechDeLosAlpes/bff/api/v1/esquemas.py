import strawberry
import datetime
from typing import List

# ======================== SUBAtributos (Basado en imagen.cs) ======================== #


@strawberry.input
class ModalidadInput:
    Nombre: str
    Descripcion: str

@strawberry.input
class RegionAnatomicaInput:
    Nombre: str
    Descripcion: str

@strawberry.input
class PatologiaInput:
    Descripcion: str

@strawberry.input
class TipoImagenInput:
    Modalidad: ModalidadInput
    RegionAnatomica: RegionAnatomicaInput
    Patologia: PatologiaInput

@strawberry.input
class AtributosImagenInput:
    Resolucion: str
    Contraste: str
    Es3D: bool
    FaseEscaner: str

@strawberry.input
class ContextoProcesalInput:
    Etapa: str

@strawberry.input
class EntornoClinicoInput:
    TipoAmbiente: str

@strawberry.input
class SintomaInput:
    Descripcion: str

@strawberry.input
class MetadatosInput:
    EntornoClinico: EntornoClinicoInput
    Sintomas: List[SintomaInput]

@strawberry.input
class DemografiaInput:
    GrupoEdad: str
    Sexo: str
    Etnicidad: str

@strawberry.input
class HistorialInput:
    Fumador: bool
    Diabetico: bool
    CondicionesPrevias: List[str]

@strawberry.input
class PacienteInput:
    Demografia: DemografiaInput
    Historial: HistorialInput
    TokenAnonimo: str

# ======================== INPUT PRINCIPAL ======================== #

@strawberry.input
class IngestionInput:
    Version: str
    ImagenId: strawberry.ID
    TipoImagen: TipoImagenInput
    AtributosImagen: AtributosImagenInput
    ContextoProcesal: ContextoProcesalInput
    Metadatos: MetadatosInput
    Paciente: PacienteInput

# ======================== RESPUESTA ======================== #

@strawberry.type
class IngestionResponse:
    status: str
    message: str

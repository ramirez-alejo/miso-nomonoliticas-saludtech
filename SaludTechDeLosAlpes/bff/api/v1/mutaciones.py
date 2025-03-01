import strawberry
from strawberry.types import Info
from bff.despachadores import Despachador
from .esquemas import IngestionInput, IngestionResponse

@strawberry.type
class Mutation:
    @strawberry.mutation
    def procesar_ingestion(self, info: Info, input: IngestionInput) -> IngestionResponse:
        despachador = Despachador()
        return despachador.procesar_ingestion(input)

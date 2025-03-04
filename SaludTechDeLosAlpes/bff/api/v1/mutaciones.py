import strawberry
from strawberry.types import Info
from despachadores import Despachador
from .esquemas import IngestionInput, IngestionResponse

@strawberry.type
class Mutation:
    @strawberry.mutation
    def procesar_ingestion(self, info: Info, input: IngestionInput) -> IngestionResponse:
        despachador = Despachador()
        response = despachador.procesar_ingestion(input)

        # Aquí creamos y retornamos el objeto IngestionResponse
        return IngestionResponse(
            status=response.status,
            message=response.message
        )

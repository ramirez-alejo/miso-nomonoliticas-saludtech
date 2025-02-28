import uvicorn
from fastapi import FastAPI
from strawberry.fastapi import GraphQLRouter
from .schemas import schema  # Importamos el esquema definido

app = FastAPI()

# Creamos el router GraphQL
graphql_app = GraphQLRouter(schema)

# Lo agregamos a FastAPI
app.include_router(graphql_app, prefix="/graphql")

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8001, reload=True)

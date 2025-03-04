from fastapi import FastAPI
from api.v1.router import router

app = FastAPI()
app.include_router(router, prefix="/graphql")

# 2. Instala las dependencias con `pip install -r requirements.txt`.
# 3. Ejecuta el servidor con:
#    uvicorn bff.main:app --host 0.0.0.0 --port 8003 --reload
# 4. Accede a: http://127.0.0.1:8003/graphql
# 5. copiar el ejemplo.txt y pegarlo en el graphql
if __name__ == "__main__":
    import uvicorn
    uvicorn.run("bff.main:app", host="0.0.0.0", port=8003, reload=True)

from fastapi import FastAPI
from bff.api.v1.router import router

app = FastAPI()
app.include_router(router, prefix="/graphql")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)

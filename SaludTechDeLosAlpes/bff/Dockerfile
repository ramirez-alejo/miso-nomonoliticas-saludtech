FROM python:3.10

EXPOSE 8003/tcp

WORKDIR "/src"
COPY ./bff/requirements.txt ./requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

COPY . .


WORKDIR "/src/bff"
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8003", "--reload"]

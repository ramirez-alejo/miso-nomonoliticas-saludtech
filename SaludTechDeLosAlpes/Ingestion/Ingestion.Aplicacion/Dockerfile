FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Ingestion/Ingestion.Aplicacion/Ingestion.Aplicacion.csproj", "Ingestion/Ingestion.Aplicacion/"]
COPY ["Ingestion/Ingestion.Infraestructura/Ingestion.Infraestructura.csproj", "Ingestion/Ingestion.Infraestructura/"]
COPY ["Ingestion/Ingestion.Dominio/Ingestion.Dominio.csproj", "Ingestion/Ingestion.Dominio/"]
RUN dotnet restore "Ingestion/Ingestion.Aplicacion/Ingestion.Aplicacion.csproj"
COPY . .
WORKDIR "/src/Ingestion/Ingestion.Aplicacion"
RUN dotnet build "Ingestion.Aplicacion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ingestion.Aplicacion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ingestion.Aplicacion.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Ingestion/Ingestion.Aplicacion.Metadata/Ingestion.Aplicacion.Metadata.csproj", "Ingestion/Ingestion.Aplicacion.Metadata/"]
COPY ["Core/Core.Dominio/Core.Dominio.csproj", "Core/Core.Dominio/"]
COPY ["Ingestion/Ingestion.Dominio/Ingestion.Dominio.csproj", "Ingestion/Ingestion.Dominio/"]
COPY ["Ingestion/Ingestion.Infraestructura/Ingestion.Infraestructura.csproj", "Ingestion/Ingestion.Infraestructura/"]
RUN dotnet restore "Ingestion/Ingestion.Aplicacion.Metadata/Ingestion.Aplicacion.Metadata.csproj"
COPY . .
WORKDIR "/src/Ingestion/Ingestion.Aplicacion.Metadata"
RUN dotnet build "Ingestion.Aplicacion.Metadata.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ingestion.Aplicacion.Metadata.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ingestion.Aplicacion.Metadata.dll"]

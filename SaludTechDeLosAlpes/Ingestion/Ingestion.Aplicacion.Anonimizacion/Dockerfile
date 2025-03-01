FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ingestion/Ingestion.Aplicacion.Anonimizacion/Ingestion.Aplicacion.Anonimizacion.csproj", "Ingestion/Ingestion.Aplicacion.Anonimizacion/"]
COPY ["Ingestion/Ingestion.Dominio/Ingestion.Dominio.csproj", "Ingestion/Ingestion.Dominio/"]
COPY ["Distribucion/Consulta.Aplicacion/Consulta.Aplicacion.csproj", "Distribucion/Consulta.Aplicacion/"]
COPY ["Core/Core.Infraestructura/Core.Infraestructura.csproj", "Core/Core.Infraestructura/"]
COPY ["Core/Core.Dominio/Core.Dominio.csproj", "Core/Core.Dominio/"]
COPY ["Distribucion/Consulta.Dominio/Consulta.Dominio.csproj", "Distribucion/Consulta.Dominio/"]
COPY ["Distribucion/Consulta.Infraestructura/Consulta.Infraestructura.csproj", "Distribucion/Consulta.Infraestructura/"]
RUN dotnet restore "./Ingestion/Ingestion.Aplicacion.Anonimizacion/Ingestion.Aplicacion.Anonimizacion.csproj"
COPY . .
WORKDIR "/src/Ingestion/Ingestion.Aplicacion.Anonimizacion"
RUN dotnet build "./Ingestion.Aplicacion.Anonimizacion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ingestion.Aplicacion.Anonimizacion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ingestion.Aplicacion.Anonimizacion.dll"]

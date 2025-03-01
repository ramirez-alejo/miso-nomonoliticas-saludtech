FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ingestion/Consulta.Aplicacion.Anonimizacion/Consulta.Aplicacion.Anonimizacion.csproj", "Ingestion/Consulta.Aplicacion.Anonimizacion/"]
COPY ["Core/Core.Infraestructura/Core.Infraestructura.csproj", "Core/Core.Infraestructura/"]
COPY ["Ingestion/Consulta.Dominio/Consulta.Dominio.csproj", "Ingestion/Consulta.Dominio/"]
COPY ["Ingestion/Consulta.Infraestructura/Consulta.Infraestructura.csproj", "Ingestion/Consulta.Infraestructura/"]
RUN dotnet restore "Ingestion/Consulta.Aplicacion.Anonimizacion/Consulta.Aplicacion.Anonimizacion.csproj"
COPY . .
WORKDIR "/src/Ingestion/Consulta.Aplicacion.Anonimizacion"
RUN dotnet build "Consulta.Aplicacion.Anonimizacion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Consulta.Aplicacion.Anonimizacion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consulta.Aplicacion.Anonimizacion.dll"]

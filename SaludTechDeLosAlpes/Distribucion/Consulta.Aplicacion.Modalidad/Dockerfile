FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Distribucion/Consulta.Aplicacion.Modalidad/Consulta.Aplicacion.Modalidad.csproj", "Distribucion/Consulta.Aplicacion.Modalidad/"]
COPY ["Core/Core.Infraestructura/Core.Infraestructura.csproj", "Core/Core.Infraestructura/"]
COPY ["Distribucion/Consulta.Dominio/Consulta.Dominio.csproj", "Distribucion/Consulta.Dominio/"]
COPY ["Distribucion/Consulta.Infraestructura/Consulta.Infraestructura.csproj", "Distribucion/Consulta.Infraestructura/"]
RUN dotnet restore "Distribucion/Consulta.Aplicacion.Modalidad/Consulta.Aplicacion.Modalidad.csproj"
COPY . .
WORKDIR "/src/Distribucion/Consulta.Aplicacion.Modalidad"
RUN dotnet build "Consulta.Aplicacion.Modalidad.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Consulta.Aplicacion.Modalidad.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consulta.Aplicacion.Modalidad.dll"]

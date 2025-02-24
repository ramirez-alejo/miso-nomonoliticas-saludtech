FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Distribucion/Consulta.Aplicacion/Consulta.Aplicacion.csproj", "Distribucion/Consulta.Aplicacion/"]
COPY ["Core/Core.Infraestructura/Core.Infraestructura.csproj", "Core/Core.Infraestructura/"]
COPY ["Distribucion/Consulta.Dominio/Consulta.Dominio.csproj", "Distribucion/Consulta.Dominio/"]
COPY ["Distribucion/Consulta.Infraestructura/Consulta.Infraestructura.csproj", "Distribucion/Consulta.Infraestructura/"]
RUN dotnet restore "Distribucion/Consulta.Aplicacion/Consulta.Aplicacion.csproj"
COPY . .
WORKDIR "/src/Distribucion/Consulta.Aplicacion"
RUN dotnet build "Consulta.Aplicacion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consulta.Aplicacion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consulta.Aplicacion.dll"]

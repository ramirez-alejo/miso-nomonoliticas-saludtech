FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Distribucion/Consulta.Aplicacion.Demografia/Consulta.Aplicacion.Demografia.csproj", "Distribucion/Consulta.Aplicacion.Demografia/"]
COPY ["Core/Core.Infraestructura/Core.Infraestructura.csproj", "Core/Core.Infraestructura/"]
COPY ["Distribucion/Consulta.Dominio/Consulta.Dominio.csproj", "Distribucion/Consulta.Dominio/"]
COPY ["Distribucion/Consulta.Infraestructura/Consulta.Infraestructura.csproj", "Distribucion/Consulta.Infraestructura/"]
RUN dotnet restore "Distribucion/Consulta.Aplicacion.Demografia/Consulta.Aplicacion.Demografia.csproj"
COPY . .
WORKDIR "/src/Distribucion/Consulta.Aplicacion.Demografia"
RUN dotnet build "Consulta.Aplicacion.Demografia.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consulta.Aplicacion.Demografia.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consulta.Aplicacion.Demografia.dll"]

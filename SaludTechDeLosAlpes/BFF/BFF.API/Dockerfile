FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BFF/BFF.API/BFF.API.csproj", "BFF/BFF.API/"]
COPY ["Core/Core.Dominio/Core.Dominio.csproj", "Core/Core.Dominio/"]
COPY ["Core/Core.Infraestructura/Core.Infraestructura.csproj", "Core/Core.Infraestructura/"]
COPY ["Ingestion/Ingestion.Dominio/Ingestion.Dominio.csproj", "Ingestion/Ingestion.Dominio/"]
RUN dotnet restore "BFF/BFF.API/BFF.API.csproj"
COPY . .
WORKDIR "/src/BFF/BFF.API"
RUN dotnet build "BFF.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BFF.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BFF.API.dll"]

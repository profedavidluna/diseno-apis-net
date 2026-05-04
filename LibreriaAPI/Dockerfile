# Etapa base: imagen de runtime ASP.NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build: imagen SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LibreriaAPI/LibreriaAPI.csproj", "LibreriaAPI/"]
RUN dotnet restore "LibreriaAPI/LibreriaAPI.csproj"
COPY . .
WORKDIR "/src/LibreriaAPI"
RUN dotnet build "LibreriaAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LibreriaAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final: imagen mínima de runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibreriaAPI.dll"]

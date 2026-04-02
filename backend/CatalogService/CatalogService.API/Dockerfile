FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["CatalogService/CatalogService.API/CatalogService.API.csproj", "CatalogService/CatalogService.API/"]
COPY ["CatalogService/CatalogService.BLL/CatalogService.BLL.csproj", "CatalogService/CatalogService.BLL/"]
COPY ["CatalogService/CatalogService.DAL/CatalogService.DAL.csproj", "CatalogService/CatalogService.DAL/"]
COPY ["CatalogService/CatalogService.Domain/CatalogService.Domain.csproj", "CatalogService/CatalogService.Domain/"]
RUN dotnet restore "CatalogService/CatalogService.API/CatalogService.API.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/CatalogService/CatalogService.API"
RUN dotnet publish "CatalogService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.API.dll"]

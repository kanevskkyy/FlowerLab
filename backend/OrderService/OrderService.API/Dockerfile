FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["OrderService/OrderService.API/OrderService.API.csproj", "OrderService/OrderService.API/"]
COPY ["OrderService/OrderService.BLL/OrderService.BLL.csproj", "OrderService/OrderService.BLL/"]
COPY ["OrderService/OrderService.DAL/OrderService.DAL.csproj", "OrderService/OrderService.DAL/"]
COPY ["OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService/OrderService.Domain/"]
RUN dotnet restore "OrderService/OrderService.API/OrderService.API.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/OrderService/OrderService.API"
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]

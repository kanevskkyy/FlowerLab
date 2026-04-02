FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["AggregatorService/AggregatorService.csproj", "AggregatorService/"]
RUN dotnet restore "AggregatorService/AggregatorService.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/AggregatorService"
RUN dotnet publish "AggregatorService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AggregatorService.dll"]

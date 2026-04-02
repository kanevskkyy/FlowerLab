FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["ReviewService/ReviewService.API/ReviewService.API.csproj", "ReviewService/ReviewService.API/"]
COPY ["ReviewService/ReviewService.Application/ReviewService.Application.csproj", "ReviewService/ReviewService.Application/"]
COPY ["ReviewService/ReviewService.Domain/ReviewService.Domain.csproj", "ReviewService/ReviewService.Domain/"]
COPY ["ReviewService/ReviewService.Infrastructure/ReviewService.Infrastructure.csproj", "ReviewService/ReviewService.Infrastructure/"]
RUN dotnet restore "ReviewService/ReviewService.API/ReviewService.API.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/ReviewService/ReviewService.API"
RUN dotnet publish "ReviewService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewService.API.dll"]

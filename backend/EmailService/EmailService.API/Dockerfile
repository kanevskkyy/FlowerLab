FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["EmailService/EmailService.API/EmailService.API.csproj", "EmailService/EmailService.API/"]
COPY ["EmailService/EmailService.BLL/EmailService.BLL.csproj", "EmailService/EmailService.BLL/"]
RUN dotnet restore "EmailService/EmailService.API/EmailService.API.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/EmailService/EmailService.API"
RUN dotnet publish "EmailService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailService.API.dll"]

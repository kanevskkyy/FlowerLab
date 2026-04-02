FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["NotificationService/Notify.csproj", "NotificationService/"]
RUN dotnet restore "NotificationService/Notify.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/NotificationService"
RUN dotnet publish "Notify.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notify.dll"]

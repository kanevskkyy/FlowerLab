FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["shared/shared.csproj", "shared/"]
COPY ["UsersService/UsersService.API/UsersService.API.csproj", "UsersService/UsersService.API/"]
COPY ["UsersService/UsersService.BLL/UsersService.BLL.csproj", "UsersService/UsersService.BLL/"]
COPY ["UsersService/UsersService.DAL/UsersService.DAL.csproj", "UsersService/UsersService.DAL/"]
COPY ["UsersService/UsersService.Domain/UsersService.Domain.csproj", "UsersService/UsersService.Domain/"]
RUN dotnet restore "UsersService/UsersService.API/UsersService.API.csproj"
COPY . .

FROM build AS publish
WORKDIR "/src/UsersService/UsersService.API"
RUN dotnet publish "UsersService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.API.dll"]

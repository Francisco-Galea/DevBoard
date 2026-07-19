# Etapa 1 — build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/DevBoard.API/DevBoard.API.csproj", "src/DevBoard.API/"]
COPY ["src/DevBoard.Application/DevBoard.Application.csproj", "src/DevBoard.Application/"]
COPY ["src/DevBoard.Domain/DevBoard.Domain.csproj", "src/DevBoard.Domain/"]
COPY ["src/DevBoard.Infrastructure/DevBoard.Infrastructure.csproj", "src/DevBoard.Infrastructure/"]
RUN dotnet restore "src/DevBoard.API/DevBoard.API.csproj"

COPY . .
RUN dotnet publish "src/DevBoard.API/DevBoard.API.csproj" \
    -c Release \
    -o /app/publish

# Etapa 2 — runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DevBoard.API.dll"]

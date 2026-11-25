# Багатоетапна збірка Web API: SDK-образ для збирання, легкий runtime-образ для запуску.
# Контекст збірки — каталог src/WarehouseApi (див. docker-compose.yaml).

# ---- Етап 1: build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Спочатку лише .csproj: шар із відновленими пакетами кешується,
# поки не змінюються залежності.
COPY WarehouseApi.csproj ./
RUN dotnet restore WarehouseApi.csproj

COPY . ./
RUN dotnet publish WarehouseApi.csproj -c Release -o /app/publish --no-restore

# ---- Етап 2: runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Образ aspnet:8.0 слухає порт 8080 і запускається від непривілейованого користувача app.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app

ENTRYPOINT ["dotnet", "WarehouseApi.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "AssetApi/AssetApi.csproj"
COPY . .
WORKDIR "/src/AssetApi"
RUN dotnet build "AssetApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AssetApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssetApi.dll"]

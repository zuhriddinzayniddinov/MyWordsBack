FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TemplateApi/TemplateApi.csproj"
COPY . .
WORKDIR "/src/TemplateApi"
RUN dotnet build "TemplateApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TemplateApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TemplateApi.dll"]

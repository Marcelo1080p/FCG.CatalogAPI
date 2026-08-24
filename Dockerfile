FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/FCG.CatalogAPI.Domain/FCG.CatalogAPI.Domain.csproj src/FCG.CatalogAPI.Domain/
COPY src/FCG.CatalogAPI.Application/FCG.CatalogAPI.Application.csproj src/FCG.CatalogAPI.Application/
COPY src/FCG.CatalogAPI.Infrastructure/FCG.CatalogAPI.Infrastructure.csproj src/FCG.CatalogAPI.Infrastructure/
COPY src/FCG.CatalogAPI.API/FCG.CatalogAPI.API.csproj src/FCG.CatalogAPI.API/
RUN dotnet restore src/FCG.CatalogAPI.API/FCG.CatalogAPI.API.csproj

COPY src/ src/
RUN dotnet publish src/FCG.CatalogAPI.API/FCG.CatalogAPI.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG.CatalogAPI.API.dll"]

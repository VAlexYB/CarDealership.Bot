FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5265
EXPOSE 7282

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CarDealership.Bot.Api/nuget.config", "./"]
COPY ./nupkgs /nupkgs
COPY ["CarDealership.Bot.Api/CarDealership.Bot.Api.csproj", "CarDealership.Bot.Api/"]
COPY ["CarDealership.Bot.DataAccess/CarDealership.Bot.DataAccess.csproj", "CarDealership.Bot.DataAccess/"]
RUN dotnet restore "./CarDealership.Bot.Api/CarDealership.Bot.Api.csproj"

COPY . .
WORKDIR "/src/CarDealership.Bot.Api"
RUN dotnet build "./CarDealership.Bot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CarDealership.Bot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarDealership.Bot.Api.dll"]
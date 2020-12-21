#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Newbe.Claptrap.StorageTestWebApi/Newbe.Claptrap.StorageTestWebApi.csproj", "Newbe.Claptrap.StorageTestWebApi/"]
COPY ["Newbe.Claptrap.StorageProvider.MySql/Newbe.Claptrap.StorageProvider.MySql.csproj", "Newbe.Claptrap.StorageProvider.MySql/"]
COPY ["Newbe.Claptrap.Configuration/Newbe.Claptrap.Configuration.csproj", "Newbe.Claptrap.Configuration/"]
COPY ["Newbe.Claptrap.StorageProvider.Relational/Newbe.Claptrap.StorageProvider.Relational.csproj", "Newbe.Claptrap.StorageProvider.Relational/"]
COPY ["Newbe.Claptrap.AppMetrics/Newbe.Claptrap.AppMetrics.csproj", "Newbe.Claptrap.AppMetrics/"]
COPY ["Newbe.Claptrap.Abstractions/Newbe.Claptrap.Abstractions.csproj", "Newbe.Claptrap.Abstractions/"]
COPY ["Newbe.Claptrap/Newbe.Claptrap.csproj", "Newbe.Claptrap/"]
COPY ["Newbe.Claptrap.Attributes/Newbe.Claptrap.Attributes.csproj", "Newbe.Claptrap.Attributes/"]
COPY ["Newbe.Claptrap.Localization/Newbe.Claptrap.Localization.csproj", "Newbe.Claptrap.Localization/"]
COPY ["Newbe.Claptrap.Orleans.Hosting/Newbe.Claptrap.Orleans.Hosting.csproj", "Newbe.Claptrap.Orleans.Hosting/"]
COPY ["Newbe.Claptrap.StorageProvider.Faster/Newbe.Claptrap.StorageProvider.Faster.csproj", "Newbe.Claptrap.StorageProvider.Faster/"]
COPY ["Newbe.Claptrap.StorageSetup/Newbe.Claptrap.StorageSetup.csproj", "Newbe.Claptrap.StorageSetup/"]
COPY ["Newbe.Claptrap.StorageProvider.SQLite/Newbe.Claptrap.StorageProvider.SQLite.csproj", "Newbe.Claptrap.StorageProvider.SQLite/"]
COPY ["Newbe.Claptrap.StorageProvider.PostgreSQL/Newbe.Claptrap.StorageProvider.PostgreSQL.csproj", "Newbe.Claptrap.StorageProvider.PostgreSQL/"]
COPY ["Newbe.Claptrap.StorageProvider.MongoDB/Newbe.Claptrap.StorageProvider.MongoDB.csproj", "Newbe.Claptrap.StorageProvider.MongoDB/"]
COPY ["Newbe.Claptrap.TestSuit/Newbe.Claptrap.TestSuit.csproj", "Newbe.Claptrap.TestSuit/"]
COPY ["Newbe.Claptrap.DataSerializer.Json/Newbe.Claptrap.DataSerializer.Json.csproj", "Newbe.Claptrap.DataSerializer.Json/"]
RUN dotnet restore "Newbe.Claptrap.StorageTestWebApi/Newbe.Claptrap.StorageTestWebApi.csproj"
COPY . .
WORKDIR "/src/Newbe.Claptrap.StorageTestWebApi"
RUN dotnet build "Newbe.Claptrap.StorageTestWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Newbe.Claptrap.StorageTestWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Newbe.Claptrap.StorageTestWebApi.dll"]
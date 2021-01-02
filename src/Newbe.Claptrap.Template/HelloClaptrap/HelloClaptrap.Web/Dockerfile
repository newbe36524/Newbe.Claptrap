#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

# Create this "restore-solution" section by running ./Create-DockerfileSolutionRestore.ps1, to optimize build cache reuse
COPY ["HelloClaptrap.Actors.Tests/HelloClaptrap.Actors.Tests.csproj", "HelloClaptrap.Actors.Tests/"]
COPY ["HelloClaptrap.Actors/HelloClaptrap.Actors.csproj", "HelloClaptrap.Actors/"]
COPY ["HelloClaptrap.BackendServer/HelloClaptrap.BackendServer.csproj", "HelloClaptrap.BackendServer/"]
COPY ["HelloClaptrap.IActor/HelloClaptrap.IActor.csproj", "HelloClaptrap.IActor/"]
COPY ["HelloClaptrap.Models/HelloClaptrap.Models.csproj", "HelloClaptrap.Models/"]
COPY ["HelloClaptrap.Web/HelloClaptrap.Web.csproj", "HelloClaptrap.Web/"]
COPY ["docker-compose.dcproj", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["NuGet.config", "./"]
COPY ["HelloClaptrap.sln", "./"]
RUN dotnet restore "HelloClaptrap.sln"

COPY . .
WORKDIR "/src/HelloClaptrap.Web"
RUN dotnet build "HelloClaptrap.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelloClaptrap.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelloClaptrap.Web.dll"]

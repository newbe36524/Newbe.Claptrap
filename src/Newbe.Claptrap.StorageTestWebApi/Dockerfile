#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
# Copy csproj and restore as distinct layers
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files-part-2/
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

RUN dotnet restore 
COPY . .
WORKDIR "/src/Newbe.Claptrap.StorageTestWebApi"
RUN dotnet build "Newbe.Claptrap.StorageTestWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Newbe.Claptrap.StorageTestWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Newbe.Claptrap.StorageTestWebApi.dll"]
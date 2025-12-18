# Build-Phase
FROM mcr.microsoft.com/dotnet/sdk:10 AS build-env
WORKDIR /app

# Kopieren und Restore
COPY *.csproj ./
RUN dotnet restore

# Alles kopieren und Build
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime-Phase
FROM mcr.microsoft.com/dotnet/aspnet:10
WORKDIR /app
COPY --from=build-env /app/out .

# Port für Cloud-Hoster (Render nutzt oft 8080)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProTaskManagerAPI.dll"]
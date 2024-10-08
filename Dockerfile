# Use the official Microsoft image for building .NET Framework 4.8 applications
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY SecretSantaApp/*.csproj ./SecretSantaApp/
RUN nuget restore

# Copy everything else and build
COPY . .
WORKDIR /app/SecretSantaApp
RUN msbuild /p:Configuration=Release /p:OutputPath=/app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["C:\\Windows\\System32\\inetsrv\\w3wp.exe"]

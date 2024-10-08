FROM mcr.microsoft.com/dotnet/sdk:4.8 AS build
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY SecretSantaApp/*.csproj ./SecretSantaApp/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/SecretSantaApp
RUN dotnet publish --configuration Release --output /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:4.8 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SecretSantaApp.dll"]

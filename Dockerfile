FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Install Visual Studio Build Tools
RUN apt-get update && \
    apt-get install -y wget && \
    wget https://aka.ms/vs/17/release/vs_buildtools.exe && \
    wine vs_buildtools.exe --quiet --wait --norestart --nocache \
    --installPath C:\BuildTools \
    --add Microsoft.VisualStudio.Workload.WebBuildTools
    
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY SecretSantaApp/*.csproj ./SecretSantaApp/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/SecretSantaApp
RUN dotnet publish --configuration Release --output /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SecretSantaApp.dll"]

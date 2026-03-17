FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.sln .
COPY SecretSantaApp/*.csproj ./SecretSantaApp/
RUN dotnet restore

COPY . .
WORKDIR /app/SecretSantaApp
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SecretSantaApp.dll"]

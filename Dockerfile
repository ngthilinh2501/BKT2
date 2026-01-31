# Base image for build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["PCM_357.csproj", "./"]
RUN dotnet restore "PCM_357.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/."
RUN dotnet build "PCM_357.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PCM_357.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PCM_357.dll"]

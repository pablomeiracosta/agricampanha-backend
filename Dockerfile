# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Auria.sln", "./"]
COPY ["Auria.API/Auria.API.csproj", "Auria.API/"]
COPY ["Auria.Bll/Auria.Bll.csproj", "Auria.Bll/"]
COPY ["Auria.Data/Auria.Data.csproj", "Auria.Data/"]
COPY ["Auria.Dto/Auria.Dto.csproj", "Auria.Dto/"]
COPY ["Auria.Structure/Auria.Structure.csproj", "Auria.Structure/"]

# Restore dependencies
RUN dotnet restore "Auria.API/Auria.API.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/Auria.API"
RUN dotnet build "Auria.API.csproj" -c Release -o /app/build
RUN dotnet publish "Auria.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Run the application
ENTRYPOINT ["dotnet", "Auria.API.dll"]

# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["Directory.Packages.props", "."]
COPY ["Microservices/Challenge.Microservices.MotorbikeApi/Challenge.Microservices.MotorbikeApi.csproj", "Microservices/Challenge.Microservices.MotorbikeApi/"]
COPY ["Common/Core/Challenge.Common.Core.Response/Challenge.Common.Core.Response.csproj", "Common/Core/Challenge.Common.Core.Response/"]
COPY ["Common/Core/Challenge.Common.Core.Validation/Challenge.Common.Core.Validation.csproj", "Common/Core/Challenge.Common.Core.Validation/"]
COPY ["Common/Data/Challenge.Common.Data/Challenge.Common.Data.csproj", "Common/Data/Challenge.Common.Data/"]
COPY ["Common/Data/Challenge.Common.Data.Mongo/Challenge.Common.Data.Mongo.csproj", "Common/Data/Challenge.Common.Data.Mongo/"]

# Restore dependencies
RUN dotnet restore "Microservices/Challenge.Microservices.MotorbikeApi/Challenge.Microservices.MotorbikeApi.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Microservices/Challenge.Microservices.MotorbikeApi"
RUN dotnet build "Challenge.Microservices.MotorbikeApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Challenge.Microservices.MotorbikeApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Challenge.Microservices.MotorbikeApi.dll"]
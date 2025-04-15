# Use the official ASP.NET Core runtime image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["PingyThingy.sln", "./"]
COPY ["src/PingyThingy.Api/PingyThingy.Api.csproj", "src/PingyThingy.Api/"]
COPY ["src/PingyThingy.Core/PingyThingy.Core.csproj", "src/PingyThingy.Core/"]
RUN dotnet restore "PingyThingy.sln"

# Copy the rest of the application code
COPY . .
WORKDIR "/src/src/PingyThingy.Api"
RUN dotnet build "PingyThingy.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PingyThingy.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PingyThingy.Api.dll"]

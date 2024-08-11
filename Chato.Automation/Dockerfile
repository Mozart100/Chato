#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Chato.Automation/Chato.Automation.csproj", "Chato.Automation/"]
COPY ["Chatto.Shared/Chatto.Shared.csproj", "Chatto.Shared/"]
RUN dotnet restore "./Chato.Automation/./Chato.Automation.csproj"
COPY . .
WORKDIR "/src/Chato.Automation"
RUN dotnet build "./Chato.Automation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Chato.Automation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chato.Automation.dll"]
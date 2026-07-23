FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Docker
WORKDIR /src
COPY ["CvManagementSystem.Api/CvManagementSystem.Api.csproj", "CvManagementSystem.Api/"]
COPY ["CvManagementSystem.DataAccess/CvManagementSystem.DataAccess.csproj", "CvManagementSystem.DataAccess/"]
COPY ["CvManagementSystem.Application/CvManagementSystem.Application.csproj", "CvManagementSystem.Application/"]
COPY ["CvManagementSystem.Domain/CvManagementSystem.Domain.csproj", "CvManagementSystem.Domain/"]
COPY ["CvManagementSystem.Infrastructure/CvManagementSystem.Infrastructure.csproj", "CvManagementSystem.Infrastructure/"]
RUN dotnet restore "CvManagementSystem.Api/CvManagementSystem.Api.csproj"
COPY . .
WORKDIR "/src/CvManagementSystem.Api"
RUN dotnet build "CvManagementSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Docker
RUN dotnet publish "CvManagementSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CvManagementSystem.Api.dll"]

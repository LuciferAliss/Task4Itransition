FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Task4Itransition.API/Task4Itransition.API.csproj", "Task4Itransition.API/"]
COPY ["Task4Itransition.Application/Task4Itransition.Application.csproj", "Task4Itransition.Application/"]
COPY ["Task4Itransition.Domain/Task4Itransition.Domain.csproj", "Task4Itransition.Domain/"]
COPY ["Task4Itransition.Infrastructure/Task4Itransition.Infrastructure.csproj", "Task4Itransition.Infrastructure/"]
RUN dotnet restore "Task4Itransition.API/Task4Itransition.API.csproj"
COPY . .
WORKDIR "/src/Task4Itransition.API"
RUN dotnet build "./Task4Itransition.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Task4Itransition.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task4Itransition.API.dll"]

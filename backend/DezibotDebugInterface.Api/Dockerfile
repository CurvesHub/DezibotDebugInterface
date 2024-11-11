FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/DezibotDebugInterface.Api/DezibotDebugInterface.Api.csproj", "DezibotDebugInterface.Api/"]
RUN dotnet restore "backend/DezibotDebugInterface.Api/DezibotDebugInterface.Api.csproj"
COPY ["backend/DezibotDebugInterface.Api/", "DezibotDebugInterface.Api/"]
WORKDIR "/src/DezibotDebugInterface.Api"
RUN dotnet build "DezibotDebugInterface.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DezibotDebugInterface.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DezibotDebugInterface.Api.dll"]

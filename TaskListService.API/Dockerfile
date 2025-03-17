FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaskListService.API/TaskListService.API.csproj", "TaskListService.API/"]
COPY ["TaskListService.Application/TaskListService.Application.csproj", "TaskListService.Application/"]
COPY ["TaskListService.Domain/TaskListService.Domain.csproj", "TaskListService.Domain/"]
COPY ["TaskListService.Persistence/TaskListService.Persistence.csproj", "TaskListService.Persistence/"]
RUN dotnet restore "TaskListService.API/TaskListService.API.csproj"
COPY . .
WORKDIR "/src/TaskListService.API"
RUN dotnet build "TaskListService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TaskListService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskListService.API.dll"]

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["TempLoggerService.ModelsCore/TempLoggerService.ModelsCore.csproj", "TempLoggerService.ModelsCore/"]
RUN dotnet restore "TempLoggerService.ModelsCore/TempLoggerService.ModelsCore.csproj"
COPY TempLoggerService.ModelsCore TempLoggerService.ModelsCore

COPY ["TempLoggerService.Api/TempLoggerService.Api.csproj", "TempLoggerService.Api/"]
RUN dotnet restore "TempLoggerService.Api/TempLoggerService.Api.csproj"
COPY TempLoggerService.Api TempLoggerService.Api
WORKDIR "/src/TempLoggerService.Api"
RUN dotnet build "TempLoggerService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TempLoggerService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TempLoggerService.Api.dll"]
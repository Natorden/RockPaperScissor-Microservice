﻿FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RandomPlayerService/RandomPlayerService.csproj", "RandomPlayerService/"]
COPY ["Helpers/Helpers.csproj", "Helpers/"]
COPY ["Events/Events.csproj", "Events/"]
RUN dotnet restore "RandomPlayerService/RandomPlayerService.csproj"
COPY . .
WORKDIR "/src/RandomPlayerService"
RUN dotnet build "RandomPlayerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RandomPlayerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RandomPlayerService.dll"]

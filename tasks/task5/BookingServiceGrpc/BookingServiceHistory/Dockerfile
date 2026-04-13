FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookingServiceHistory/BookingServiceHistory.csproj", "BookingServiceHistory/"]
RUN dotnet restore "BookingServiceHistory/BookingServiceHistory.csproj"
COPY . .
WORKDIR "/src/BookingServiceHistory"
RUN dotnet build "./BookingServiceHistory.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookingServiceHistory.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet tool install --global dotnet-ef

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingServiceHistory.dll"]

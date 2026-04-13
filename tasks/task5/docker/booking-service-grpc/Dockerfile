
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["../BookingServiceGrpc/BookingServiceGrpc/BookingServiceGrpc.csproj", "BookingServiceGrpc/"]
RUN dotnet restore "BookingServiceGrpc/BookingServiceGrpc.csproj"
COPY ../BookingServiceGrpc/ .
WORKDIR "/src/BookingServiceGrpc"
RUN dotnet build "./BookingServiceGrpc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./BookingServiceGrpc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet tool install --global dotnet-ef


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingServiceGrpc.dll"]

FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /app/VehicleTracker.ServiceBus
COPY ./VehicleTracker.ServiceBus/*.csproj ./
RUN dotnet restore

WORKDIR /app/VehicleTracker.Connector
COPY ./VehicleTracker.Connector/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app/VehicleTracker.ServiceBus
COPY ./VehicleTracker.ServiceBus ./

WORKDIR /app/VehicleTracker.Connector
COPY ./VehicleTracker.Connector ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/VehicleTracker.Connector/out/ .
ENTRYPOINT ["dotnet", "VehicleTracker.Connector.dll"]

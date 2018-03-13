FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /app/VehicleTracker.ServiceBus
COPY ./VehicleTracker.ServiceBus/*.csproj ./
RUN dotnet restore

WORKDIR /app/VehicleTracker.TrackerManager
COPY ./VehicleTracker.TrackerManager/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app/VehicleTracker.ServiceBus
COPY ./VehicleTracker.ServiceBus ./

WORKDIR /app/VehicleTracker.TrackerManager
COPY ./VehicleTracker.TrackerManager ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/VehicleTracker.TrackerManager/out/ .
ENTRYPOINT ["dotnet", "VehicleTracker.TrackerManager.dll"]

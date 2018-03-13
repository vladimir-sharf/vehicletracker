FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /app/VehicleTracker.ServiceBus
COPY ./VehicleTracker.ServiceBus/*.csproj ./
RUN dotnet restore

WORKDIR /app/VehicleTracker.Api
COPY ./VehicleTracker.Api/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app/VehicleTracker.ServiceBus
COPY ./VehicleTracker.ServiceBus ./

WORKDIR /app/VehicleTracker.Api
COPY ./VehicleTracker.Api ./
RUN npm install
RUN ./node_modules/.bin/webpack --config webpack.release.config.js -p
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/VehicleTracker.Api/out/ .
COPY --from=build-env /app/VehicleTracker.Api/client-build/ ./client-build/
ENTRYPOINT ["dotnet", "VehicleTracker.Api.dll"]

FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /app/VehicleTracker.AuthService
COPY ./VehicleTracker.AuthService/*.csproj ./
RUN dotnet restore

# Copy everything else and build
WORKDIR /app/VehicleTracker.AuthService
COPY ./VehicleTracker.AuthService ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/VehicleTracker.AuthService/out/ .
ENTRYPOINT ["dotnet", "VehicleTracker.AuthService.dll"]

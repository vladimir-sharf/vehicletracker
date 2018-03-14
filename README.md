# Vehicle Tracker

## Installation

You need Docker with linux containers support to run.

git clone https://github.com/vladimir-sharf/vehicletracker.git
cd VehicleTracker
docker-compose up

## Building and running from the source code

git clone https://github.com/vladimir-sharf/vehicletracker.git
cd VehicleTracker/VehicleTracker.Api
npm install
./node_modules/.bin/webpack --config webpack.release.config.js -p
cd ..
dotnet build

To run each project
cd {ProjectDir}
dotnet run

Or
./build.cmd
on Windows

Note: you need MS SQL server and RabbitMq to run the services. Check appsettings.json and set up correct connection properties.

## Description

VehicleTracker shows information about (fake) vehicles status. Statuses are updated in real-time through websockets.

Technologies:
* AspNet.Core
* RabbitMq for messaging - I needed free messaging service not related to any cloud platform. Probably should have chosen Redis.
* MS SQL for storage - just the database server that I had installed on my machine. If there were strict performance requirements, it would be better to use some NoSQL storage
* IdentityServer for authentification - for this test task I needed standalone auth service in order to remove external dependencies
* React, Mobx - front-end
* SignalR - server-client communication

### VehicleTracker.TrackerService

Responsible for contacting vehicles. All it can do is to get vehicle id (vin) and ping it. No information about vehicles, customers, subscriptions is stored.

REST endpoint vehicles/ping/{id} is for testing purposes. Main interaction happens through the message bus. The service accepts a track request with single vehicle id in it and then publishes status on the bus. This makes service easily scalable - just add more instances.

### VehicleTracker.TrackerManager

Stores information which vehicles to query and hosts the job that posts track requests periodically for these vehicles.

It is questionable whether we need Tracker Manager or should we manage subscriptions in TrackerService instead. I picked the first options, because it gives greater separation of responsibility and more flexibility in future.

TODO: implement unsubscription

### VehicleTracker.StorageService

Stores the information about vehicles and customers. 

I didn't separate these two on the first stage in order not to make application structure too complex. But I was always keeping in mind that some day I may need separate services for vehicles and customers. That is the reason that I don't have foreign keys in DB and don't do any joins, for example.

EntityFramework was the easiest way to deal with data, though I would probably take something more lightweight for production if it required performance.

StorageService currently communicates through REST api. I didn't see the reason why it should use messaging. Maybe later I would change it anyway, for consistency reasons across solution.

### VehicleTracker.AuthService

Authentificates users and issues JWT tokens.

Don't look too much on the code. Just took sample IdentityServer4 project and tweaked it a little bit.

### VehicleTracker.Api

Public api & hosting the client application.
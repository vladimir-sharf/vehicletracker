using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.ServiceBus 
{
    public static class MessageUtils 
    {
        public static IServiceBus SubscribeVehicleTrackSubscribe(this IServiceBus serviceBus, Func<VehicleTrackSubscribeRequest, Task> callback) 
        {
            serviceBus.Subscribe<VehicleTrackSubscribeRequest>(QueueNames.SubscribeQueueName, callback, InteractionType.CompetingConsumers);
            return serviceBus;            
        }

        public static IServiceBus SubscribeVehicleTrackRequest(this IServiceBus serviceBus, Func<VehicleTrackRequest, Task> callback) 
        {
            serviceBus.Subscribe<VehicleTrackRequest>(QueueNames.TrackQueueName, callback, InteractionType.CompetingConsumers);
            return serviceBus;            
        }

        public static IServiceBus SubscribeVehicleInfo(this IServiceBus serviceBus, Func<VehicleStatusMessage, Task> callback) 
        {
            serviceBus.Subscribe<VehicleStatusMessage>(QueueNames.StatusQueueName, callback, InteractionType.PublishSubscribe);
            return serviceBus;            
        }

        public static Task SendVehicleTrackRequest(this IServiceBus serviceBus, VehicleTrackRequest req)
            => serviceBus.Publish(QueueNames.TrackQueueName, req, InteractionType.CompetingConsumers);

        public static Task SendVehicleStatus(this IServiceBus serviceBus, VehicleStatusMessage req)
            => serviceBus.Publish(QueueNames.StatusQueueName, req, InteractionType.PublishSubscribe);

        public static Task SendSubscribeRequest(this IServiceBus serviceBus, VehicleTrackSubscribeRequest req)
            => serviceBus.Publish(QueueNames.SubscribeQueueName, req, InteractionType.CompetingConsumers);          
    }
}
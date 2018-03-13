using System;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus.Messages;

namespace VehicleTracker.ServiceBus 
{
    public static class MessageUtils 
    {
        public static Task<IServiceBusListener> SubscribeVehicleTrackSubscribe(this Task<IServiceBusListener> serviceBus, Func<VehicleTrackSubscribeRequest, Task> callback) 
            => serviceBus.Subscribe(QueueNames.SubscribeQueueName, callback, InteractionType.CompetingConsumers);

        public static Task<IServiceBusListener> SubscribeVehicleTrackRequest(this Task<IServiceBusListener> serviceBus, Func<VehicleTrackRequest, Task> callback) 
            => serviceBus.Subscribe(QueueNames.TrackQueueName, callback, InteractionType.CompetingConsumers);

        public static Task<IServiceBusListener> SubscribeVehicleInfo(this Task<IServiceBusListener> serviceBus, Func<VehicleStatusMessage, Task> callback) 
            => serviceBus.Subscribe(QueueNames.StatusQueueName, callback, InteractionType.PublishSubscribe);

        public static Task SendVehicleTrackRequest(this IServiceBus serviceBus, VehicleTrackRequest req)
            => serviceBus.Publish(QueueNames.TrackQueueName, req, InteractionType.CompetingConsumers);

        public static Task SendVehicleStatus(this IServiceBus serviceBus, VehicleStatusMessage req)
            => serviceBus.Publish(QueueNames.StatusQueueName, req, InteractionType.PublishSubscribe);

        public static Task SendSubscribeRequest(this IServiceBus serviceBus, VehicleTrackSubscribeRequest req)
            => serviceBus.Publish(QueueNames.SubscribeQueueName, req, InteractionType.CompetingConsumers);          
    }
}
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using VehicleTracker.ServiceBus;
using Xunit;

namespace VehicleTracker.UnitTests
{
    public class ServiceBusTests
    {
        private class TestMessage
        {
            public string Id { get; set; }
            public string String { get; set; }
        }
        
        [Fact]
        public async Task RabbitMqBus_CompetingConsumers_Test()
        {
            var options = CreateOptions();
            var logger = CommonUtils.CreateLogger<RabbitMqBus>();
            var rabbitMqBus = new RabbitMqBus(options, logger);
            var rabbitMqListener = new RabbitMqListener(options, logger);

            var message = new TestMessage
            {
                Id = "1",
                String = "Test string",
            };

            var count = 0;
            TestMessage result = null;
            rabbitMqListener.Subscribe("test", (TestMessage m) =>
            {
                result = m;
                return Task.FromResult(count++);
            }, InteractionType.CompetingConsumers);

            await rabbitMqBus.Publish("test", message, InteractionType.CompetingConsumers);
            await rabbitMqBus.Publish("test", message, InteractionType.CompetingConsumers);
            await rabbitMqBus.Publish("test", message, InteractionType.CompetingConsumers);

            await TimeSpan.FromSeconds(1)
                .StopOnCondition(TimeSpan.FromMilliseconds(100), () => count >= 3);

            count.ShouldBe(3);
            result.ShouldNotBeNull();
            result.Id.ShouldBe(message.Id);
            result.String.ShouldBe(message.String);
        }

        [Fact]
        public async Task RabbitMqBus_PublishSubscribe_Test()
        {
            var options = CreateOptions();
            var logger = CommonUtils.CreateLogger<RabbitMqBus>();
            var rabbitMqBus = new RabbitMqBus(options, logger);
            var rabbitMqListener = new RabbitMqListener(options, logger);

            var message = new TestMessage
            {
                Id = "1",
                String = "Test string",
            };

            var count = 0;
            TestMessage result = null;
            rabbitMqListener.Subscribe("test", (TestMessage m) =>
            {
                result = m;
                return Task.FromResult(count++);
            }, InteractionType.PublishSubscribe);

            var count2 = 0;
            TestMessage result2 = null;
            rabbitMqListener.Subscribe("test", (TestMessage m) =>
            {
                result2 = m;
                return Task.FromResult(count2++);
            }, InteractionType.PublishSubscribe);

            await rabbitMqBus.Publish("test", message, InteractionType.PublishSubscribe);
            await rabbitMqBus.Publish("test", message, InteractionType.PublishSubscribe);

            await TimeSpan.FromSeconds(1)
                .StopOnCondition(TimeSpan.FromMilliseconds(100), () => count >= 2 && count2 >= 2);

            count.ShouldBe(2);
            result.ShouldNotBeNull();
            result.Id.ShouldBe(message.Id);
            result.String.ShouldBe(message.String);

            count2.ShouldBe(2);
            result2.ShouldNotBeNull();
            result2.Id.ShouldBe(message.Id);
            result2.String.ShouldBe(message.String);
        }

        [Fact]
        public async Task ListenerFactory_Test()
        {
            var options = CreateOptions();
            var logger = CommonUtils.CreateLogger<RabbitMqBus>();
            var rabbitMqBus = new RabbitMqBus(options, logger);
            var listenerFactory = new RabbitMqListenerFactory(options, CommonUtils.CreateLogger<RabbitMqListenerFactory>());
            var rabbitMqListener = await listenerFactory.Create();

            rabbitMqListener.ShouldNotBeNull();

            var count = 0;
            rabbitMqListener.Subscribe("test", (string m) => Task.FromResult(count++), InteractionType.CompetingConsumers);

            await rabbitMqBus.Publish("test", "1", InteractionType.CompetingConsumers);

            await TimeSpan.FromSeconds(1)
                .StopOnCondition(TimeSpan.FromMilliseconds(100), () => count > 0);

            count.ShouldBe(1);
        }

        [Fact]
        public async Task ListenerFactory_WaitAndDispose_Test()
        {
            var options = CreateOptions(new RabbitMqOptions() {
                HostName = "http://unknown999",
                Port = 111,
                UserName = "1",
                Password = "2"
            });
            var logger = CommonUtils.CreateLogger<RabbitMqBus>();
            var listenerFactory = new RabbitMqListenerFactory(options, CommonUtils.CreateLogger<RabbitMqListenerFactory>());
            var t = listenerFactory.Create();

            await Task.WhenAny(t, Task.Delay(1000));
            t.IsCompleted.ShouldBeFalse();

            listenerFactory.Dispose();
            await Task.WhenAny(t, Task.Delay(1000));

            t.IsCanceled.ShouldBeTrue();
        }

        [Fact]
        public async Task ListenerFactory_SubscribeExtension_Test()
        {
            var options = CreateOptions();
            var logger = CommonUtils.CreateLogger<RabbitMqBus>();
            var rabbitMqBus = new RabbitMqBus(options, logger);
            var listenerFactory = new RabbitMqListenerFactory(options, CommonUtils.CreateLogger<RabbitMqListenerFactory>());

            var count = 0;
            await listenerFactory.Create()
                .Subscribe("test", (string m) => Task.FromResult(Interlocked.Increment(ref count)), InteractionType.PublishSubscribe)
                .Subscribe("test", (string m) => Task.FromResult(Interlocked.Increment(ref count)), InteractionType.PublishSubscribe);

            await rabbitMqBus.Publish("test", "1", InteractionType.PublishSubscribe);

            await TimeSpan.FromSeconds(1)
                .StopOnCondition(TimeSpan.FromMilliseconds(100), () => count >= 2);

            count.ShouldBe(2);
        }

        private IOptions<RabbitMqOptions> CreateOptions(RabbitMqOptions customOptions = null)
        {
            var rabbitMqOptions = customOptions ?? ConfigurationUtils.RabbitMqOptions;
            var options = new Mock<IOptions<RabbitMqOptions>>();
            options.Setup(x => x.Value)
                .Returns(rabbitMqOptions);
            return options.Object;
        }
    }
}

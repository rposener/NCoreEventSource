using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCoreEventServer.Configuration;
using NCoreEventServer.Models;
using NCoreEventServer.Services;
using NCoreEventServer.Services.Results;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServerTests.Services.Hosted
{
    [TestClass]
    public class HostedDeliveryServiceTests
    {
        private ISubscriberQueueStore subscriberQueueStore;
        private IDeliveryService deliveryService;
        private IServiceProvider serviceProvider;
        private ISubscriberStore subscriberStore;
        private ILogger<HostedDeliveryService> logger;
        private IOptions<EventServerOptions> options;

        [TestInitialize]
        public void Setup_Mocks()
        {
            subscriberQueueStore = Mock.Of<ISubscriberQueueStore>();
            subscriberStore = Mock.Of<ISubscriberStore>();
            serviceProvider = Mock.Of<IServiceProvider>();
            logger = Mock.Of<ILogger<HostedDeliveryService>>();
            options = Options.Create(new EventServerOptions());
            deliveryService = Mock.Of<IDeliveryService>();
            SetupScopeMock();
        }

        [TestMethod]
        public async Task Delivery_On_DeliveryStart()
        {
            // Setup
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(ISubscriberQueueStore))))
                .Returns(subscriberQueueStore);
            Mock.Get(subscriberQueueStore)
                .Setup(s => s.SubscriberIdsWithPendingMessages())
                .ReturnsAsync(new string[0]);

            // Test
            var service = new HostedDeliveryService(new TriggerService(), serviceProvider, logger, options);
            var tokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            service.StartAsync(tokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Asserts
            await Task.Delay(20);   // Artificial Delay to allow Processing
            Mock.Get(serviceProvider).Verify(p => p.GetService(It.Is<Type>(v => v == typeof(ISubscriberQueueStore))), Times.AtLeastOnce());
            Mock.Get(subscriberQueueStore).Verify(s => s.SubscriberIdsWithPendingMessages(), Times.AtLeastOnce());
        }

        [TestMethod]
        public async Task Delivery_On_With_Messages()
        {
            // Setup
            SetupScopeMock();
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(ISubscriberQueueStore))))
                .Returns(subscriberQueueStore);
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(IDeliveryService))))
                .Returns(deliveryService);
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(ISubscriberStore))))
                .Returns(subscriberStore);

            Mock.Get(subscriberStore)
                .Setup(s => s.GetSubscriber(It.Is<string>(v => v == "sub1")))
                .ReturnsAsync(new Subscriber { BaseUri = new Uri("https://server1/"), State = SubscriberStates.Active })
                .Verifiable();
            Mock.Get(subscriberStore)
                .Setup(s => s.GetSubscriber(It.Is<string>(v => v == "sub2")))
                .ReturnsAsync(new Subscriber { BaseUri = new Uri("https://server2/"), State = SubscriberStates.Active })
                .Verifiable();
            Mock.Get(subscriberQueueStore)
                .Setup(s => s.SubscriberIdsWithPendingMessages())
                .ReturnsAsync(new[] { "sub1", "sub2" })
                .Verifiable();
            var msg1 = new SubscriberMessage() { DestinationUri = new Uri("https://server1/path"), MessageId = 234L, SubscriberId = "sub1", JsonBody = "test" };
            Mock.Get(subscriberQueueStore)
               .Setup(s => s.PeekMessageAsync(It.Is<string>(v => v == "sub1")))
               .ReturnsAsync(msg1)
               .Verifiable();
            Mock.Get(subscriberQueueStore)
               .Setup(s => s.ClearMessageAsync(It.Is<long>(v => v == 234L)))
               .Callback(() => { msg1 = null; })
               .Returns(Task.CompletedTask)
               .Verifiable();
            var msg2 = new SubscriberMessage() { DestinationUri = new Uri("https://server2/path"), MessageId = 544L, SubscriberId = "sub2", JsonBody= "test" };
            Mock.Get(subscriberQueueStore)
               .Setup(s => s.PeekMessageAsync(It.Is<string>(v => v == "sub2")))
               .ReturnsAsync(msg2)
               .Verifiable();
            Mock.Get(subscriberQueueStore)
               .Setup(s => s.ClearMessageAsync(It.Is<long>(v => v == 544L)))
               .Callback(() => { msg2 = null; })
               .Returns(Task.CompletedTask)
               .Verifiable();
            Mock.Get(deliveryService)
                .Setup(d => d.DeliverMessage(It.Is<Uri>(v => v == new Uri("https://server1/path")), It.IsAny<string>()))
                .ReturnsAsync(DeliveryResult.Success(System.Net.HttpStatusCode.OK, null))
                .Verifiable();
            Mock.Get(deliveryService)
                .Setup(d => d.DeliverMessage(It.Is<Uri>(v => v == new Uri("https://server2/path")), It.IsAny<string>()))
                .ReturnsAsync(DeliveryResult.Success(System.Net.HttpStatusCode.OK, null))
                .Verifiable();

            // Test
            var service = new HostedDeliveryService(new TriggerService(), serviceProvider, logger, options);
            var tokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            service.StartAsync(tokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Asserts
            await Task.Delay(200);   // Artificial Delay to allow Processing
            Mock.Get(subscriberQueueStore).Verify();
            Mock.Get(deliveryService).Verify();
            Mock.Get(subscriberStore).Verify();
        }

        private void SetupScopeMock()
        {
            var factory = Mock.Of<IServiceScopeFactory>();
            Mock.Get(serviceProvider)
                .Setup(s => s.GetService(It.Is<Type>(v => v == typeof(IServiceScopeFactory))))
                .Returns(factory);
            var scope = Mock.Of<IServiceScope>();
            Mock.Get(factory)
                .Setup(f => f.CreateScope())
                .Returns(scope);
            Mock.Get(scope)
                .SetupGet(s => s.ServiceProvider)
                .Returns(serviceProvider);
        }
    }
}

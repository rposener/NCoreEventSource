using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCoreEventServer.Configuration;
using NCoreEventServer.Models;
using NCoreEventServer.Services;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServerTests.Services.Hosted
{
    [TestClass]
    public class HostedProcessingServiceTests
    {
        private IServiceProvider serviceProvider;
        private ILogger<HostedProcessingService> logger;

        IEventQueueStore eventQueueStore;
        IMetadataService metadataService;
        IEventProcessingService eventProcessingService;
        IObjectUpdateService objectUpdateService;

        [TestInitialize]
        public void Setup_Mocks()
        {
            serviceProvider = Mock.Of<IServiceProvider>();
            logger = Mock.Of<ILogger<HostedProcessingService>>();

            SetupScopeMock();

            eventQueueStore = Mock.Of<IEventQueueStore>();
            metadataService = Mock.Of<IMetadataService>();
            eventProcessingService = Mock.Of<IEventProcessingService>();
            objectUpdateService = Mock.Of<IObjectUpdateService>();

            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(IEventQueueStore))))
                .Returns(eventQueueStore);
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(IMetadataService))))
                .Returns(metadataService);
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(IEventProcessingService))))
                .Returns(eventProcessingService);
            Mock.Get(serviceProvider)
                .Setup(p => p.GetService(It.Is<Type>(v => v == typeof(IObjectUpdateService))))
                .Returns(objectUpdateService);
        }

        [TestMethod]
        public async Task ProcessAllMessagesInQueue_TriggersDelivery()
        {
            // Setup
            List<ServerEventMessage> eventsToReturn = new List<ServerEventMessage> {
                new ServerEventMessage {}   // Blank Event (should trigger no processing
            };
            Mock.Get(eventQueueStore)
                .SetupSequence(s => s.NextEventsAsync(It.IsAny<int>()))
                .ReturnsAsync(eventsToReturn.ToArray())
                .ReturnsAsync(new ServerEventMessage[0]);

            var triggerService = new TriggerService();
            bool DeliverySet = false;
            var cts = new CancellationTokenSource();
            var waitingTask = Task.Factory.StartNew(() =>
            {
                triggerService.DeliveryStart.WaitOne();
                DeliverySet = true;
            }, cts.Token);

            // Test
            var options = Options.Create(new EventServerOptions());
            var service = new HostedProcessingService(new TriggerService(), serviceProvider, logger, options);
            await service.ProcessAllMessagesInQueue();
            eventsToReturn.Clear();

            // Assert
            Mock.Get(eventQueueStore).Verify();
            Assert.IsTrue(DeliverySet, "DeliveryStart was not set");

            Mock.Get(objectUpdateService).VerifyNoOtherCalls();
            Mock.Get(metadataService).VerifyNoOtherCalls();
            Mock.Get(eventProcessingService).VerifyNoOtherCalls();
            cts.Cancel();
        }

        [TestMethod]
        public async Task ProcessAllMessagesInQueue_Exceptions_PoisonMessage()
        {
            // Setup
            List<ServerEventMessage> eventsToReturn = new List<ServerEventMessage> {
                new ServerEventMessage { LogId=53232L, Topic = "TopicA", EventJson="{Event-Data-Here}"}   // Blank Event (should trigger no processing
            };
            Mock.Get(eventQueueStore)
                .SetupSequence(s => s.NextEventsAsync(It.IsAny<int>()))
                .ReturnsAsync(eventsToReturn.ToArray())
                .ReturnsAsync(new ServerEventMessage[0]);

            Mock.Get(eventQueueStore)
                .Setup(s => s.PoisonedEventAsync(It.Is<long>(v => v == 53232L)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            Mock.Get(eventProcessingService)
                .Setup(s => s.ProcessEvent(It.Is<string>(v => v == "TopicA"), It.Is<string>(v => v == "{Event-Data-Here}")))
                .ThrowsAsync(new Exception())
                .Verifiable();

            // Test
            var options = Options.Create(new EventServerOptions { AutoDiscoverEvents = false, AutoDiscoverObjectTypes = false });
            var service = new HostedProcessingService(new TriggerService(), serviceProvider, logger, options);
            await service.ProcessAllMessagesInQueue();
            eventsToReturn.Clear();

            // Assert
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(eventProcessingService).Verify();
            Mock.Get(eventQueueStore).Verify();

            Mock.Get(objectUpdateService).VerifyNoOtherCalls();
            Mock.Get(metadataService).VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ProcessAllMessagesInQueue_EventMessage_OnlySends()
        {
            // Setup
            List<ServerEventMessage> eventsToReturn = new List<ServerEventMessage> {
                new ServerEventMessage { LogId=53232L, Topic = "TopicA", EventJson="{Event-Data-Here}"}   // Blank Event (should trigger no processing
            };
            Mock.Get(eventQueueStore)
                .SetupSequence(s => s.NextEventsAsync(It.IsAny<int>()))
                .ReturnsAsync(eventsToReturn.ToArray())
                .ReturnsAsync(new ServerEventMessage[0]);

            Mock.Get(eventQueueStore)
                .Setup(s => s.ClearEventAsync(It.Is<long>(v => v == 53232L)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            Mock.Get(eventProcessingService)
                .Setup(s => s.ProcessEvent(It.Is<string>(v => v == "TopicA"), It.Is<string>(v => v == "{Event-Data-Here}")))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Test
            var options = Options.Create(new EventServerOptions { AutoDiscoverEvents = false, AutoDiscoverObjectTypes = false });
            var service = new HostedProcessingService(new TriggerService(), serviceProvider, logger, options);
            await service.ProcessAllMessagesInQueue();
            eventsToReturn.Clear();

            // Assert
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(eventProcessingService).Verify();
            Mock.Get(eventQueueStore).Verify();

            Mock.Get(objectUpdateService).VerifyNoOtherCalls();
            Mock.Get(metadataService).VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ProcessAllMessagesInQueue_ObjectMessage_OnlySends()
        {
            // Setup
            List<ServerEventMessage> eventsToReturn = new List<ServerEventMessage> {
                new ServerEventMessage { LogId=53232L, ObjectType="ObjectTypeA", ObjectId="235", ObjectUpdate="{Object-Data}"}   // Blank Event (should trigger no processing
            };
            Mock.Get(eventQueueStore)
                .SetupSequence(s => s.NextEventsAsync(It.IsAny<int>()))
                .ReturnsAsync(eventsToReturn.ToArray())
                .ReturnsAsync(new ServerEventMessage[0]);

            Mock.Get(eventQueueStore)
                .Setup(s => s.ClearEventAsync(It.Is<long>(v => v == 53232L)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            Mock.Get(objectUpdateService)
                .Setup(s => s.UpdateObject(It.Is<string>(v => v == "ObjectTypeA"), It.Is<string>(v => v == "235"), It.Is<string>(v => v == "{Object-Data}")))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Test
            var options = Options.Create(new EventServerOptions { AutoDiscoverEvents = false, AutoDiscoverObjectTypes = false });
            var service = new HostedProcessingService(new TriggerService(), serviceProvider, logger, options);
            await service.ProcessAllMessagesInQueue();
            eventsToReturn.Clear();

            // Assert
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(objectUpdateService).Verify();
            Mock.Get(eventQueueStore).Verify();

            Mock.Get(eventProcessingService).VerifyNoOtherCalls();
            Mock.Get(metadataService).VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ProcessAllMessagesInQueue_EventMessage_WithAutoDiscovery()
        {
            // Setup
            List<ServerEventMessage> eventsToReturn = new List<ServerEventMessage> {
                new ServerEventMessage { LogId=53232L, Topic = "TopicA", EventJson="{Event-Data-Here}"}   // Blank Event (should trigger no processing
            };
            Mock.Get(eventQueueStore)
                .SetupSequence(s => s.NextEventsAsync(It.IsAny<int>()))
                .ReturnsAsync(eventsToReturn.ToArray())
                .ReturnsAsync(new ServerEventMessage[0]);

            Mock.Get(eventQueueStore)
                .Setup(s => s.ClearEventAsync(It.Is<long>(v => v == 53232L)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            Mock.Get(eventProcessingService)
                .Setup(s => s.ProcessEvent(It.Is<string>(v => v == "TopicA"), It.Is<string>(v => v == "{Event-Data-Here}")))
                .Returns(Task.CompletedTask)
                .Verifiable();
            Mock.Get(metadataService)
                .Setup(s => s.AutoDiscoverEventsAsync(It.IsAny<ServerEventMessage>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Test
            var options = Options.Create(new EventServerOptions { AutoDiscoverEvents = true, AutoDiscoverObjectTypes = false });
            var service = new HostedProcessingService(new TriggerService(), serviceProvider, logger, options);
            await service.ProcessAllMessagesInQueue();
            eventsToReturn.Clear();

            // Assert
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(eventProcessingService).Verify();
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(metadataService).Verify();

            Mock.Get(objectUpdateService).VerifyNoOtherCalls();
            Mock.Get(metadataService).VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ProcessAllMessagesInQueue_ObjectMessage_WithAutoDiscovery()
        {
            // Setup
            List<ServerEventMessage> eventsToReturn = new List<ServerEventMessage> {
                new ServerEventMessage { LogId=53232L, ObjectType="ObjectTypeA", ObjectId="235", ObjectUpdate="{Object-Data}"}   // Blank Event (should trigger no processing
            };
            Mock.Get(eventQueueStore)
                .SetupSequence(s => s.NextEventsAsync(It.IsAny<int>()))
                .ReturnsAsync(eventsToReturn.ToArray())
                .ReturnsAsync(new ServerEventMessage[0]);

            Mock.Get(eventQueueStore)
                .Setup(s => s.ClearEventAsync(It.Is<long>(v => v == 53232L)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            Mock.Get(objectUpdateService)
                .Setup(s => s.UpdateObject(It.Is<string>(v => v == "ObjectTypeA"), It.Is<string>(v => v == "235"), It.Is<string>(v => v == "{Object-Data}")))
                .Returns(Task.CompletedTask)
                .Verifiable();
            Mock.Get(metadataService)
               .Setup(s => s.AutoDiscoverObjectsAsync(It.IsAny<ServerEventMessage>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

            // Test
            var options = Options.Create(new EventServerOptions { AutoDiscoverEvents = false, AutoDiscoverObjectTypes = true });
            var service = new HostedProcessingService(new TriggerService(), serviceProvider, logger, options);
            await service.ProcessAllMessagesInQueue();
            eventsToReturn.Clear();

            // Assert
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(objectUpdateService).Verify();
            Mock.Get(eventQueueStore).Verify();
            Mock.Get(metadataService).Verify();

            Mock.Get(eventProcessingService).VerifyNoOtherCalls();
            Mock.Get(metadataService).VerifyNoOtherCalls();
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

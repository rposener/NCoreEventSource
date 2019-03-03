using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using System.Threading.Tasks;
using NCoreEventServer.Services;
using NCoreEventServer.Models;

namespace NCoreEventServerTests.Services.Default
{
    [TestClass]
    public class DefaultEventProcessingServiceTests
    {
        private ILogger<DefaultEventProcessingService> logger;
        private IMetadataStore metadataStore;
        private ISubscriberStore subscriberStore;
        private ISubscriberQueueStore subscriberQueueStore;

        [TestInitialize]
        public void Setup_Mocks()
        {
            logger = Mock.Of<ILogger<DefaultEventProcessingService>>();
            metadataStore = Mock.Of<IMetadataStore>();
            subscriberQueueStore = Mock.Of<ISubscriberQueueStore>();
            subscriberStore = Mock.Of<ISubscriberStore>();
        }

        [TestMethod]
        public async Task ProcessEvent_InvalidTopic()
        {
            // setup
            Mock.Get(metadataStore)
                .Setup(s => s.GetTopicsAsync())
                .ReturnsAsync(new[] { "topic1", "topic2" })
                .Verifiable();

            // Test
            var service = new DefaultEventProcessingService(logger, metadataStore, subscriberStore, subscriberQueueStore);
            await service.ProcessEvent("badtopic", "");

            // Assert
            Mock.Get(metadataStore).Verify();
            Mock.Get(metadataStore).VerifyNoOtherCalls();
            Mock.Get(subscriberStore).VerifyNoOtherCalls();
            Mock.Get(subscriberQueueStore).VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ProcessEvent_NoSubsribers()
        {
            // setup
            Mock.Get(metadataStore)
                .Setup(s => s.GetTopicsAsync())
                .ReturnsAsync(new[] { "topic1", "topic2" })
                .Verifiable();
            Mock.Get(subscriberStore)
                .Setup(s => s.GetSubscriptionsToTopic(It.Is<string>(v => v == "topic1")))
                .ReturnsAsync(new SubscriptionDetails[0])
                .Verifiable();

            // Test
            var service = new DefaultEventProcessingService(logger, metadataStore, subscriberStore, subscriberQueueStore);
            await service.ProcessEvent("topic1", "");

            // Assert
            Mock.Get(metadataStore).Verify();
            Mock.Get(metadataStore).VerifyNoOtherCalls();
            Mock.Get(subscriberStore).Verify();
            Mock.Get(subscriberStore).VerifyNoOtherCalls();
            Mock.Get(subscriberQueueStore).VerifyNoOtherCalls();
        }


        [TestMethod]
        public async Task ProcessEvent_AddsMessages()
        {
            // setup
            Mock.Get(metadataStore)
                .Setup(s => s.GetTopicsAsync())
                .ReturnsAsync(new[] { "topic1", "topic2" })
                .Verifiable();
            Mock.Get(subscriberStore)
                .Setup(s => s.GetSubscriptionsToTopic(It.Is<string>(v => v == "topic1")))
                .ReturnsAsync(new[] {
                    new SubscriptionDetails{ BaseUri= new Uri("https://server-test/"), RelativePath = "/path/2352" },
                    new SubscriptionDetails {  BaseUri= new Uri("https://another-test"), RelativePath = "some-long-post-to-me" }
                    })
                .Verifiable();
            Mock.Get(subscriberQueueStore)
                .Setup(q => q.EnqueueMessageAsync(It.Is<SubscriberMessage>(v => v.DestinationUri == new Uri("https://server-test/path/2352") && v.JsonBody == "{test}")))
                .Returns(Task.CompletedTask)
                .Verifiable();
            Mock.Get(subscriberQueueStore)
                .Setup(q => q.EnqueueMessageAsync(It.Is<SubscriberMessage>(v => v.DestinationUri == new Uri("https://another-test/some-long-post-to-me") && v.JsonBody == "{test}")))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Test
            var service = new DefaultEventProcessingService(logger, metadataStore, subscriberStore, subscriberQueueStore);
            await service.ProcessEvent("topic1", "{test}");

            // Assert
            Mock.Get(metadataStore).Verify();
            Mock.Get(metadataStore).VerifyNoOtherCalls();
            Mock.Get(subscriberStore).Verify();
            Mock.Get(subscriberStore).VerifyNoOtherCalls();
            Mock.Get(subscriberQueueStore).Verify();
            Mock.Get(subscriberQueueStore).VerifyNoOtherCalls();
        }
    }
}

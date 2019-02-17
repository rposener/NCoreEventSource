using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCoreEventServer.Models;
using NCoreEventServer.Services;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServerTests.Services.Default
{
    [TestClass]
    public class DefaultMetadataServiceTests
    {
        private ILogger<DefaultMetadataService> logger;
        private IMetadataStore metadataStore;

        [TestInitialize]
        public void Setup_Mocks()
        {
            logger = Mock.Of<ILogger<DefaultMetadataService>>();
            metadataStore = Mock.Of<IMetadataStore>();
        }

        [TestMethod]
        public async Task AutoDiscoverEventAsync_AddsTopic()
        {
            // setup 
            var testEvent = new ServerEventMessage
            {
                Topic = "Topic1"
            };
            Mock.Get(metadataStore)
                .Setup(s => s.AddTopicAsync(It.Is<string>(v => v == "Topic1")))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Test
            var service = new DefaultMetadataService(logger, metadataStore);
            await service.AutoDiscoverEventsAsync(testEvent);

            // Assert
            Mock.Get(metadataStore).Verify();
        }


        [TestMethod]
        public async Task AutoDiscoverEventAsync_OnlyNewTopics_Added()
        {
            // setup 
            var testEvent1 = new ServerEventMessage
            {
                Topic = "Topic1"
            };
            var testEvent2 = new ServerEventMessage
            {
                Topic = "Topic2"
            };

            Mock.Get(metadataStore)
                .Setup(s => s.AddTopicAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            Mock.Get(metadataStore)
                .Setup(s => s.GetTopicsAsync())
                .ReturnsAsync(new[] { "Topic1" })
                .Verifiable();

            // Test
            var service = new DefaultMetadataService(logger, metadataStore);
            await service.AutoDiscoverEventsAsync(testEvent1);
            await service.AutoDiscoverEventsAsync(testEvent2);

            // Assert
            Mock.Get(metadataStore).Verify();
            Mock.Get(metadataStore).Verify(s => s.AddTopicAsync(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task AutoDiscoverEventAsync_AddsObjectType()
        {
            // setup 
            var testEvent = new ServerEventMessage
            {
                ObjectType = "Customer"
            };
            Mock.Get(metadataStore)
                .Setup(s => s.AddObjectTypeAsync(It.Is<string>(v => v == "Customer")))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Test
            var service = new DefaultMetadataService(logger, metadataStore);
            await service.AutoDiscoverObjectsAsync(testEvent);

            // Assert
            Mock.Get(metadataStore).Verify();
        }


        [TestMethod]
        public async Task AutoDiscoverObjectTypeAsync_OnlyNewObjectTypes_Added()
        {
            // setup 
            var testEvent1 = new ServerEventMessage
            {
                ObjectType = "Customer"
            };
            var testEvent2 = new ServerEventMessage
            {
                ObjectType = "Project"
            };
            Mock.Get(metadataStore)
                .Setup(s => s.AddObjectTypeAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            Mock.Get(metadataStore)
                .Setup(s => s.GetObjectTypesAsync())
                .ReturnsAsync(new[] { "Customer" })
                .Verifiable();

            // Test
            var service = new DefaultMetadataService(logger, metadataStore);
            await service.AutoDiscoverObjectsAsync(testEvent1);
            await service.AutoDiscoverObjectsAsync(testEvent2);

            // Assert
            Mock.Get(metadataStore).Verify();
            Mock.Get(metadataStore).Verify(s => s.AddObjectTypeAsync(It.IsAny<string>()), Times.Once());
        }
    }
}

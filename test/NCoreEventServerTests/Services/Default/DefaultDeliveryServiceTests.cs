using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCoreEventServer.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCoreEventServerTests.Services.Default
{
    [TestClass]
    public class DefaultDeliveryServiceTests
    {
        private ILogger<DefaultDeliveryService> logger;
        private MockHttpHandler httpHandler;
        private HttpClient httpClient;

        [TestInitialize]
        public void Setup_Test_Mocks()
        {
            logger = Mock.Of<ILogger<DefaultDeliveryService>>();
            httpHandler = new MockHttpHandler();
            httpClient = new HttpClient(httpHandler);
        }

        [TestMethod]
        public async Task DeliverMessage_Successful()
        {
            // Setup
            var testUri = new Uri("https://server/path");
            var testBody = "Test Value";
            httpHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.NoContent));

            // Test
            var service = new DefaultDeliveryService(logger, httpClient);
            var result = await service.DeliverMessage(testUri, testBody);

            // Assert
            Assert.AreEqual(true, result.SentSuccessfully, "result reported failure");
            Assert.AreEqual(204, result.StatusCode, "result reported wrong status");
            Assert.AreEqual("Message Delivered", result.Message, "incorrect message");

        }

        [TestMethod]
        public async Task DeliverMessage_BadRequest()
        {
            // Setup
            var testUri = new Uri("https://server/path");
            var testBody = "Test Value";
            httpHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.BadRequest));

            // Test
            var service = new DefaultDeliveryService(logger, httpClient);
            var result = await service.DeliverMessage(testUri, testBody);

            // Assert
            Assert.AreEqual(false, result.SentSuccessfully, "result reported failure");
            Assert.AreEqual(400, result.StatusCode, "result reported wrong status");
        }


        [TestMethod]
        public async Task DeliverMessage_Redirect()
        {
            // Setup
            var testUri = new Uri("https://server/path");
            var testBody = "Test Value";
            httpHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.PermanentRedirect));

            // Test
            var service = new DefaultDeliveryService(logger, httpClient);
            var result = await service.DeliverMessage(testUri, testBody);

            // Assert
            Assert.AreEqual(false, result.SentSuccessfully, "result reported failure");
            Assert.AreEqual(308, result.StatusCode, "result reported wrong status");
        }


        [TestMethod]
        public async Task DeliverMessage_ServerError()
        {
            // Setup
            var testUri = new Uri("https://server/path");
            var testBody = "Test Value";
            httpHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.InsufficientStorage));

            // Test
            var service = new DefaultDeliveryService(logger, httpClient);
            var result = await service.DeliverMessage(testUri, testBody);

            // Assert
            Assert.AreEqual(false, result.SentSuccessfully, "result reported failure");
            Assert.AreEqual(507, result.StatusCode, "result reported wrong status");
        }
    }
}


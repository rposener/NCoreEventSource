using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCoreEventServer.Services;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServerTests.Services.Default
{
    [TestClass]
    public class DefaultInjestionServiceTests
    {
        private IEventQueueStore eventQueueStore;
        private ILogger<DefaultInjestionService> logger;

        [TestInitialize]
        public void Setup_Mocks()
        {
            eventQueueStore = Mock.Of<IEventQueueStore>();
            logger = Mock.Of<ILogger<DefaultInjestionService>>();
        }

        [DataRow("GET")]
        [DataRow("PUT")]
        [DataRow("PATCH")]
        [DataRow("DELETE")]
        [DataTestMethod]
        public async Task InjestRequest_NotPost(string method)
        {
            // Setup
            var context = new DefaultHttpContext();
            context.Request.Method = method;

            // Test 
            var service = new DefaultInjestionService(new TriggerService(), eventQueueStore, logger);
            var result = await service.InjestRequest(context);

            // Assert
            Assert.AreEqual(405, result.StatusCode, "Method not allowed not set");
        }

        [DataRow("application/js")]
        [DataRow("text/csv")]
        [DataRow("html/text")]
        [DataRow("hacker/me")]
        [DataTestMethod]
        public async Task InjestRequest_NotJsonType(string contentType)
        {
            // Setup
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = contentType;

            // Test 
            var service = new DefaultInjestionService(new TriggerService(), eventQueueStore, logger);
            var result = await service.InjestRequest(context);

            // Assert
            Assert.AreEqual(415, result.StatusCode, "Unsupported Media Type not set");
        }

        [DataRow("")]
        [DataRow("javascript:hackthis();")]
        [DataRow("<html></html>")]
        [DataRow("[]")]
        [DataTestMethod]
        public async Task InjestReqest_InvalidMessage(string body)
        {
            // Setup
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            // Test 
            var service = new DefaultInjestionService(new TriggerService(), eventQueueStore, logger);
            var result = await service.InjestRequest(context);

            // Assert
            Assert.AreEqual(400, result.StatusCode, "Bad Request not set");
        }

        [DataRow("{\"Topic\":\"Test\"}")]
        [DataRow("{\"Topic\":\"Test\",\"EventJson\":\"{something:235}\"}")]
        [DataRow("{\"Topic\":\"Test\",\"ExtraField\":\"{something:235}\"}")]
        [DataRow("{\"Topic\":\"Test\",\"objectType\":\"MyType\"}")]
        [DataTestMethod]
        public async Task InjestReqest_ValidMessage(string body)
        {
            // Setup
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            // Test 
            var service = new DefaultInjestionService(new TriggerService(), eventQueueStore, logger);
            var result = await service.InjestRequest(context);

            // Assert
            Assert.AreEqual(202, result.StatusCode, "Bad Request not set");
        }
    }
}

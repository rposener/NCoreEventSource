using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCoreEventServer.Models;
using NCoreEventServer.SqlStore.Models;
using NCoreEventServer.SqlStore.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.SqlStore.Tests.Stores
{
    [TestClass]
    public class SqlEventQueueStoreTests
    {
        private IMapper mapper;
        private ILogger<SqlEventQueueStore> logger;
        private EventServerContext context;
        private DbContextOptions options;

        [TestInitialize]
        public void Init_Tests()
        {
            mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
            logger = Mock.Of<ILogger<SqlEventQueueStore>>();
            options = new DbContextOptionsBuilder<EventServerContext>().UseInMemoryDatabase(databaseName: "Test_DB").Options;
            context = new EventServerContext(options);
            context.Database.EnsureCreated();
        }

        [TestCleanup]
        public void Cleanup_Tests()
        {
            context.Database.EnsureDeleted();
        }


        [TestMethod]
        public async Task EnqueueEventAsync_Adds_And_Saves()
        {
            // Setup
            var message = new ServerEventMessage { LogId = 4, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson="EVENT_JSON" };

            // Test
            var store = new SqlEventQueueStore(mapper, logger, context);
            var newId = await store.EnqueueEventAsync(message);

            // Assert
            Assert.AreEqual(4L, newId, "New Id was not returned");
            var testcontext = new EventServerContext(options);
            var items = await testcontext.EventMessages.CountAsync();
            Assert.AreEqual(1, items);

        }

        [TestMethod]
        public async Task DequeueEventAsync_Removes_And_Saves()
        {
            // Setup
            var setupcontext = new EventServerContext(options);
            setupcontext.AddRange(
                new ServerEventMessageEntity { LogId = 4, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson = "EVENT_JSON" });
            await setupcontext.SaveChangesAsync();

            // Test
            var store = new SqlEventQueueStore(mapper, logger, context);
            await store.DequeueEventAsync(4);

            // Assert
            var testcontext = new EventServerContext(options);
            var items = await testcontext.EventMessages.CountAsync();
            Assert.AreEqual(0, items);

        }

        [TestMethod]
        public async Task PeekEventAsync_Returns_And_Keeps()
        {
            // Setup
            var setupcontext = new EventServerContext(options);
            setupcontext.AddRange(
                new ServerEventMessageEntity { LogId = 4, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson = "EVENT_JSON" });
            await setupcontext.SaveChangesAsync();

            // Test
            var store = new SqlEventQueueStore(mapper, logger, context);
            var nextEvent = await store.PeekEventAsync();

            // Assert
            Assert.AreEqual(4, nextEvent.LogId);
            var testcontext = new EventServerContext(options);
            var items = await testcontext.EventMessages.CountAsync();
            Assert.AreEqual(1, items);

        }

        [TestMethod]
        public async Task PoisonedEventAsync_Moves_To_Poison()
        {
            // Setup
            var setupcontext = new EventServerContext(options);
            setupcontext.AddRange(
                new ServerEventMessageEntity { LogId = 4, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson = "EVENT_JSON" });
            await setupcontext.SaveChangesAsync();

            // Test
            var store = new SqlEventQueueStore(mapper, logger, context);
            await store.PoisonedEventAsync(4);

            // Assert
            var testcontext = new EventServerContext(options);
            var items = await testcontext.EventMessages.CountAsync();
            var poison = await testcontext.PoisonMessages.CountAsync();
            Assert.AreEqual(0, items);
            Assert.AreEqual(1, poison);

        }


        [TestMethod]
        public async Task PoisonEventsAsync_Moves_To_Poison()
        {
            // Setup
            var setupcontext = new EventServerContext(options);
            setupcontext.AddRange(
                new PoisonEventMessageEntity { LogId = 4, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson = "EVENT_JSON" },
                new PoisonEventMessageEntity { LogId = 5, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson = "EVENT_JSON" },
                new PoisonEventMessageEntity { LogId = 6, ObjectId = "TEST", ObjectType = "TESTTYPE", Topic = "TOPIC", EventJson = "EVENT_JSON" });
            await setupcontext.SaveChangesAsync();

            // Test
            var store = new SqlEventQueueStore(mapper, logger, context);
            var items = await store.PoisonEventsAsync(2);

            // Assert
            Assert.AreEqual(2, items.Count());

        }
    }
}

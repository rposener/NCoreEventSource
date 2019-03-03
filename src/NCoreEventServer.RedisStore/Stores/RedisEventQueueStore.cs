using AutoMapper;
using Microsoft.Extensions.Logging;
using NCoreEventServer.Models;
using NCoreEventServer.RedisStore.Models;
using NCoreEventServer.Stores;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.RedisStore.Stores
{
    public class RedisEventQueueStore : IEventQueueStore
    {
        const string NCORE_EVENT_LIST = "urn:ncore:events";
        const string NCORE_EVENT_POISON_LIST = "urn:ncore:events:poison";
        private readonly IMapper mapper;
        private readonly ILogger<RedisEventQueueStore> logger;
        private readonly IRedisClientsManager clientsManager;

        public RedisEventQueueStore(
            IMapper mapper,
            ILogger<RedisEventQueueStore> logger,
            IRedisClientsManager clientsManager)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientsManager = clientsManager ?? throw new ArgumentNullException(nameof(clientsManager));
        }

        public Task DequeueEventAsync(long id)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var eventMessageClient = redis.As<ServerEventMessageEntity>();
                var entity = eventMessageClient.GetById(id);
                eventMessageClient.Lists[NCORE_EVENT_LIST].Remove(entity);
            }
            return Task.CompletedTask;
        }

        public Task<long> EnqueueEventAsync(ServerEventMessage message)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var eventMessageClient = redis.As<ServerEventMessageEntity>();
                message.LogId = eventMessageClient.GetNextSequence();
                var entity = mapper.Map<ServerEventMessageEntity>(message);
                eventMessageClient.Lists[NCORE_EVENT_LIST].Add(entity);
            }
            return Task.FromResult(message.LogId);
        }

        public Task<ServerEventMessage> PeekEventAsync()
        {
            ServerEventMessage eventMessage;
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var eventMessageClient = redis.As<ServerEventMessageEntity>();
                var eventList = eventMessageClient.Lists[NCORE_EVENT_LIST];
                var entity = eventMessageClient.GetItemFromList(eventList, 0);
                eventMessage = mapper.Map<ServerEventMessage>(entity);
            }
            return Task.FromResult(eventMessage);
        }

        public Task PoisonedEventAsync(long id)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var eventMessageClient = redis.As<ServerEventMessageEntity>();
                var eventList = eventMessageClient.Lists[NCORE_EVENT_LIST];
                var entity = eventMessageClient.GetById(id);
                var poisonList = eventMessageClient.Lists[NCORE_EVENT_POISON_LIST];
                using (var tran = eventMessageClient.CreateTransaction())
                {
                    eventMessageClient.RemoveItemFromList(eventList, entity);
                    eventMessageClient.AddItemToList(eventList, entity);
                    tran.Commit();
                }
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ServerEventMessage>> PoisonEventsAsync(int Max)
        {
            IEnumerable<ServerEventMessage> results;
            using (IRedisClient redis = clientsManager.GetClient())
            {
                var eventMessageClient = redis.As<ServerEventMessageEntity>();
                var eventList = eventMessageClient.Lists[NCORE_EVENT_LIST];
                var entities = eventMessageClient.Lists[NCORE_EVENT_POISON_LIST].GetRange(0, Max);
                results = Mapper.Map<IEnumerable<ServerEventMessage>>(entities);
            }
            return Task.FromResult(results);
        }
    }
}

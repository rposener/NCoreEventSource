using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;

namespace NCoreEventServer.Services
{
    public class DefaultMetadataService : IMetadataService
    {
        public Task AutoDiscoverEventsAsync(EventMessage eventMessage)
        {
            throw new NotImplementedException();
        }

        public Task AutoDiscoverObjectsAsync(EventMessage eventMessage)
        {
            throw new NotImplementedException();
        }

        public Task RegisterGlobalEventAsync(string EventName)
        {
            throw new NotImplementedException();
        }

        public Task RegisterObjectEventAsync(string ObjectType, string EventName)
        {
            throw new NotImplementedException();
        }

        public Task RegisterObjectTypeAsync(string ObjectType)
        {
            throw new NotImplementedException();
        }

        public Task RegisterTopicAsync(string Topic)
        {
            throw new NotImplementedException();
        }

        public Task RegisterTopicEventAsync(string Topic, string EventName)
        {
            throw new NotImplementedException();
        }
    }
}

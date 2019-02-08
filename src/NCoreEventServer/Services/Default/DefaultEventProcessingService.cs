using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class DefaultEventProcessingService : IEventProcessingService
    {
        private readonly ILogger<DefaultEventProcessingService> logger;

        public DefaultEventProcessingService(ILogger<DefaultEventProcessingService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public Task ProcessEvent(string Topic, string Event, string EventData)
        {
            logger.LogInformation($"Processing ({Topic},{Event})");
            return Task.CompletedTask;
        }
    }
}

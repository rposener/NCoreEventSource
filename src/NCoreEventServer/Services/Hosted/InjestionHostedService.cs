using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCoreEventServer.Configuration;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class InjestionHostedService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TriggerService triggerService;
        private readonly ILogger<InjestionHostedService> logger;
        private readonly IOptions<EventServerOptions> options;

        public InjestionHostedService(
            IServiceProvider serviceProvider,
            TriggerService triggerService, 
            ILogger<InjestionHostedService> logger,
            IOptions<EventServerOptions> options)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Continually injests <seealso cref="EventMessage"/> messages 
        /// until <paramref name="stoppingToken"/> requests Stop
        /// starts instantly upon <seealso cref="TriggerService.InjestionStart"/> reset 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Starting Injestion!");
                await ProcessAllMessagesInQueue();
                logger.LogInformation("Injestion Caught up, Waiting for More Events");
                triggerService.InjestionStart.WaitOne(TimeSpan.FromSeconds(15), true);
            }
        }

        /// <summary>
        /// Processes all Pending <seealso cref="EventMessage"/> messages 
        /// in the <seealso cref="IEventQueueStore"/>
        /// </summary>
        /// <returns></returns>
        private async Task ProcessAllMessagesInQueue()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var eventQueueStore = scope.ServiceProvider.GetRequiredService<IEventQueueStore>();
                IEnumerable<EventMessage> pendingEvents = await eventQueueStore.NextEventsAsync(8);
                while (pendingEvents.Any((_) => { return true; }))
                {
                    foreach (var pendingEvent in pendingEvents)
                    {
                        var processingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();
                        await processingService.ProcessEvent(pendingEvent.Topic, pendingEvent.Event, pendingEvent.EventJson);
                        await eventQueueStore.ClearEventAsync(pendingEvent.LogId);
                    }
                    pendingEvents = await eventQueueStore.NextEventsAsync(options.Value.InjestionBatchSize);
                }
            }
        }
    }
}

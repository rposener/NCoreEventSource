﻿using Microsoft.Extensions.DependencyInjection;
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
    /// <summary>
    /// This service runs on background thread and calls the
    /// <seealso cref="IEventProcessingService"/> and <seealso cref="IObjectUpdateService"/> for each <seealso cref="EventMessage"/>
    /// in the <seealso cref="IEventQueueStore"/>.
    /// </summary>
    public class HostedProcessingService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TriggerService triggerService;
        private readonly ILogger<HostedProcessingService> logger;
        private readonly IOptions<EventServerOptions> options;

        public HostedProcessingService(
            IServiceProvider serviceProvider,
            TriggerService triggerService, 
            ILogger<HostedProcessingService> logger,
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
        /// starts instantly upon <seealso cref="TriggerService.ProcessingStart"/> reset 
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
                triggerService.ProcessingStart.WaitOne(TimeSpan.FromSeconds(15), true);
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
                // Load store and other services necessary for coordination
                var eventQueueStore = scope.ServiceProvider.GetRequiredService<IEventQueueStore>();
                var metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();
                var processingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();
                var objectUpdateService = scope.ServiceProvider.GetRequiredService<IObjectUpdateService>();

                IEnumerable<EventMessage> pendingEvents = await eventQueueStore.NextEventsAsync(options.Value.InjestionBatchSize);
                while (pendingEvents.Any((_) => { return true; }))
                {
                    foreach (var pendingEvent in pendingEvents)
                    {
                        try
                        {
                            logger.LogDebug($"Processing Event #{pendingEvent.LogId}");
                            // Start with Metadata updates
                            if (options.Value.AutoDiscoverEvents)
                                await metadataService.AutoDiscoverEventsAsync(pendingEvent);
                            if (options.Value.AutoDiscoverObjectTypes)
                                await metadataService.AutoDiscoverObjectsAsync(pendingEvent);

                            // Process the Event (if Topic is Set)
                            if (!pendingEvent.IsEventMessage())
                                await processingService.ProcessEvent(pendingEvent.Topic, pendingEvent.Event, pendingEvent.EventJson);

                            // Process the Object (if ObjectType is set)
                            if (!pendingEvent.IsObjectMessage())
                                await objectUpdateService.UpdateObject(pendingEvent.ObjectType, pendingEvent.ObjectId, pendingEvent.ObjectUpdate);
                        }
                        catch (Exception ex)
                        {
                            logger.LogCritical(ex, ex.Message);
                            await eventQueueStore.PoisonedEventAsync(pendingEvent.LogId);
                            continue;
                        }
                        // Clear the Event as Processed
                        await eventQueueStore.ClearEventAsync(pendingEvent.LogId);
                    }
                    
                    // Trigger Delivery
                    triggerService.DeliveryStart.Set();

                    // Keep checking for more Events
                    pendingEvents = await eventQueueStore.NextEventsAsync(options.Value.InjestionBatchSize);
                }
            }
        }
    }
}

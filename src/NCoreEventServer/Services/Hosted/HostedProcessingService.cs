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
    /// <seealso cref="IEventProcessingService"/> and <seealso cref="IObjectUpdateService"/> for each <seealso cref="ServerEventMessage"/>
    /// in the <seealso cref="IEventQueueStore"/>.
    /// </summary>
    public class HostedProcessingService : BackgroundService
    {
        private readonly TriggerService triggerService;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<HostedProcessingService> logger;
        private readonly IOptions<EventServerOptions> options;

        public HostedProcessingService(
            TriggerService triggerService,
            IServiceProvider serviceProvider,
            ILogger<HostedProcessingService> logger,
            IOptions<EventServerOptions> options)
        {
            this.triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            logger.LogInformation(nameof(HostedProcessingService) + " is created.");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(HostedProcessingService) + " has started.");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(HostedProcessingService) + " has stopped.");
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Continually injests <seealso cref="ServerEventMessage"/> messages 
        /// until <paramref name="stoppingToken"/> requests Stop
        /// starts instantly upon <seealso cref="TriggerService.ProcessingStart"/> reset 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Starting Injestion!");
                    await ProcessAllMessagesInQueue();
                    logger.LogInformation("Injestion Caught up, Waiting for More Events");
                    await triggerService.ProcessingStart.WaitOneAsync(TimeSpan.FromSeconds(15));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Processes all Pending <seealso cref="ServerEventMessage"/> messages 
        /// in the <seealso cref="IEventQueueStore"/>
        /// </summary>
        /// <returns></returns>
        public async Task ProcessAllMessagesInQueue()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                // Load store and other services necessary for coordination
                var eventQueueStore = scope.ServiceProvider.GetRequiredService<IEventQueueStore>();
                var metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();
                var processingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();
                var objectUpdateService = scope.ServiceProvider.GetRequiredService<IObjectUpdateService>();

                IEnumerable<ServerEventMessage> pendingEvents = await eventQueueStore.NextEventsAsync(options.Value.InjestionBatchSize);
                if (pendingEvents != null && pendingEvents.Any())
                {
                    foreach (var pendingEvent in pendingEvents)
                    {
                        try
                        {
                            logger.LogDebug($"Processing Event #{pendingEvent.LogId}");
                            // Start with Metadata updates
                            if (options.Value.AutoDiscoverEvents && pendingEvent.IsServerEventMessage())
                                await metadataService.AutoDiscoverEventsAsync(pendingEvent);
                            if (options.Value.AutoDiscoverObjectTypes && pendingEvent.IsObjectMessage())
                                await metadataService.AutoDiscoverObjectsAsync(pendingEvent);

                            // Process the Event (if Topic is Set)
                            if (pendingEvent.IsServerEventMessage())
                                await processingService.ProcessEvent(pendingEvent.Topic, pendingEvent.EventJson);

                            // Process the Object (if ObjectType is set)
                            if (pendingEvent.IsObjectMessage())
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

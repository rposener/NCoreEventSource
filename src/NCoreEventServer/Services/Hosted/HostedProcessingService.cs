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
    /// <summary>
    /// This service runs on background thread and calls the
    /// <seealso cref="IEventProcessingService"/> and <seealso cref="IObjectUpdateService"/> for each <seealso cref="ServerEventMessage"/>
    /// in the <seealso cref="IEventQueueStore"/>.
    /// </summary>
    public class HostedProcessingService : IHostedService, IDisposable
    {
        private Task workerTask;
        private readonly CancellationTokenSource stoppingToken = new CancellationTokenSource();
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

        /// <summary>
        /// Implementation of <seealso cref="IHostedService"/> to start processing
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(HostedProcessingService) + $" is starting.");
            if (!cancellationToken.IsCancellationRequested)
            {
                workerTask = ProcessEvents();
                logger.LogInformation(nameof(HostedProcessingService) + " has started.");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Implementation of <seealso cref="IHostedService"/> to stop processing
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(HostedProcessingService) + " is stopping.");

            // Stop called without start
            if (workerTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the worker Tasks
                stoppingToken.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(workerTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
            logger.LogInformation(nameof(HostedProcessingService) + " has stopped.");           
        }

        /// <summary>
        /// Continually injests <seealso cref="ServerEventMessage"/> messages 
        /// until <paramref name="stoppingToken"/> requests Stop
        /// starts instantly upon <seealso cref="TriggerService.ProcessingStart"/> reset 
        /// </summary>
        /// <returns></returns>
        public async Task ProcessEvents()
        {
            Random delayJitter = new Random();
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Starting Injestion!");
                    await ProcessAllMessagesInQueue();
                    logger.LogInformation("Injestion Caught up, Waiting for More Events");

                    // Default Pause when caught-up before checking again (unless ResetEvent is Set)
                    await triggerService.ProcessingStart.WaitOneAsync(TimeSpan.FromSeconds(1));
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

                ServerEventMessage pendingEvent = await eventQueueStore.PeekEventAsync();
                if (pendingEvent != null)
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

                        // Clear the Event as Processed
                        await eventQueueStore.DequeueEventAsync(pendingEvent.LogId);

                        // Trigger Delivery
                        triggerService.DeliveryStart.Set();
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, ex.Message);
                        await eventQueueStore.PoisonedEventAsync(pendingEvent.LogId);
                    }
                }

                // Stop immediately if requested
                if (stoppingToken.IsCancellationRequested)
                    return;

                // Keep checking for more Events
                pendingEvent = await eventQueueStore.PeekEventAsync();
            }
        }

        public void Dispose()
        {
            // Signal cancellation to the worker Tasks
            stoppingToken.Cancel();
        }
    }
}

﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCoreEventServer.Configuration;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    /// <summary>
    /// Manages delivering <seealso cref="SubscriberMessage"/>s to <seealso cref="Subscriber"/>s,
    /// via the <seealso cref="IDeliveryService"/> and Updates the status of <seealso cref="Subscriber"/>s
    /// in the <seealso cref="ISubscriberStore"/>.
    /// </summary>
    public class HostedDeliveryService : IHostedService, IDisposable
    {
        private Task workerTask;
        private readonly CancellationTokenSource stoppingToken = new CancellationTokenSource();
        private readonly TriggerService triggerService;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<HostedDeliveryService> logger;
        private readonly IOptions<EventServerOptions> options;

        public HostedDeliveryService(
            TriggerService triggerService,
            IServiceProvider serviceProvider,
            ILogger<HostedDeliveryService> logger,
            IOptions<EventServerOptions> options)
        {
            this.triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            logger.LogInformation(nameof(HostedDeliveryService) + " is created.");
        }

        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(HostedDeliveryService) + " has started.");
            if (!cancellationToken.IsCancellationRequested)
            {
                workerTask = DeliveryLoop();
                logger.LogInformation(nameof(HostedDeliveryService) + " has started.");
            }
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(nameof(HostedDeliveryService) + " is stopping.");

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
            logger.LogInformation(nameof(HostedDeliveryService) + " has stopped.");
        }

        /// <summary>
        /// Delivers all messages in <seealso cref="ISubscriberQueueStore"/> using parallel implementations of 
        /// <seealso cref="IDeliveryService"/> to send the messages.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected async Task DeliveryLoop()
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Starting Delivery!");
                    await DeliverAllMessagesInQueue();
                    logger.LogInformation("Delivery Caught up, Waiting for More Events");

                    // Default Pause when caught-up before checking again (unless ResetEvent is Set)
                    await triggerService.DeliveryStart.WaitOneAsync(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Delivers all Messages to Subscribers with Pending Messages in the Queue
        /// </summary>
        /// <returns></returns>
        private async Task DeliverAllMessagesInQueue()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var queueService = scope.ServiceProvider.GetRequiredService<ISubscriberQueueStore>();

                var subscribersToSend = await queueService.SubscriberIdsWithPendingMessages();

                if (subscribersToSend != null && subscribersToSend.Any())
                {
                    // Send to 4 subscribers concurrently
                    await subscribersToSend.ForEachAsync(4, DeliverMessagesToSubscriber);
                }
            }
        }

        /// <summary>
        /// Delivers all Pending Messages for a Single Subscriber
        /// </summary>
        /// <param name="SubscriberId"></param>
        /// <returns></returns>
        private async Task DeliverMessagesToSubscriber(string SubscriberId)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var subscriberStore = scope.ServiceProvider.GetRequiredService<ISubscriberStore>();
                var queueService = scope.ServiceProvider.GetRequiredService<ISubscriberQueueStore>();
                var deliveryService = scope.ServiceProvider.GetRequiredService<IDeliveryService>();

                Subscriber subscriber = await subscriberStore.GetSubscriber(SubscriberId);

                // If the Subscriber is Inactive, just log and stop
                if (subscriber.State == SubscriberStates.Inactive)
                {
                    logger.LogWarning($"The Subscriber '{SubscriberId}' is currently marked inactive");
                    return;
                }

                // Deliver each message in sequence until all messages are deliverd
                var nextMessage = await queueService.PeekMessageAsync(SubscriberId);
                while (nextMessage != null)
                {
                    var result = await deliveryService.DeliverMessage(nextMessage.DestinationUri, nextMessage.JsonBody);
                    if (!result.SentSuccessfully)
                    {
                        // Update the Subscriber with Failure Details and Exit
                        logger.LogWarning($"The Subscriber '{SubscriberId}' is down");
                        subscriber = await subscriberStore.GetSubscriber(SubscriberId);
                        subscriber.State = SubscriberStates.Down;
                        subscriber.LastFailure = DateTime.UtcNow;
                        subscriber.FailureCount++;
                        await subscriberStore.UpdateSubscriberAsync(subscriber);
                        return;
                    }
                    await queueService.ClearMessageAsync(nextMessage.MessageId);

                    // Stop if Requested
                    if (stoppingToken.IsCancellationRequested)
                        return;

                    // Attempt to get the next message
                    nextMessage = await queueService.PeekMessageAsync(SubscriberId);
                }

                // Check the Subscriber Status
                // Update the Subscriber with Failure Details and Exit
                subscriber = await subscriberStore.GetSubscriber(SubscriberId);
                if (subscriber.State == Models.SubscriberStates.Down)
                {
                    logger.LogInformation($"The Subscriber '{SubscriberId}' is active again");
                    subscriber.State = SubscriberStates.Active;
                    subscriber.LastFailure = null;
                    subscriber.FailureCount = 0;
                    await subscriberStore.UpdateSubscriberAsync(subscriber);
                }
            }
        }

        public void Dispose()
        {
            // Signal cancellation to the worker Tasks
            stoppingToken.Cancel();
        }
    }
}

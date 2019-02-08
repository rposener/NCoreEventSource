using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class InjestionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TriggerService triggerService;
        private readonly ILogger<InjestionBackgroundService> logger;

        public InjestionBackgroundService(
            IServiceProvider serviceProvider,
            TriggerService triggerService, 
            ILogger<InjestionBackgroundService> logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                triggerService.InjestionStart.WaitOne();
                logger.LogInformation("Starting Injestion!");
                using (var scope = serviceProvider.CreateScope())
                {
                    var eventQueueStore = scope.ServiceProvider.GetRequiredService<IEventQueueStore>();
                    var pendingEvents = await eventQueueStore.NextEventsAsync(8);
                    foreach (var pendingEvent in pendingEvents)
                    {
                        var processingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();
                        await processingService.ProcessEvent(pendingEvent.Topic, pendingEvent.Event, pendingEvent.EventJson);
                        await eventQueueStore.ClearEventAsync(pendingEvent.LogId);
                    }

                }
            }
        }
    }
}

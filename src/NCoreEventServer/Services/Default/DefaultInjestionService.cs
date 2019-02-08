using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;
using NCoreEventServer.Stores;

namespace NCoreEventServer.Services
{
    /// <summary>
    /// Provides Injestion of new Event Messages
    /// </summary>
    public class DefaultInjestionService : IInjestionService
    {
        private readonly IEventQueueStore eventQueueStore;
        private readonly TriggerService triggerService;

        public DefaultInjestionService(IEventQueueStore eventQueueStore, TriggerService triggerService)
        {
            this.eventQueueStore = eventQueueStore ?? throw new ArgumentNullException(nameof(eventQueueStore));
            this.triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
        }

        public async Task InjestRequest(EventMessage eventMessage)
        {
            await eventQueueStore.AddEventAsync(eventMessage);
            triggerService.InjestionStart.Set();
        }
    }
}

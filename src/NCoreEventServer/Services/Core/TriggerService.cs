using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NCoreEventServer.Services
{
    public class TriggerService
    {
        public AutoResetEvent ProcessingStart { get; set; }

        public AutoResetEvent DeliveryStart { get; set; }

        /// <summary>
        /// Private Cosntructor for Singleton Pattern
        /// </summary>
        private TriggerService()
        {
            ProcessingStart = new AutoResetEvent(true);
            DeliveryStart = new AutoResetEvent(true);
        }

        
        private static readonly Lazy<TriggerService> lazyService =
            new Lazy<TriggerService>(() => new TriggerService());

        /// <summary>
        /// Retrieves the only instance of the TriggerService
        /// </summary>
        public static TriggerService Instance { get { return lazyService.Value; } }
    }
}

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
        public TriggerService()
        {
            ProcessingStart = new AutoResetEvent(true);
            DeliveryStart = new AutoResetEvent(true);
        }
    }
}

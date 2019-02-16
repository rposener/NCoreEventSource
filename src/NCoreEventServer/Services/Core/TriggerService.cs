using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NCoreEventServer.Services
{
    public static class TriggerService
    {
        public static AutoResetEvent ProcessingStart { get; set; }

        public static AutoResetEvent DeliveryStart { get; set; }

        /// <summary>
        /// Private Cosntructor for Singleton Pattern
        /// </summary>
        static TriggerService()
        {
            ProcessingStart = new AutoResetEvent(true);
            DeliveryStart = new AutoResetEvent(true);
        }
    }
}

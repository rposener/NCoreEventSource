using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NCoreEventServer.Services
{
    public class TriggerService
    {
        public AutoResetEvent InjestionStart { get; set; }

        public TriggerService()
        {
            InjestionStart = new AutoResetEvent(true);
        }
    }
}

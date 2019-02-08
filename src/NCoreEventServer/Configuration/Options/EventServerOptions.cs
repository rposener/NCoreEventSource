using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Configuration
{
    /// <summary>
    /// All Configurable Options for NCoreEventServer
    /// </summary>
    public class EventServerOptions
    {
        public EventServerOptions()
        {
            AutoDiscoverEvents = true;
            AutoDiscoverObjectTypes = true;
        }

        /// <summary>
        /// Allow Autodiscovery of Events
        /// </summary>
        public bool AutoDiscoverEvents { get; set; }

        /// <summary>
        /// Allow Autodiscovery of ObjectTypes
        /// </summary>
        public bool AutoDiscoverObjectTypes { get; set; }
    }
}

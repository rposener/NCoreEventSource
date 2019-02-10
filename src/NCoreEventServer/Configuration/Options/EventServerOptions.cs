using Microsoft.AspNetCore.Http;
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
            InjestionBatchSize = 8;
            InjestionPath = "/eventserver/injest";
            RegistationPath = "/eventserver/register";
        }

        /// <summary>
        /// Allow Autodiscovery of Events
        /// </summary>
        public bool AutoDiscoverEvents { get; set; }

        /// <summary>
        /// Allow Autodiscovery of ObjectTypes
        /// </summary>
        public bool AutoDiscoverObjectTypes { get; set; }

        /// <summary>
        /// Number of Items the InjestionService Processes at a time
        /// </summary>
        public int InjestionBatchSize { get; set; }

        /// <summary>
        /// Base EventServer Url
        /// </summary>
        public PathString InjestionPath { get; set; }

        /// <summary>
        /// Base EventServer Url
        /// </summary>
        public PathString RegistationPath { get; set; }
    }
}

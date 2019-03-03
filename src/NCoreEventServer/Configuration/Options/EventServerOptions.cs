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
        const int MIN_SIZE = 1;
        const int MAX_SIZE = 8;

        int eventProcessors;

        public EventServerOptions()
        {
            AutoDiscoverEvents = true;
            AutoDiscoverObjectTypes = true;
            InjestionPath = "/eventserver/injest";
            RegistationPath = "/eventserver/register";
            EventProcessors = Math.Max(MIN_SIZE, Math.Max(MAX_SIZE, Environment.ProcessorCount/2));
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
        /// Number of Tasks Processes at a time
        /// </summary>
        public int EventProcessors
        {
            get { return eventProcessors; }
            set
            {
                if (value < MIN_SIZE || value > MAX_SIZE)
                    throw new ArgumentOutOfRangeException(nameof(EventProcessors), $"Event Processors should be between {MIN_SIZE} and {MAX_SIZE}");
                eventProcessors = value;
            }
        }

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

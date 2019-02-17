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
        const int MAX_SIZE = 265;

        int injestionBatchSize;

        public EventServerOptions()
        {
            AutoDiscoverEvents = true;
            AutoDiscoverObjectTypes = true;
            injestionBatchSize = 8;
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
        public int InjestionBatchSize
        {
            get { return injestionBatchSize; }
            set
            {
                if (value < MIN_SIZE || value > MAX_SIZE)
                    throw new ArgumentOutOfRangeException(nameof(InjestionBatchSize), $"Injestion Batch should be between {MIN_SIZE} and {MAX_SIZE}");
                injestionBatchSize = value;
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

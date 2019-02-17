using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.SqlStore.Models
{

    public class TopicMetadataEntity
    {
        /// <summary>
        /// Name of the Topic that is known
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// List of Registered Events
        /// </summary>
        public string RegisteredEvents { get; set; }
    }
}

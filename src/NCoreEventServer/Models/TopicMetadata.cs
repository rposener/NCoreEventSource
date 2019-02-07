using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Models
{

    public class TopicMetadata
    {
        /// <summary>
        /// Name of the Topic that is known
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// List of Registered Events
        /// </summary>
        public IEnumerable<string> RegisteredEvents { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Models
{
    public class Metadata
    {
        /// <summary>
        /// List of all Objects Known to the System
        /// </summary>
        IEnumerable<ObjectMetadata> Objects { get; set; }

        /// <summary>
        /// List of all Global Events known to the System
        /// </summary>
        IEnumerable<string> GlobalEvents { get; set; }

        /// <summary>
        /// List of all Topics and their Events known to the system
        /// </summary>
        IEnumerable<TopicMetadata> Topics { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventClient
{
    public class NCoreEventAttribute : Attribute
    {
        public string Topic { get; }

        /// <summary>
        /// The Event Topic this Event will Subscribe To
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="event"></param>
        public NCoreEventAttribute(string topic)
        {
            Topic = topic;
        }
    }
}

﻿namespace NCoreEventServer.Models
{
    /// <summary>
    /// Inbound Message when an Event is Raised
    /// </summary>
    public class EventMessage
    {
        /// <summary>
        /// The Id Assigned to this Event
        /// </summary>
        public long LogId { get; set; }

        /// <summary>
        /// Any Topic/Event this is Targeting
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Any Data associated with the Event
        /// </summary>
        public string EventJson { get; set; }

        /// <summary>
        /// Any Type of Object that is Updated by this event
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// The Id of Object that is Updated by this event
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Any Update for the Object
        /// </summary>
        public string ObjectUpdate { get; set; }
    }
}

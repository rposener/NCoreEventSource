using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer
{
    public static class EventMessageExtensions
    {
        /// <summary>
        /// Checks to See if the <seealso cref="EventMessage"/> contains an Event
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        public static bool IsEventMessage(this EventMessage eventMessage)
        {
            return !String.IsNullOrWhiteSpace(eventMessage.Topic);
        }

        /// <summary>
        /// Checks to See if the <seealso cref="EventMessage"/> contains an Object update
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        public static bool IsObjectMessage(this EventMessage eventMessage)
        {
            return (!String.IsNullOrWhiteSpace(eventMessage.ObjectType) &&
                !String.IsNullOrWhiteSpace(eventMessage.ObjectId) &&
                eventMessage.ObjectUpdate != null);
        }
    }
}

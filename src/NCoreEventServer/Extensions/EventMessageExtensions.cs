using NCoreEventServer.Models;
using System;

namespace NCoreEventServer
{
    public static class ServerEventMessageExtensions
    {
        /// <summary>
        /// Checks to See if the <seealso cref="ServerEventMessage"/> contains an Event
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        public static bool IsServerEventMessage(this ServerEventMessage eventMessage)
        {
            return !String.IsNullOrWhiteSpace(eventMessage.Topic);
        }

        /// <summary>
        /// Checks to See if the <seealso cref="ServerEventMessage"/> contains an Object update
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        public static bool IsObjectMessage(this ServerEventMessage eventMessage)
        {
            return (!String.IsNullOrWhiteSpace(eventMessage.ObjectType) &&
                !String.IsNullOrWhiteSpace(eventMessage.ObjectId) &&
                eventMessage.ObjectUpdate != null);
        }
    }
}

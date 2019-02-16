using NCoreEvent;

namespace NCoreEventServer.Models
{
    public class ServerEventMessage : EventMessage
    {
        /// <summary>
        /// The Id in the Log for this <see cref="EventMessage"/>
        /// </summary>
        public long LogId { get; set; }
    }
}

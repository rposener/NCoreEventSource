using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public interface IEventProcessingService
    {
        /// <summary>
        /// Processes an event on a Topic
        /// </summary>
        /// <param name="Topic">The Topic (or null if Global)</param>
        /// <param name="Event"></param>
        /// <param name="EventData"></param>
        /// <returns></returns>
        Task ProcessEvent(string Topic, string Event, string EventData);
    }
}

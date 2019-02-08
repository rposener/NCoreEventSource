using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// IEventLogStore provides an orderly way to read events
    /// </summary>
    public interface IEventQueueStore
    {
        /// <summary>
        /// Adds a <seealso cref="EventMessage"/> to the LogStore
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<long> AddEventAsync(EventMessage message);

        /// <summary>
        /// Reads the next <seealso cref="EventMessage"/>s from the LogStore
        /// </summary>
        /// <param name="Max">Maximum number of events to Read</param>
        /// <returns></returns>
        Task<IEnumerable<EventMessage>> NextEventsAsync(int Max);

        /// <summary>
        /// Removes the <seealso cref="EventMessage"/> from the LogStore Once Processed
        /// </summary>
        /// <returns></returns>
        Task ClearEventAsync(long id);
    }
}

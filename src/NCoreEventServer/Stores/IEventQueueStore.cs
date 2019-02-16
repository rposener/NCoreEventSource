using NCoreEventServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// IEventLogStore provides an orderly way to read events
    /// </summary>
    public interface IEventQueueStore
    {
        /// <summary>
        /// Adds a <seealso cref="ServerEventMessage"/> to the LogStore
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<long> AddEventAsync(ServerEventMessage message);

        /// <summary>
        /// Reads the next <seealso cref="ServerEventMessage"/>s from the LogStore
        /// </summary>
        /// <param name="Max">Maximum number of events to Read</param>
        /// <returns></returns>
        Task<IEnumerable<ServerEventMessage>> NextEventsAsync(int Max);

        /// <summary>
        /// Removes the <seealso cref="ServerEventMessage"/> from the LogStore Once Processed
        /// </summary>
        /// <returns></returns>
        Task ClearEventAsync(long id);

        /// <summary>
        /// Marks the <seealso cref="ServerEventMessage"/> as Poisoned
        /// </summary>
        /// <returns></returns>
        Task PoisonedEventAsync(long id);

        /// <summary>
        /// Reads the Last <seealso cref="ServerEventMessage"/>s that were Poison
        /// </summary>
        /// <param name="Max">Maximum number of events to Read</param>
        /// <returns></returns>
        Task<IEnumerable<ServerEventMessage>> PoisonEventsAsync(int Max);
    }
}

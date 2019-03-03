using NCoreEventServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// IEventQueueStore provides an orderly way to read events
    /// </summary>
    public interface IEventQueueStore
    {
        /// <summary>
        /// Adds a <seealso cref="ServerEventMessage"/> to the QueueStore
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<long> EnqueueEventAsync(ServerEventMessage message);

        /// <summary>
        /// Reads the next <seealso cref="ServerEventMessage"/> from the QueueStore
        /// </summary>
        /// <returns></returns>
        Task<ServerEventMessage> PeekEventAsync();

        /// <summary>
        /// Removes the <seealso cref="ServerEventMessage"/> from the QueueStore Once Processed
        /// </summary>
        /// <returns></returns>
        Task DequeueEventAsync(long id);

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

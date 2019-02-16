using NCoreEventServer.Models;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public interface IMetadataService
    {
        /// <summary>
        /// Auto-discovery of any MetaData
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        Task AutoDiscoverEventsAsync(ServerEventMessage eventMessage);

        /// <summary>
        /// Auto-discovery of any MetaData
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        Task AutoDiscoverObjectsAsync(ServerEventMessage eventMessage);
    }
}

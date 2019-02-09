using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
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
        Task AutoDiscoverEventsAsync(EventMessage eventMessage);

        /// <summary>
        /// Auto-discovery of any MetaData
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        Task AutoDiscoverObjectsAsync(EventMessage eventMessage);
    }
}

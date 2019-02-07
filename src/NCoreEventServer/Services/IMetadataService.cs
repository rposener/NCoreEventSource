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
        Task AutoDiscoverAsync(EventMessage eventMessage);

        /// <summary>
        /// Registers an object Type witht the Metadata Store
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task RegisterObjectTypeAsync(string ObjectType);

        /// <summary>
        /// Registers an Event for a Type of Object with the Metadata Store
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="EventName"></param>
        /// <returns></returns>
        Task RegisterObjectEventAsync(string ObjectType, string EventName);

        /// <summary>
        /// Registers a Topic the Metadata Store
        /// </summary>
        /// <param name="Topic"></param>
        /// <returns></returns>
        Task RegisterTopicAsync(string Topic);

        /// <summary>
        /// Registers an Event for a Topic with the Metadata Store
        /// </summary>
        /// <param name="Topic"></param>
        /// <param name="EventName"></param>
        /// <returns></returns>
        Task RegisterTopicEventAsync(string Topic, string EventName);

        /// <summary>
        /// Registers a Global Event witht the Metadata Store
        /// </summary>
        /// <param name="EventName"></param>
        /// <returns></returns>
        Task RegisterGlobalEventAsync(string EventName);
    }
}

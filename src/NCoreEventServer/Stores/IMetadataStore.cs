using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// Store for all Metadata
    /// </summary>
    public interface IMetadataStore
    {
        /// <summary>
        /// Adds or Updates Topic MetaData
        /// </summary>
        /// <param name="topicMetadata"></param>
        /// <returns></returns>
        Task AddTopicAsync(string Topic);

        /// <summary>
        /// Adds an Event to a Topic
        /// </summary>
        /// <param name="Topic"></param>
        /// <param name="Event"></param>
        /// <returns></returns>
        Task AddEventToTopicAsync(string Topic, string Event);

        /// <summary>
        /// Removes an Event from a Topic
        /// </summary>
        /// <param name="Topic"></param>
        /// <param name="Event"></param>
        /// <returns></returns>
        Task RemoveEventFromTopicAsync(string Topic, string Event);

        /// <summary>
        /// Removes a Topic and all Events
        /// </summary>
        /// <param name="Topic"></param>
        /// <returns></returns>
        Task RemoveTopicAsync(string Topic);

        /// <summary>
        /// Returns Topic MetaData
        /// </summary>
        /// <param name="Topic"></param>
        /// <returns></returns>
        Task<TopicMetadata> GetTopicAsync(string Topic);

        /// <summary>
        /// Adds an ObjectType
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task AddObjectTypeAsync(string ObjectType);

        /// <summary>
        /// Removes an ObjectType
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task RemoveObjectTypeAsync(string ObjectType);

        /// <summary>
        /// Gets a list of all ObjectTypes
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetObjectTypesAsync();
    }
}

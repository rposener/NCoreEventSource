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
        Task<IEnumerable<string>> GetTopicsAsync();

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

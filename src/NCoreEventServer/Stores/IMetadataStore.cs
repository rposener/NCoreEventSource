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
        /// Adds or Updates a Subscriber
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        Task SaveSubscriber(Subscriber subscriber);

        /// <summary>
        /// Removes a Subscriber
        /// </summary>
        /// <param name="BaseUri"></param>
        /// <returns></returns>
        Task RemoveSubscriber(Uri BaseUri);

        /// <summary>
        /// Adds or Updates Topic MetaData
        /// </summary>
        /// <param name="topicMetadata"></param>
        /// <returns></returns>
        Task SaveTopic(TopicMetadata topicMetadata);

        /// <summary>
        /// Removes a Topic
        /// </summary>
        /// <param name="Topic"></param>
        /// <returns></returns>
        Task RemoveTopic(string Topic);

        /// <summary>
        /// Adds an ObjectType
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task AddObjectType(string ObjectType);

        /// <summary>
        /// Removes an ObjectType
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task RemoveObjectType(string ObjectType);
    }
}

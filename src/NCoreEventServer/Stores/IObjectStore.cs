using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// Store to hold current materialized States of Objects
    /// </summary>
    public interface IObjectStore
    {
        /// <summary>
        /// Sets the Property of an Object
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="ObjectId"></param>
        /// <param name="Properties"></param>
        /// <returns></returns>
        Task SetObjectPropertyAsync(string ObjectType, string ObjectId, IDictionary<string,string> Properties);

        /// <summary>
        /// Retrieves a Specific Object
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="ObjectId"></param>
        /// <returns>Json Value of the Object</returns>
        Task<string> GetObjectAsync(string ObjectType, string ObjectId);

        /// <summary>
        /// Retrieves a Specific Object
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns>Collection of Json Values for the Objects</returns>
        Task<IEnumerable<string>> GetObjectsAsync(string ObjectType);

        /// <summary>
        /// Retrieves a List of Properties for an Object
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns>Collection of Json Values for the Objects</returns>
        Task<IEnumerable<string>> GetObjectPropertiesAsync(string ObjectType);

        /// <summary>
        /// Removes all Objects of a Type
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task RemoveAllObjectsOfType(string ObjectType);

        /// <summary>
        /// Removes a Property from all Objects of a Certain Type
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="Property"></param>
        /// <returns></returns>
        Task PruneObjectProperty(string ObjectType, string Property);
    }
}

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
        /// <param name="ObjectJson"></param>
        /// <returns></returns>
        Task SetObjectAsync(string ObjectType, string ObjectId, string ObjectJson);

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
        /// Removes all Objects of a Type
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        Task RemoveAllObjectsOfTypeAsync(string ObjectType);

        /// <summary>
        /// Removes a Specific Object
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="ObjectId"></param>
        /// <returns>Json Value of the Object</returns>
        Task RemoveObjectAsync(string ObjectType, string ObjectId);

    }
}

using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public interface IObjectUpdateService
    {
        /// <summary>
        /// Processes an Update for the Service
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <param name="ObjectId"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        Task UpdateObject(string ObjectType, string ObjectId, string jsonObject);
    }
}

using Microsoft.AspNetCore.Http;
using NCoreEventServer.Models;
using NCoreEventServer.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    /// <summary>
    /// This Service Injests Event Requests into the EventServer
    /// </summary>
    public interface IInjestionService
    {
        /// <summary>
        /// Injests a New Message off the Wire
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<InjestionResult> InjestRequest(HttpContext context);
    }
}

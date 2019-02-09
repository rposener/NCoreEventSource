using Microsoft.AspNetCore.Http;
using NCoreEventServer.Models;
using NCoreEventServer.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
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

using Microsoft.AspNetCore.Http;
using NCoreEventServer.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public interface IRegistrationService
    {
        /// <summary>
        /// Registration Service
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<RegistrationResult> RegistrationRequest(HttpContext context);
    }
}

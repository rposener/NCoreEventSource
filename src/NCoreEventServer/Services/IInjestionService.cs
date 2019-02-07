using NCoreEventServer.Models;
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
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        Task InjestRequest(EventMessage eventMessage);
    }
}

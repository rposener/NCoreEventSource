using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NCoreEventServer.Models;

namespace NCoreEventServer.Services
{
    /// <summary>
    /// Provides Injestion of new Event Messages
    /// </summary>
    public class InjestionService : IInjestionService
    {
        public Task InjestRequest(EventMessage eventMessage)
        {
            throw new NotImplementedException();
        }
    }
}

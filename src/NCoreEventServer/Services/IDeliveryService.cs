using NCoreEventServer.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public interface IDeliveryService
    {
        /// <summary>
        /// Delivers a Message
        /// </summary>
        /// <param name="Uri"></param>
        /// <param name="Body"></param>
        /// <returns></returns>
        Task<DeliveryResult> DeliverMessage(Uri Uri, string Body);
    }
}

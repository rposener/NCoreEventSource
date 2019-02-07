using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// Store of all Registered Subscribers
    /// </summary>
    public interface ISubscriberStore
    {
        /// <summary>
        /// Registers a Subscriber and all of it's Subscriptions
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        Task RegisterSubscriberAsync(Subscriber subscriber);
    }
}

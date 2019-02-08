using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// Store of all Subscribers
    /// </summary>
    public interface ISubscriberStore
    {
        /// <summary>
        /// Adds a Subscriber and all of it's Subscriptions
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        Task AddSubscriberAsync(Subscriber subscriber);

        /// <summary>
        /// Updates a Subscriber and all of it's Subscriptions
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        Task UpdateSubscriberAsync(Subscriber subscriber);

        /// <summary>
        /// Returns a Subscriber from the Store
        /// </summary>
        /// <param name="SubscriberId"></param>
        /// <returns></returns>
        Task<Subscriber> GetSubscriber(string SubscriberId);

        /// <summary>
        /// Deletes a current Subscriber
        /// </summary>
        /// <param name="SubscriberId"></param>
        /// <returns></returns>
        Task DeleteSubscriberAsync(string SubscriberId);
    }
}

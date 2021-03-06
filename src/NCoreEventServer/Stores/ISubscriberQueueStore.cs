﻿using NCoreEventServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Stores
{
    /// <summary>
    /// This Store holds Messages as they are ready to be sent to Subscribers
    /// </summary>
    public interface ISubscriberQueueStore
    {

        /// <summary>
        /// Adds a Message to be sent to a Subscriber
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task EnqueueMessageAsync(SubscriberMessage message);

        /// <summary>
        /// Gets the next message destined for a certain subscriber
        /// </summary>
        /// <param name="SubscriberId"></param>
        /// <returns></returns>
        Task<SubscriberMessage> PeekMessageAsync(string SubscriberId);

        /// <summary>
        /// Clears a specific Message from the Queue
        /// </summary>
        /// <param name="MessageId"></param>
        /// <returns></returns>
        Task ClearMessageAsync(long MessageId);

        /// <summary>
        /// Returns List of SubscriberIds that have pending messages
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> SubscriberIdsWithPendingMessages();
    }
}

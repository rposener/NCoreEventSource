using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NCoreEventServer.Models;
using NCoreEventServer.Services.Results;
using NCoreEventServer.Stores;

namespace NCoreEventServer.Services
{
    /// <summary>
    /// Provides Injestion of new Event Messages
    /// </summary>
    public class DefaultInjestionService : IInjestionService
    {
        private readonly IEventQueueStore eventQueueStore;
        private readonly ILogger<DefaultInjestionService> logger;

        public DefaultInjestionService(
            IEventQueueStore eventQueueStore, 
            ILogger<DefaultInjestionService> logger)
        {
            this.eventQueueStore = eventQueueStore ?? throw new ArgumentNullException(nameof(eventQueueStore));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Injests a Request into a <seealso cref="ServerEventMessage"/> and sends to the <seealso cref="IEventQueueStore"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Result from Injesting the Request</returns>
        public async Task<InjestionResult> InjestRequest(HttpContext context)
        {

            // Some Quick Validations
            if (!context.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Invalid Method");
                return InjestionResult.Failure(HttpStatusCode.MethodNotAllowed, "Invalid Method");
            }
            if (!context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Invalid Content-Type");
                return InjestionResult.Failure(HttpStatusCode.UnsupportedMediaType, "Invalid Content-Type");
            }

            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, false))
            {
                var json = await reader.ReadToEndAsync();
                if (String.IsNullOrWhiteSpace(json))
                {
                    logger.LogWarning("Request had an Empty Body");
                    return InjestionResult.Failure(HttpStatusCode.BadRequest, "Invalid Body");
                }
                var eventMessage = EventStoreSerialization.DeSerializeObject<ServerEventMessage>(json);
                if (eventMessage == null)
                {
                    logger.LogWarning("Request did not match expected format for Events.");
                    return InjestionResult.Failure(HttpStatusCode.BadRequest, "Invalid Body Format");
                }

                // Store the Message and Trigger background processing
                await eventQueueStore.AddEventAsync(eventMessage);
                TriggerService.ProcessingStart.Set();

                // Return a Success to the Middleware
                return InjestionResult.Success;
            }
        }
    }
}

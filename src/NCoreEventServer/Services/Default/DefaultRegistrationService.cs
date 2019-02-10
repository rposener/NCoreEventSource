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
    public class DefaultRegistrationService : IRegistrationService
    {
        private readonly ILogger<DefaultRegistrationService> logger;
        private readonly ISubscriberStore subscriberStore;

        public DefaultRegistrationService(
            ILogger<DefaultRegistrationService> logger,
            ISubscriberStore subscriberStore)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.subscriberStore = subscriberStore ?? throw new ArgumentNullException(nameof(subscriberStore));
        }

        public async Task<RegistrationResult> RegistrationRequest(HttpContext context)
        {
            // Some Quick Validations
            if (!context.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Invalid Method");
                return RegistrationResult.Failure(HttpStatusCode.MethodNotAllowed, "Invalid Method");
            }
            if (!context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Invalid Content-Type");
                return RegistrationResult.Failure(HttpStatusCode.UnsupportedMediaType, "Invalid Content-Type");
            }

            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, false))
            {
                var json = await reader.ReadToEndAsync();
                if (String.IsNullOrWhiteSpace(json))
                {
                    logger.LogWarning("Request had an Empty Body");
                    return RegistrationResult.Failure(HttpStatusCode.BadRequest, "Invalid Body");
                }
                var subscriber = EventStoreSerialization.DeSerializeObject<Subscriber>(json);
                if (subscriber == null)
                {
                    logger.LogWarning("Request did not match expected format for Subscribers.");
                    return RegistrationResult.Failure(HttpStatusCode.BadRequest, "Invalid Body Format");
                }

                // Store the Message and Trigger background processing
                await subscriberStore.AddSubscriberAsync(subscriber);
                logger.LogInformation($"Registered the Subscriber '{subscriber.SubscriberId}'");

                // Return a Success to the Middleware
                return RegistrationResult.Success;
            }
        }
    }
}

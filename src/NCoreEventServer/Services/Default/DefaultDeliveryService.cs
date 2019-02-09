using Microsoft.Extensions.Logging;
using NCoreEventServer.Services.Results;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Services
{
    public class DefaultDeliveryService : IDeliveryService
    {
        private readonly ILogger<DefaultDeliveryService> logger;
        private readonly HttpClient httpClient;

        public DefaultDeliveryService(
            ILogger<DefaultDeliveryService> logger,
            HttpClient httpClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Delivers a Message to an external System
        /// </summary>
        /// <param name="Uri"></param>
        /// <param name="Body"></param>
        /// <returns></returns>
        public async Task<DeliveryResult> DeliverMessage(Uri Uri, string Body)
        {
            logger.LogDebug($"Delivering Message to {Uri}");
            var content = new StringContent(Body, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Uri, content);

            if (response.IsSuccessStatusCode)
                return DeliveryResult.Success(response.StatusCode, "Message Delivered");

            // Failed
            return DeliveryResult.Failure(response.StatusCode, response.ReasonPhrase);
        }
    }
}

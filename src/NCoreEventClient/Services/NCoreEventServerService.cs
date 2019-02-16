using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCoreEventClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventClient
{
    public class NCoreEventServerService
    {
        private readonly IOptions<NCoreEventOptions> options;
        private readonly ILogger<NCoreEventServerService> logger;
        private readonly HttpClient httpClient;

        public NCoreEventServerService(
            IOptions<NCoreEventOptions> options,
            ILogger<NCoreEventServerService> logger,
            HttpClient httpClient)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Sends Registration to the Server
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        internal async Task SendRegistration(Subscriber subscriber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(subscriber), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(options.Value.RegistrationUrl, content);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Registration successfully Sent");
                return;
            }
            logger.LogError($"Error Registering with EventServer: {response.StatusCode} Status");
        }

        /// <summary>
        /// Sends Registration to the Server
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public async Task SendMessage<T>(string Topic, T EventData)
        {
            var message = new EventMessage { Topic = Topic, EventJson = JsonConvert.SerializeObject(EventData) };
            ValidateEvent(message);
            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(options.Value.InjestionUrl, content);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("Message successfully Sent");
        }


        /// <summary>
        /// Sends Registration to the Server
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public async Task SendMessage(string ObjectType, string ObjectId, string jsonObject)
        {
            var message = new EventMessage { ObjectType = ObjectType, ObjectId = ObjectId, ObjectUpdate = jsonObject };
            ValidateEvent(message);
            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(options.Value.InjestionUrl, content);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("Message successfully Sent");
        }

        /// <summary>
        /// Sends Registration to the Server
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public async Task SendMessage<T>(string Topic, T EventData, string ObjectType, string ObjectId, string jsonObject)
        {
            var message = new EventMessage { Topic = Topic, EventJson = JsonConvert.SerializeObject(EventData), ObjectType = ObjectType, ObjectId = ObjectId, ObjectUpdate = jsonObject };
            ValidateEvent(message);
            var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(options.Value.InjestionUrl, content);
            response.EnsureSuccessStatusCode();
            logger.LogInformation("Message successfully Sent");
        }

        private void ValidateEvent(EventMessage message)
        {
            var validationIssues = message.Validate(new ValidationContext(message));
            if (validationIssues.Any())
            {
                var result = validationIssues.First();
                throw new ValidationException(result.ErrorMessage);
            }
        }
    }
}

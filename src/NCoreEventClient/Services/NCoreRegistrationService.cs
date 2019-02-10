using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using NCoreEventClient.Models;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;

namespace NCoreEventClient.Services
{
    /// <summary>
    /// Service to Collect all Routes and Update the EventServer
    /// </summary>
    public class NCoreRegistrationService : BackgroundService
    {
        private readonly ILogger<NCoreRegistrationService> logger;
        private readonly IOptions<NCoreEventOptions> options;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private readonly NCoreEventServerService eventServerService;

        public NCoreRegistrationService(
            ILogger<NCoreRegistrationService> logger,
            IOptions<NCoreEventOptions> options,
            IHostingEnvironment hostingEnvironment,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            NCoreEventServerService eventServerService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider ?? throw new ArgumentNullException(nameof(actionDescriptorCollectionProvider));
            this.eventServerService = eventServerService ?? throw new ArgumentNullException(nameof(eventServerService));
        }

        public override void Dispose()
        {
            logger.LogInformation("Registration Service is disposed.");
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var settings = new JsonSerializerSettings();

            var subscriber = new Subscriber
            {
                BaseUri = new Uri(options.Value.SubscriberBaseUrl),
                SubscriberId = options.Value.SubscriberId,
                Subscriptions = new List<Subscription>()
            };


            foreach (ActionDescriptor route in actionDescriptorCollectionProvider.ActionDescriptors.Items)
            {
                var methodConstraint = route.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault();
                if (methodConstraint != null && methodConstraint.HttpMethods.Contains("POST", StringComparer.OrdinalIgnoreCase))
                {
                    var controllerDescriptor = route as ControllerActionDescriptor;
                    if (controllerDescriptor != null)
                    {
                        var eventAttribute = controllerDescriptor.MethodInfo.GetCustomAttributes(typeof(NCoreEventAttribute), false).OfType<NCoreEventAttribute>().FirstOrDefault();
                        if (eventAttribute != null)
                        {
                            logger.LogInformation($"Route Discovery {route.AttributeRouteInfo.Template} is subscribing to {eventAttribute.Topic}.");
                            subscriber.Subscriptions.Add(new Subscription
                            {
                                RelativePath = route.AttributeRouteInfo.Template,
                                Topic = eventAttribute.Topic,
                                Type = SubscriptionTypes.Event
                            });
                        }

                        var objectAttribute = controllerDescriptor.MethodInfo.GetCustomAttributes(typeof(NCoreObjectAttribute), false).OfType<NCoreObjectAttribute>().FirstOrDefault();
                        if (objectAttribute != null)
                        {
                            logger.LogInformation($"Route Discovery {route.AttributeRouteInfo.Template} is subscribing to {objectAttribute.ObjectType}.");
                            subscriber.Subscriptions.Add(new Subscription
                            {
                                RelativePath = route.AttributeRouteInfo.Template,
                                Topic = objectAttribute.ObjectType,
                                Type = SubscriptionTypes.Object
                            });
                        }
                    }
                }
            }

            logger.LogInformation("Subscription Information:\r\n{0}", JsonConvert.SerializeObject(subscriber, Formatting.Indented, settings));
            await eventServerService.SendRegistration(subscriber);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCoreEventServer.Configuration;
using NCoreEventServer.Services;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NCoreEventServer.Middleware
{
    /// <summary>
    /// Core Middleware for EventServer
    /// </summary>
    public class EventServerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<EventServerMiddleware> logger;
        private readonly IOptions<EventServerOptions> options;

        public EventServerMiddleware(
            RequestDelegate next, 
            ILogger<EventServerMiddleware> logger,
            IOptions<EventServerOptions> options)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (options.Value.InjestionPath.Equals(context.Request.Path))
                {
                    logger.LogDebug("EventServer will Process the Request");
                    // Process the Request
                    var injestionService = context.RequestServices.GetRequiredService<IInjestionService>();
                     var result = await injestionService.InjestRequest(context);

                    // Send Response
                    logger.LogDebug($"Processing has resulted in {result.StatusCode} status.");
                    context.Response.StatusCode = result.StatusCode;
                    if (!String.IsNullOrWhiteSpace(result.Message))
                    await context.Response.WriteAsync(result.Message, Encoding.UTF8);
                    return;
                }

                if (options.Value.RegistationPath.Equals(context.Request.Path))
                {
                    logger.LogDebug("EventServer will Process the Request");
                    // Process the Request
                    var registrationService = context.RequestServices.GetRequiredService<IRegistrationService>();
                    var result = await registrationService.RegistrationRequest(context);

                    // Send Response
                    logger.LogDebug($"Processing has resulted in {result.StatusCode} status.");
                    context.Response.StatusCode = result.StatusCode;
                    if (!String.IsNullOrWhiteSpace(result.Message))
                        await context.Response.WriteAsync(result.Message, Encoding.UTF8);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, ex.Message);
            }
            await next(context);
        }
    }
}

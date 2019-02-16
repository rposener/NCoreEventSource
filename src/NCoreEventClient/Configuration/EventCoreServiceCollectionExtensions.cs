using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCoreEventClient;
using NCoreEventClient.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventCoreServiceCollectionExtensions
    {

        /// <summary>
        /// Adds Event Server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureAction">Configuration Action</param>
        /// <returns></returns>
        public static IServiceCollection AddEventCore(this IServiceCollection services, Action<NCoreEventOptions> configureAction)
        {
            services.Configure(configureAction);
            return RegisterServices(services);
        }

        /// <summary>
        /// Adds Event Server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddEventCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NCoreEventOptions>(configuration);
            return RegisterServices(services);
        }

        static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddHostedService<NCoreRegistrationService>();
            services.AddHttpClient<NCoreEventService>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<NCoreEventOptions>>();
                    client.BaseAddress = new Uri(options.Value.EventServerUrl);
                })
                .AddPolicyHandler(GetRetryPolicy(4));
            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            Random jitterer = new Random();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
        }
    }
}

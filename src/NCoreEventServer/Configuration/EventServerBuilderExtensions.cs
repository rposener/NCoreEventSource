using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NCoreEventServer.Services;
using NCoreEventServer.Stores;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServerBuilderExtensions
    {

        public static IEventServerBuilder AddCoreServices(this IEventServerBuilder builder)
        {
            builder.Services.AddSingleton<TriggerService>();
            builder.Services.AddHostedService<HostedProcessingService>();
            builder.Services.AddHostedService<HostedDeliveryService>();
            return builder;
        }

        public static IEventServerBuilder AddDefaultServices(this IEventServerBuilder builder)
        {
            builder.Services.TryAddTransient<IRegistrationService, DefaultRegistrationService>();
            builder.Services.TryAddTransient<IInjestionService, DefaultInjestionService>();
            builder.Services.TryAddTransient<IMetadataService, DefaultMetadataService>();
            builder.Services.TryAddTransient<IEventProcessingService, DefaultEventProcessingService>();
            builder.Services.TryAddTransient<IObjectUpdateService, DefaultObjectUpdateService>();
            return builder;
        }

        public static IEventServerBuilder AddInMemoryStores(this IEventServerBuilder builder)
        {
            builder.Services.AddSingleton<IEventQueueStore, InMemoryEventQueueStore>();
            builder.Services.AddSingleton<IMetadataStore, InMemoryMetadataStore>();
            builder.Services.AddSingleton<IObjectStore, InMemoryObjectStore>();
            builder.Services.AddSingleton<ISubscriberQueueStore, InMemorySubscriberQueueStore>();
            builder.Services.AddSingleton<ISubscriberStore, InMemorySubscriberStore>();
            return builder;
        }


        public static IEventServerBuilder AddHttpPostDelivery(this IEventServerBuilder builder)
        {
            builder.Services.AddHttpClient<IDeliveryService, DefaultDeliveryService>();
            return builder;
        }

        public static IEventServerBuilder AddHttpPostDeliveryWithRetry(this IEventServerBuilder builder, int retryCount = 6)
        {
            builder.Services.AddHttpClient<IDeliveryService, DefaultDeliveryService>()
                .AddPolicyHandler(GetRetryPolicy(retryCount));
            return builder;
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NCoreEventServer.Services;
using NCoreEventServer.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServerBuilderExtensions
    {

        public static IEventServerBuilder AddCoreServices(this IEventServerBuilder builder)
        {
            builder.Services.AddSingleton<TriggerService>();
            builder.Services.AddHostedService<HostedProcessingService>();
            return builder;
        }

        public static IEventServerBuilder AddDefaultServices(this IEventServerBuilder builder)
        {
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
    }
}

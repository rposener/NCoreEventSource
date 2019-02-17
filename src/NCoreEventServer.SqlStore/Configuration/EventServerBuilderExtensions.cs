using Microsoft.EntityFrameworkCore;
using NCoreEventServer.SqlStore;
using NCoreEventServer.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServerBuilderExtensions
    {

        
        public static IEventServerBuilder AddSqlServerStores(this IEventServerBuilder builder, Action<DbContextOptionsBuilder> configureDb)
        {
            builder.Services.AddDbContextPool<EventServerContext>(configureDb);
            builder.Services.AddScoped<IEventQueueStore, InMemoryEventQueueStore>();
            builder.Services.AddScoped<IMetadataStore, InMemoryMetadataStore>();
            builder.Services.AddScoped<IObjectStore, InMemoryObjectStore>();
            builder.Services.AddScoped<ISubscriberQueueStore, InMemorySubscriberQueueStore>();
            builder.Services.AddScoped<ISubscriberStore, InMemorySubscriberStore>();
            return builder;
        }

      
    }
}

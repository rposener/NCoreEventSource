using Microsoft.EntityFrameworkCore;
using NCoreEventServer.SqlStore;
using NCoreEventServer.SqlStore.Stores;
using NCoreEventServer.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServerBuilderExtensions
    {
                
        public static IEventServerBuilder AddSqlServerStores(this IEventServerBuilder builder, Action<DbContextOptionsBuilder> configureDb)
        {
            builder.Services.AddDbContextPool<EventServerContext>(configureDb);
            builder.Services.AddScoped<IEventQueueStore, SqlEventQueueStore>();
            builder.Services.AddScoped<IMetadataStore, SqlMetadataStore>();
            builder.Services.AddScoped<IObjectStore, SqlObjectStore>();
            builder.Services.AddScoped<ISubscriberQueueStore, SqlSubscriberQueueStore>();
            builder.Services.AddScoped<ISubscriberStore, SqlSubscriberStore>();
            return builder;
        }

    }
}

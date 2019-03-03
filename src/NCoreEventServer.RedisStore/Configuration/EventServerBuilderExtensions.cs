using AutoMapper;
using Microsoft.Extensions.Options;
using NCoreEventServer.RedisStore;
using NCoreEventServer.RedisStore.Stores;
using NCoreEventServer.Stores;
using ServiceStack.Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServerBuilderExtensions
    {
                
        public static IEventServerBuilder AddRedisStores(this IEventServerBuilder builder, Action<RedisStoreOptions> configure)
        {
            builder.Services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });
            builder.Services.AddOptions();
            builder.Services.Configure<RedisStoreOptions>(configure);
            builder.Services.AddSingleton<IRedisClientsManager>(provider =>
            {
                var opts = provider.GetRequiredService<IOptions<RedisStoreOptions>>();
                return new RedisManagerPool(opts.Value.ConnectionString);
            });
            builder.Services.AddScoped<IEventQueueStore, RedisEventQueueStore>();
            builder.Services.AddScoped<IMetadataStore, RedisMetadataStore>();
            builder.Services.AddScoped<IObjectStore, RedisObjectStore>();
            //builder.Services.AddScoped<ISubscriberQueueStore, SqlSubscriberQueueStore>();
            //builder.Services.AddScoped<ISubscriberStore, SqlSubscriberStore>();
            return builder;
        }

    }
}

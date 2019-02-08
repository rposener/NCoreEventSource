using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NCoreEventServer.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventServerServiceCollectionExtensions
    {

        /// <summary>
        /// Adds Event Server
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEventServerBuilder AddEventServer(this IServiceCollection services)
        {
            var builder = new EventServerBuilder(services);
            builder.AddCoreServices();
            builder.AddDefaultServices();
            return builder;
        }

        /// <summary>
        /// Adds Event Server
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEventServerBuilder AddEventServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EventServerOptions>(configuration);
            var builder = new EventServerBuilder(services);
            builder.AddCoreServices();
            builder.AddDefaultServices();
            return builder;
        }
    }
}

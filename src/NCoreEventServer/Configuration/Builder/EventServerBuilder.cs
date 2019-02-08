using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreEventServer.Configuration
{
    public class EventServerBuilder : IEventServerBuilder
    {
        public IServiceCollection Services { get; }

        public EventServerBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}

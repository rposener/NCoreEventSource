using Microsoft.AspNetCore.Builder;
using NCoreEventServer.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class EventServerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEventServer(this IApplicationBuilder app)
        {
            app.UseMiddleware<EventServerMiddleware>();
            return app;
        }
    }
}

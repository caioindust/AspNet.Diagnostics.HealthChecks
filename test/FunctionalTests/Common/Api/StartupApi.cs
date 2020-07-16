using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tests.Api
{
    public class StartupApi : Startup
    {
        public override void AddServices(IServiceCollection services)
        {
        }

        public override void Setup(IAppBuilder app, IServiceProvider serviceProvider)
        {
            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments(new PathString("/")), conf => conf.Use<Middleware>(HttpStatusCode.OK));
            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments(new PathString("/failure")), conf => conf.Use<Middleware>(HttpStatusCode.ServiceUnavailable));
        }

        public class Middleware : OwinMiddleware
        {
            private readonly HttpStatusCode _httpStatusCode;

            public Middleware(OwinMiddleware next, HttpStatusCode httpStatusCode)
                : base(next)
            {
                _httpStatusCode = httpStatusCode;
            }

            public override Task Invoke(IOwinContext context)
            {
                return InvokeInternal(context);
            }

            private async Task InvokeInternal(IOwinContext context)
            {
                context.Response.ContentType = "text/plain";

                context.Response.StatusCode = (int)_httpStatusCode;
                await context.Response.WriteAsync(_httpStatusCode.ToString());
            }
        }
    }
}

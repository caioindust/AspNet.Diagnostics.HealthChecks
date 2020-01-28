using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace AspNet.HealthChecks.UI.Client
{
    /*
     * https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.UI.Client/UIResponseWriter.cs
     */

    public static class UIResponseWriter
    {
        private const string DEFAULT_CONTENT_TYPE = "application/json";

        public static Task WriteHealthCheckUIResponse(IOwinContext context, HealthReport result) => WriteHealthCheckUIResponse(context, result, null);

        public static Task WriteHealthCheckUIResponse(IOwinContext context, HealthReport result, Action<JsonSerializerSettings> jsonConfigurator)
        {
            var response = "{}";

            if (result != null)
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                jsonConfigurator?.Invoke(settings);

                settings.Converters.Add(new StringEnumConverter());

                context.Response.ContentType = DEFAULT_CONTENT_TYPE;

                var uiReport = UIHealthReport
                    .CreateFrom(result);

                response = JsonConvert.SerializeObject(uiReport, settings);
            }

            return context.Response.WriteAsync(response);
        }
    }
}

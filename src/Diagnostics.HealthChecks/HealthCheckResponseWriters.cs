// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;

namespace Microsoft.AspNet.Diagnostics.HealthChecks
{
    internal static class HealthCheckResponseWriters
    {
        public static Task WriteMinimalPlaintext(IOwinContext context, HealthReport result)
        {
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync(result.Status.ToString()); 
        }
    }
}
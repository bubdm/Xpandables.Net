using System;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Xpandables.Net.Api.Configurations
{
    public sealed class ApiRoutingOptions : IConfigureOptions<RouteOptions>
    {
        public void Configure(RouteOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            options.ConstraintMap.Add("string", typeof(StringConstraintMap));
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        }
    }
}

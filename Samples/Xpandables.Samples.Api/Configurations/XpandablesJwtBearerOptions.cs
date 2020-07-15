using System;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Xpandables.Samples.Api.Configurations
{
    public sealed class XpandablesJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        public void PostConfigure(string name, JwtBearerOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "connected",
                ValidAudience = "connected",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your secret key for authentication"))
            };
        }
    }
}

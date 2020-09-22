
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Xpandables.Net.Api.Settings;

namespace Xpandables.Net.Api.Configurations
{
    public sealed class ApiJwtBearerOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        private readonly JwtSettings _jwtSettings;

        public ApiJwtBearerOptions(IOptions<JwtSettings> jwtSettingsOptions) => _jwtSettings = jwtSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(jwtSettingsOptions));

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
                ValidIssuer = _jwtSettings.JWT_Issuer,
                ValidAudience = _jwtSettings.JWT_Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.JWT_Secret))
            };
        }
    }
}

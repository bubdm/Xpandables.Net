
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

namespace Xpandables.Net.Api.Settings
{
    public sealed class JwtSettings
    {
        public JwtSettings() { }

        public JwtSettings(string jWT_Secret, string jWT_Issuer, int jWT_ExpirationMinutes, string jWt_Type)
        {
            JWT_Secret = jWT_Secret ?? throw new ArgumentNullException(nameof(jWT_Secret));
            JWT_Issuer = jWT_Issuer ?? throw new ArgumentNullException(nameof(jWT_Issuer));
            JWT_ExpirationMinutes = jWT_ExpirationMinutes;
            JWT_Type = jWt_Type ?? throw new ArgumentNullException(nameof(jWt_Type));
        }

        public string JWT_Secret { get; set; } = null!;
        public string JWT_Issuer { get; set; } = null!;
        public int JWT_ExpirationMinutes { get; set; }
        public string JWT_Type { get; set; } = "Bearer";
    }
}

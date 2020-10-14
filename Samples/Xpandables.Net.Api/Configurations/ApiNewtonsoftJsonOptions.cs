
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using System;

namespace Xpandables.Net.Api.Configurations
{
    public sealed class ApiNewtonsoftJsonOptions : IConfigureOptions<MvcNewtonsoftJsonOptions>
    {
        public void Configure(MvcNewtonsoftJsonOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            options.SerializerSettings.Formatting = Formatting.Indented;
            options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
        }
    }
}

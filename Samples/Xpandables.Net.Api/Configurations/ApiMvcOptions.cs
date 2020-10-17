﻿
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Xpandables.Net.Api.Middlewares;

namespace Xpandables.Net.Api.Configurations
{
    public sealed class ApiMvcOptions : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            options.EnableEndpointRouting = false;
            options.RespectBrowserAcceptHeader = true;
            options.ReturnHttpNotAcceptable = true;
            options.Filters.Add<ApiExceptionHandlerFilterAttribute>(int.MinValue);
        }
    }
}
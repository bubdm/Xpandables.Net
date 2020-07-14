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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System;

namespace Xpandables.Net5.AspNetCore.Localization
{
    /// <summary>
    /// Provides with <see cref="MvcOptions"/> localization configuration.
    /// </summary>
    public sealed class XMvcOptions : IConfigureOptions<MvcOptions>
    {
        private readonly ILocalizationResourceEngine _localizationResourceEngine;

        /// <summary>
        /// Initializes a new instance of <see cref="XMvcOptions"/> with the localization engine.
        /// </summary>
        /// <param name="localizationResourceEngine">The localization engine.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localizationResourceEngine"/> is null.</exception>
        public XMvcOptions(ILocalizationResourceEngine localizationResourceEngine)
            => _localizationResourceEngine = localizationResourceEngine ?? throw new ArgumentNullException(nameof(localizationResourceEngine));

        /// <summary>
        /// Invoked to configure the <see cref="MvcOptions"/> instance.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/> instance to configure.</param>
        public void Configure(MvcOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            options.ModelBinderProviders.Insert(0, _localizationResourceEngine);
            options.ModelValidatorProviders.Insert(0, _localizationResourceEngine);
        }
    }
}

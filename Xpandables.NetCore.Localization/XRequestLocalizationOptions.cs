
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;

using Xpandables.Net.Localization;

namespace Xpandables.NetCore.Localization
{
    /// <summary>
    /// Provides with <see cref="RequestLocalizationOptions"/> localization configuration.
    /// </summary>
    public sealed class XRequestLocalizationOptions : IConfigureOptions<RequestLocalizationOptions>
    {
        private readonly ILocalizationResourceProvider _localizationResourceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="XRequestLocalizationOptions"/> with the localization provider.
        /// </summary>
        /// <param name="localizationResourceProvider">The localization provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="localizationResourceProvider"/> is null.</exception>
        public XRequestLocalizationOptions(ILocalizationResourceProvider localizationResourceProvider)
            => _localizationResourceProvider = localizationResourceProvider ?? throw new ArgumentNullException(nameof(localizationResourceProvider));

        /// <summary>
        /// Invoked to configure the <see cref="RequestLocalizationOptions"/> instance.
        /// </summary>
        /// <param name="options">The <see cref="RequestLocalizationOptions"/> instance to configure.</param>
        public void Configure(RequestLocalizationOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));

            var supportedCultures = _localizationResourceProvider.AvailableViewModelCultures();

            if (supportedCultures.Any())
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First().Name, supportedCultures.First().Name);
                options.SupportedCultures = supportedCultures.ToList();
                options.SupportedUICultures = supportedCultures.ToList();
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                    {
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider(),
                        new AcceptLanguageHeaderRequestCultureProvider()
                    };
            }
        }
    }
}

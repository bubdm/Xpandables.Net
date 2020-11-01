
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xpandables.Net.Localization;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides methods to register localization services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the localization service provider.
        /// The service requires an implementation of <see cref="ILocalizationResourceProvider"/> to be registered.
        /// </summary>
        /// <param name="services">The collection of service to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXLocalizationResources<TLocalizationResourceProvider>(this IServiceCollection services)
            where TLocalizationResourceProvider : class, ILocalizationResourceProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, XRequestLocalizationOptions>();
            services.AddSingleton<IConfigureOptions<MvcOptions>, XMvcLocalizationOptions>();

            services.AddSingleton<ILocalizationResourceProvider, TLocalizationResourceProvider>();
            services.AddSingleton<ILocalizationResourceEngine, LocalizationResourceEngine>();

            return services;
        }

        /// <summary>
        /// Adds the Microsoft.AspNetCore.Localization.RequestLocalizationMiddleware to automatically set culture information for
        /// requests based on information provided by the client.
        /// </summary>
        /// <param name="app">The current application builder to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="app"/> is null.</exception>
        public static IApplicationBuilder UseXLocalizationDecorator(this IApplicationBuilder app)
        {
            _ = app ?? throw new ArgumentNullException(nameof(app));

            var locazationOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locazationOptions.Value);

            return app;
        }
    }
}

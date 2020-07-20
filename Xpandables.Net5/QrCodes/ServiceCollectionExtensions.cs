
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
using Microsoft.Extensions.DependencyInjection;

using System;

using Xpandables.Net5.QrCodes;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net5.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register qr code services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IQrCodeEngine"/> to the collection of services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQrCode(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IQrCodeEngine, QrCodeEngine>();
            services.AddScoped<IQrCodeImageGenerator, QrCodeImageGenerator>();
            services.AddScoped<IQrCodeTextGenerator, QrCodeTextGenerator>();
            services.AddScoped<IQrCodeValidator, QrCodeValidator>();

            return services;
        }

        /// <summary>
        /// Adds and configures the <see cref="IQrCodeEngine"/> to the collection of services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="QrCodeOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXQrCode(this IServiceCollection services, Action<QrCodeOptions> configureOptions)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.AddScoped<IQrCodeEngine, QrCodeEngine>();

            var definedOptions = new QrCodeOptions();
            configureOptions.Invoke(definedOptions);

            if (definedOptions.IsCustomImageGenerator is { })
                services.AddScoped(typeof(IQrCodeImageGenerator), definedOptions.IsCustomImageGenerator);
            else
                services.AddScoped<IQrCodeImageGenerator, QrCodeImageGenerator>();

            if (definedOptions.IsCustomTextGenerator is { })
                services.AddScoped(typeof(IQrCodeTextGenerator), definedOptions.IsCustomTextGenerator);
            else
                services.AddScoped<IQrCodeTextGenerator, QrCodeTextGenerator>();

            if (definedOptions.IsCustomValidator is { })
                services.AddScoped(typeof(IQrCodeValidator), definedOptions.IsCustomValidator);
            else
                services.AddScoped<IQrCodeValidator, QrCodeValidator>();

            return services;
        }
    }
}

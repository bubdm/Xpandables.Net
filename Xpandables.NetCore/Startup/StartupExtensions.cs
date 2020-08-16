
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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Xpandables.NetCore.Startup
{
    /// <summary>
    /// Provides with startup extension method.
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Registers a configuration instance which TOptions will bind against and adds a transient service of the type specified in <typeparamref name="TOptions"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection XConfigureOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.Configure<TOptions>(configuration.GetSection(typeof(TOptions).Name));
            services.AddTransient(_ => configuration.GetSection(typeof(TOptions).Name).Get<TOptions>());

            return services;
        }
    }
}

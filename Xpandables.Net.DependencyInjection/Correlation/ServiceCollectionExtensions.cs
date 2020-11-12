﻿/************************************************************************************************************
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

using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Correlation;
using Xpandables.Net.CQRS;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="CorrelationCollection{TKey, TValue}"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationCollection(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped(typeof(CorrelationCollection<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IAsyncCorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationContext(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<AsyncCorrelationContext>();
            services.AddScoped<IAsyncCorrelationContext>(provider => provider.GetRequiredService<AsyncCorrelationContext>());
            return services;
        }

        /// <summary>
        /// Adds correlation behavior to commands and queries that are decorated with the <see cref="ICorrelationDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<AsyncCorrelationContext>();
            services.AddScoped<IAsyncCorrelationContext>(provider => provider.GetRequiredService<AsyncCorrelationContext>());

            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandCorrelationDecorator<>));
            services.XTryDecorate(typeof(IAsyncCommandHandler<,>), typeof(AsyncCommandCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IAsyncEnumerableQueryHandler<,>), typeof(AsyncQueryCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(QueryCorrelationDecorator<,>));

            return services;
        }
    }
}

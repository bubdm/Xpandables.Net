
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

using Xpandables.Net.Commands;
using Xpandables.Net.Correlations;
using Xpandables.Net.Decorators;
using Xpandables.Net.Decorators.Correlations;
using Xpandables.Net.Queries;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="CorrelationCollection{TKey, TValue}"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCorrelationCollection(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped(typeof(CorrelationCollection<,>));
            return services;
        }

        /// <summary>
        /// Adds the implementation type of <see cref="ICorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <typeparam name="TCorrelationContext">the correlation context type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCorrelationContext<TCorrelationContext>(this IXpandableServiceBuilder services)
            where TCorrelationContext : class, ICorrelationContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<ICorrelationContext, TCorrelationContext>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="ICorrelationEvent"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCorrelationEvent(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<CorrelationEvent>();
            services.Services.AddScoped<ICorrelationEvent>(provider => provider.GetRequiredService<CorrelationEvent>());
            return services;
        }

        /// <summary>
        /// Adds correlation behavior to commands and queries that are decorated with the <see cref="ICorrelationDecorator"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCorrelationEventDecorator(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddXCorrelationEvent();

            services.Services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandCorrelationDecorator<>));
            services.Services.XTryDecorate(typeof(ICommandHandler<,>), typeof(CommandCorrelationDecorator<,>));
            services.Services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryCorrelationDecorator<,>));
            services.Services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryCorrelationDecorator<,>));

            return services;
        }
    }
}


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
using Xpandables.Net.Correlation;
using Xpandables.Net.Events;
using Xpandables.Net.Queries;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
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
        /// Adds the <see cref="ICorrelationContext"/> to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationContext(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<CorrelationContext>();
            services.AddScoped<ICorrelationContext>(provider => provider.GetRequiredService<CorrelationContext>());
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

            services.AddScoped<CorrelationContext>();
            services.AddScoped<ICorrelationContext>(provider => provider.GetRequiredService<CorrelationContext>());

            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandCorrelationDecorator<>));
            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandCorrelationDecorator<>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryCorrelationDecorator<,>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryCorrelationDecorator<,>));

            return services;
        }
    }
}


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

using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.CQRS;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    ///  Provides method to register data context.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IDataContext"/> class reference implementation found from the executing assembly to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXDataContext<TDataContext>(this IServiceCollection services)
            where TDataContext : class, IDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            var serviceDescriptor = new ServiceDescriptor(typeof(IDataContext), provider => provider.GetRequiredService<TDataContext>(), ServiceLifetime.Scoped);
            services.Add(serviceDescriptor);
            services.AddScoped(typeof(IDataContext<>), typeof(DataContext<>));

            return services;
        }

        /// <summary>
        /// Adds persistence behavior to commands and queries that are decorated with the <see cref="IPersistenceDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXPersistenceDecorator(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandPersistenceDecorator<>));
            services.XTryDecorate(typeof(IAsyncCommandHandler<,>), typeof(AsyncCommandPersistenceDecorator<,>));
            return services;
        }
    }
}

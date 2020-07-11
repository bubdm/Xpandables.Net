
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

using Xpandables.Net5.Commands;
using Xpandables.Net5.Identities;
using Xpandables.Net5.Queries;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net5.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the identity data type to the services.
        /// </summary>
        /// <typeparam name="TIdentityProvider">The identity type provider.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIdentityProvider<TIdentityProvider>(this IServiceCollection services)
            where TIdentityProvider : class, IIdentityProvider
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.AddScoped<IIdentityProvider, TIdentityProvider>();
        }

        /// <summary>
        /// Adds identity data behavior to commands and queries that are decorated with the <see cref="IBehaviorIdentity"/> to the services
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXIdentityBehavior(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate(typeof(ICommandHandler<>), typeof(CommandIdentityBehavior<>));
            services.XTryDecorate(typeof(IQueryHandler<,>), typeof(QueryIdentityBehavior<,>));

            return services;
        }
    }
}

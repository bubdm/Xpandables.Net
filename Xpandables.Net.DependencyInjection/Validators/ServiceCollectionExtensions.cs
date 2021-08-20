
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
using System.Linq;
using System.Reflection;

using Xpandables.Net.Validators;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="IMetadataDescriptionProvider"/> to the services collection.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXMetadataDescriptionProvider(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddTransient<IMetadataDescriptionProvider, MetadataDescriptionProvider>();
            return services;
        }


        /// <summary>
        /// Adds the <see cref="IValidator{TArgument}"/> to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IXpandableServiceBuilder AddXValidators(this IXpandableServiceBuilder services, params Assembly[] assemblies)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            var genericValidators = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract
                               && !type.IsInterface
                               && !type.IsGenericType
                               && type.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .Select(type => new { Type = type, Interfaces = type.GetInterfaces().Where(inter => inter.IsGenericType
                                                                                                    && inter.GetGenericTypeDefinition() == typeof(IValidator<>)) })
                .ToList();

            foreach (var validator in genericValidators)
            {
                foreach (var interf in validator.Interfaces)
                {
                    services.Services.AddScoped(interf, validator.Type);
                }
            }

            return services;
        }
    }
}

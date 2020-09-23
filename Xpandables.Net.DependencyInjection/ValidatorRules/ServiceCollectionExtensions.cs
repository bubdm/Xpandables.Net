﻿
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
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;
using Xpandables.Net.Validations;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register validation rules.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds validation behavior to commands and queries that are decorated with the <see cref="IValidationDecorator"/> to the services
        /// with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXValidationDecorator(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(ICompositeValidation<>), typeof(CompositeValidation<>));
            services.XTryDecorate(typeof(IAsyncCommandHandler<>), typeof(AsyncCommandValidatorDecorator<>));
            services.XTryDecorate(typeof(IAsyncQueryHandler<,>), typeof(AsyncQueryValidatorDecorator<,>));
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IValidation{TArgument}"/> to the services with transient life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXValidations(this IServiceCollection services, Assembly[] assemblies)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            services.XRegister(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IValidation<>))
                    .Where(_ => !_.IsInterface && !_.IsAbstract && !_.IsGenericType))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

            return services;
        }
    }
}

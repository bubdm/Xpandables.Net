
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

using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <typeparamref name="TUnitOfWorkImplementation"/> for <typeparamref name="TUnitOfWorkInterface"/> unit of work to the services with scoped life time.
        /// Do not use with multi-tenancy.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXUnitOfWork<TUnitOfWorkInterface, TUnitOfWorkImplementation>(this IXpandableServiceBuilder services)
            where TUnitOfWorkInterface : class, IUnitOfWork
            where TUnitOfWorkImplementation : class, TUnitOfWorkInterface
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<TUnitOfWorkInterface, TUnitOfWorkImplementation>();
            services.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<TUnitOfWorkInterface>());
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TUnitOfWorkContextFactory"/> as <see cref="IUnitOfWorkContextFactory"/> type to the collection of services.
        /// </summary>
        /// <typeparam name="TUnitOfWorkContextFactory">The type of unit of work context factory.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXUnitOfWorkContextFactory<TUnitOfWorkContextFactory>(this IXpandableServiceBuilder services)
            where TUnitOfWorkContextFactory : class, IUnitOfWorkContextFactory
        {
            services.Services.AddScoped<IUnitOfWorkContextFactory, TUnitOfWorkContextFactory>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TUnitOfWork"/> type to the collection of tenants in multi-tenancy context.
        /// The tenant will be named as the type of the unit of work.
        /// </summary>
        /// <typeparam name="TUnitOfWork">The type of unit of work.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXUnitOfWorkMultiTenancy<TUnitOfWork>(this IXpandableServiceBuilder services)
            where TUnitOfWork : class, IUnitOfWork
        {
            var serviceDescriptor = new ServiceDescriptor(
                typeof(IUnitOfWorkMultiTenancy),
                provider => new UnitOfWorkMultiTenancy<TUnitOfWork>(() => provider.GetRequiredService<TUnitOfWork>()),
                ServiceLifetime.Scoped);

            services.Services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TUnitOfWork"/> type to the collection of tenants in multi-tenancy context.
        /// </summary>
        /// <typeparam name="TUnitOfWork">The type of unit of work.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="name">The unique identifier of the tenant.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXUnitOfWorkMultiTenancy<TUnitOfWork>(this IXpandableServiceBuilder services, string name)
            where TUnitOfWork : class, IUnitOfWork
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));

            var serviceDescriptor = new ServiceDescriptor(
                typeof(IUnitOfWorkMultiTenancy),
                provider => new UnitOfWorkMultiTenancy<TUnitOfWork>(name, () => provider.GetRequiredService<TUnitOfWork>()),
                ServiceLifetime.Scoped);

            services.Services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IUnitOfWorkMultiTenancyAccessor"/> implementation type that get called to resolve <see cref="IUnitOfWork"/> in multi-tenancy context.
        /// You have to register your unit of work(s) using the <see cref="AddXUnitOfWorkMultiTenancy{TUnitOfWork}(IXpandableServiceBuilder)"/>.
        /// The type is registered with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXUnitOfWorkMultiTenancyAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IUnitOfWorkMultiTenancyAccessor, UnitOfWorkMultiTenancyAccessor>();

            var serviceDescriptor = new ServiceDescriptor(
                typeof(IUnitOfWork),
                provider => provider.GetRequiredService<IUnitOfWorkMultiTenancyAccessor>().GetUnitOfWork(),
                ServiceLifetime.Scoped);

            services.Services.Add(serviceDescriptor);

            return services;
        }
    }
}

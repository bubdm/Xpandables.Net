
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

using Xpandables.Net.Aggregates;
using Xpandables.Net.Database;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IAggregateAccessor{TAggregateId, TAggregate}"/> implementation 
        /// to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXAggregateAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped(typeof(IAggregateAccessor<,>), typeof(AggregateAccessor<,>));
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEmailEventAccessor{TAggregateId, TAggregate}"/> implementation 
        /// to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEmailEventAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped(typeof(IEmailEventAccessor<,>), typeof(EmailEventAccessor<,>));
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEntityAccessor{TEntity}"/> implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEntityAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            services.Services.AddScoped(typeof(IEntityAccessor<>), typeof(EntityAccessor<>));
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IEntityAccessor{TEntity}"/> implementation to the services with scope life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="entityAccessorType">The generic entity accessor that implements <see cref="IEntityAccessor{TEntity}"/> interface.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="entityAccessorType"/> is null.</exception>
        public static IXpandableServiceBuilder AddXEntityAccessor(this IXpandableServiceBuilder services, Type entityAccessorType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = entityAccessorType ?? throw new ArgumentNullException(nameof(entityAccessorType));

            if (!entityAccessorType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityAccessor<>)))
                throw new ArgumentException(
                    $"the type '{nameof(entityAccessorType)}' must implement the '{typeof(IEntityAccessor<>).GetNameWithoutGenericArity()}' interface.");

            services.Services.AddScoped(typeof(IEntityAccessor<>), entityAccessorType);
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type class reference implementation as <see cref="IDataContext"/> to the services with scoped life time.
        /// Caution : Do not use with multi-tenancy.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context that implements <see cref="IDataContext"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDataContext<TDataContext>(this IXpandableServiceBuilder services)
            where TDataContext : class, IDataContext
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IDataContext, TDataContext>();
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type to the collection of tenants in multi-tenancy context.
        /// The tenant will be named as the type of the data context.
        /// <para>Caution : Do not use with <see cref="AddXDataContext{TDataContext}(IXpandableServiceBuilder)"/>.</para>
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDataContextTenant<TDataContext>(this IXpandableServiceBuilder services)
            where TDataContext : class, IDataContext
        {
            var serviceDescriptor = new ServiceDescriptor(
                typeof(IDataContextTenant),
                provider => new DataContextTenant<TDataContext>(() => provider.GetRequiredService<TDataContext>()),
                ServiceLifetime.Scoped);

            services.Services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds the <typeparamref name="TDataContext"/> type to the collection of tenants in multi-tenancy context.
        /// <para>Caution : Do not use with <see cref="AddXDataContext{TDataContext}(IXpandableServiceBuilder)"/>.</para>
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="name">The unique identifier of the tenant.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDataContextTenant<TDataContext>(this IXpandableServiceBuilder services, string name)
            where TDataContext : class, IDataContext
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));

            var serviceDescriptor = new ServiceDescriptor(
                typeof(IDataContextTenant),
                provider => new DataContextTenant<TDataContext>(name, () => provider.GetRequiredService<TDataContext>()),
                ServiceLifetime.Scoped);

            services.Services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds the <see cref="IDataContextTenantAccessor"/> implementation type that get called to resolve <see cref="IDataContext"/> in multi-tenancy context.
        /// You have to register your data context(s) using the <see cref="AddXDataContextTenant{TDataContext}(IXpandableServiceBuilder)"/>.
        /// The type is registered with scoped life time.
        /// <para>Caution : Do not use with <see cref="AddXDataContext{TDataContext}(IXpandableServiceBuilder)"/>.</para>
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXDataContextTenantAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IDataContextTenantAccessor, DataContextTenantAccessor>();

            var serviceDescriptor = new ServiceDescriptor(
                typeof(IDataContext),
                provider => provider.GetRequiredService<IDataContextTenantAccessor>().GetDataContext(),
                ServiceLifetime.Scoped);

            services.Services.Add(serviceDescriptor);

            return services;
        }

    }
}

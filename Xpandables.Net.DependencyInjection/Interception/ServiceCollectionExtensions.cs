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
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Aggregates.Events;
using Xpandables.Net.Commands;
using Xpandables.Net.Interception;
using Xpandables.Net.Queries;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register interception.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping the original registered <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface">The service type interface for which implementation will be wrapped by the given <typeparamref name="TInterceptor"/>.</typeparam>
        /// <typeparam name="TInterceptor">The interceptor type that will be used to wrap the original service type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentException">The <typeparamref name="TInterface"/> must be an interface.</exception>
        public static IXpandableServiceBuilder AddXInterceptor<TInterface, TInterceptor>(this IXpandableServiceBuilder services)
            where TInterface : class
            where TInterceptor : class, IInterceptor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (!typeof(TInterface).IsInterface) throw new ArgumentException($"{typeof(TInterface).Name} must be an interface.");

            services.XTryDecorate<TInterface>((instance, provider) =>
            {
                var interceptor = provider.GetRequiredService<TInterceptor>();
                return InterceptorFactory.CreateProxy(interceptor, instance);
            });

            return services;
        }

        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping the original registered <paramref name="interfaceType"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="interfaceType">The interface service type that will be wrapped by the given <paramref name="interceptorType"/>.</param>
        /// <param name="interceptorType">The interceptor type that will be used to wrap the original service type
        /// and should implement <see cref="IInterceptor"/>.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptorType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="interceptorType"/> must implement <see cref="IInterceptor"/>.</exception>
        public static IXpandableServiceBuilder AddXInterceptor(this IXpandableServiceBuilder services, Type interfaceType, Type interceptorType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            _ = interceptorType ?? throw new ArgumentNullException(nameof(interceptorType));

            if (!interfaceType.IsInterface) throw new ArgumentException($"{interfaceType.Name} must be an interface.");

            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
                throw new ArgumentException($"{nameof(interceptorType)} must implement {nameof(IInterceptor)}.");

            services.XTryDecorate(interfaceType, (instance, provider) =>
            {
                var interceptor = (IInterceptor)provider.GetRequiredService(interceptorType);
                return InterceptorFactory.CreateProxy(interfaceType, interceptor, instance);
            });

            return services;
        }

        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping all original registered handlers
        /// type for which the command/query implementing <see cref="IInterceptorDecorator"/> found in the specified collection of assemblies.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of interceptor.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IXpandableServiceBuilder AddXInterceptorHandlers<TInterceptor>(this IXpandableServiceBuilder services, params Assembly[] assemblies)
            where TInterceptor : class, IInterceptor => services.AddXInterceptorHandlers(typeof(TInterceptor), assemblies);

        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping all original registered handlers
        /// type for which the command/query implementing <see cref="IInterceptorDecorator"/> found in the specified collection of assemblies.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="interceptorType">The interceptor type that will be used to wrap the original service type
        /// and should implement <see cref="IInterceptor"/>.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types. If not set, the calling assembly will be used.</param>
        /// <returns>The <see cref="IXpandableServiceBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="interceptorType"/> must implement <see cref="IInterceptor"/>.</exception>
        public static IXpandableServiceBuilder AddXInterceptorHandlers(this IXpandableServiceBuilder services, Type interceptorType, params Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = interceptorType ?? throw new ArgumentNullException(nameof(interceptorType));

            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
                throw new ArgumentException($"{nameof(interceptorType)} must implement {nameof(IInterceptor)}.");

            if (assemblies.Length == 0) assemblies = new[] { Assembly.GetCallingAssembly() };

            var genericHandlerInterfaceTypes = new[] { typeof(IQueryHandler<,>), typeof(ICommandHandler<>),
                typeof(ICommandHandler<,>), typeof(IDomainEventHandler<>), typeof(INotificationHandler<>)};

            var handlers = assemblies
                .SelectMany(ass => ass.GetExportedTypes())
                .Where(type => !type.IsAbstract
                    && !type.IsInterface
                    && !type.IsGenericType
                    && type.GetInterfaces()
                        .Any(i => i.IsGenericType
                            && genericHandlerInterfaceTypes.Contains(i.GetGenericTypeDefinition())
                            && i.GetGenericArguments().Any(a => typeof(IInterceptorDecorator).IsAssignableFrom(a))))
                .Select(type => new
                {
                    Type = type,
                    Interfaces = type.GetInterfaces()
                        .Where(i => i.IsGenericType
                            && genericHandlerInterfaceTypes.Contains(i.GetGenericTypeDefinition()))
                });

            foreach (var hander in handlers)
                foreach (var typeInterface in hander.Interfaces)
                {
                    services.XTryDecorate(typeInterface, (instance, provider) =>
                    {
                        var interceptor = (IInterceptor)provider.GetRequiredService(interceptorType);
                        return InterceptorFactory.CreateProxy(typeInterface, interceptor, instance);
                    });
                }

            return services;
        }
    }
}


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
using System.Design.Interception;
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides method to register interception.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping the original registered <typeparamref name="TInstance"/>.
        /// </summary>
        /// <typeparam name="TInstance">The service type that will be wrapped by the given <typeparamref name="TInterceptor"/>.</typeparam>
        /// <typeparam name="TInterceptor">The interceptor type that will be used to wrap the original service type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXInterceptor<TInstance, TInterceptor>(this IServiceCollection services)
            where TInstance : class
            where TInterceptor : class, IInterceptor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.XTryDecorate<TInstance>((instance, provider) =>
            {
                var interceptor = provider.GetRequiredService<TInterceptor>();
                return InterceptorFactory.CreateProxy(interceptor, instance);
            });

            return services;
        }

        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping the original registered <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="serviceType">The service type that will be wrapped by the given <paramref name="interceptorType"/>.</param>
        /// <param name="interceptorType">The interceptor type that will be used to wrap the original service type
        /// and should implement <see cref="IInterceptor"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptorType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="interceptorType"/> must implement <see cref="IInterceptor"/>.</exception>
        public static IServiceCollection AddXInterceptor(this IServiceCollection services, Type serviceType, Type interceptorType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            if (interceptorType is null) throw new ArgumentNullException(nameof(interceptorType));
            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
                throw new ArgumentException($"{nameof(interceptorType)} must implement {nameof(IInterceptor)}.");

            services.XTryDecorate(serviceType, (instance, provider) =>
            {
                var interceptor = (IInterceptor)provider.GetRequiredService(interceptorType);
                return InterceptorFactory.CreateProxy(serviceType, interceptor, instance);
            });

            return services;
        }

        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping all original registered
        /// type implementing <see cref="IBehaviorInterceptor"/> found in the specified collection of assemblies.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of interceptor.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <param name="bindToInterface">Determines whether to use the first interface implementation from the class as source type.</param>
        /// <param name="predicate">A filter for the target type.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXInterceptor<TInterceptor>(
            this IServiceCollection services, bool bindToInterface, Predicate<Type> predicate, params Assembly[] assemblies)
            where TInterceptor : class, IInterceptor
            => services.AddXInterceptor(typeof(TInterceptor), bindToInterface, predicate, assemblies);

        /// <summary>
        /// Ensures that the supplied interceptor is returned, wrapping all original registered
        /// type implementing <see cref="IBehaviorInterceptor"/> found in the specified collection of assemblies.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="interceptorType">The interceptor type that will be used to wrap the original service type
        /// and should implement <see cref="IInterceptor"/>.</param>
        /// <param name="bindToInterface">Determines whether to use the first interface implementation from the class as source type.</param>
        /// <param name="predicate">A filter for the target type.</param>
        /// <param name="assemblies">The assemblies to scan for implemented types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        public static IServiceCollection AddXInterceptor(
            this IServiceCollection services, Type interceptorType, bool bindToInterface, Predicate<Type> predicate, params Assembly[] assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = predicate ?? throw new ArgumentNullException(nameof(predicate));
            if (interceptorType is null) throw new ArgumentNullException(nameof(interceptorType));
            if (assemblies?.Any() != true) throw new ArgumentNullException(nameof(assemblies));

            foreach (var serviceType in assemblies
                .SelectMany(assembly => assembly.GetExportedTypes()
                .Where(t => typeof(IBehaviorInterceptor).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && predicate(t))
                .Select(serviceType => bindToInterface ? serviceType.GetInterfaces().First() : serviceType)))
            {
                services.XTryDecorate(serviceType, (instance, provider) =>
                {
                    var interceptor = (IInterceptor)provider.GetRequiredService(interceptorType);
                    return InterceptorFactory.CreateProxy(serviceType, interceptor, instance);
                });
            }

            return services;
        }
    }
}

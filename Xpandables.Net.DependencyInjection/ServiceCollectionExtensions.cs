
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xpandables.Net.Types;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides with lazy instance resolution.
    /// </summary>
    /// <typeparam name="T">The type to be resolved.</typeparam>
    public sealed class LazyResolved<T> : Lazy<T>
        where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyResolved{T}" /> class that uses a preinitialized specified value from the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for preinitialized value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public LazyResolved(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<T>()) { }
    }

    /// <summary>
    /// Provides with methods to add decorator to classes.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Ensures that any <see cref="Lazy{T}"/> requested service will return <see cref="LazyResolved{T}"/> wrapping the original registered type.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXLazyTransient(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(Lazy<>), typeof(LazyResolved<>));
            return services;
        }

        /// <summary>
        /// Registers a configuration instance which TOptions will bind against and adds a transient service of the type specified in <typeparamref name="TOptions"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection XConfigureOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.Configure<TOptions>(configuration.GetSection(typeof(TOptions).Name));
            services.AddTransient(_ => configuration.GetSection(typeof(TOptions).Name).Get<TOptions>());

            return services;
        }

        /// <summary>
        /// Determines whether or not the collection of services already contain the specified service type.
        /// </summary>
        /// <param name="services">The collection of services to act on.</param>
        /// <param name="serviceType">The service type to check the registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is null.</exception>
        public static bool HasRegistration(this IServiceCollection services, Type serviceType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

            return services.Any(descriptor => descriptor.ServiceType == serviceType);
        }

        /// <summary>
        /// Determines whether or not the collection of services already contain the specified service type.
        /// </summary>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static bool HasRegistration<TService>(this IServiceCollection services)
            where TService : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.HasRegistration(typeof(TService));
        }

        /// <summary>
        /// Ensures that the supplied <typeparamref name="TDecorator"/> decorator is returned, wrapping the
        /// original registered <typeparamref name="TService"/>, by injecting that service type into the
        /// constructor of the supplied <typeparamref name="TDecorator"/>. Multiple decorators may be applied
        /// to the same <typeparamref name="TService"/>. By default, a new <typeparamref name="TDecorator"/>
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <typeparam name="TService">The service type that will be wrapped by the given
        /// <typeparamref name="TDecorator"/>.</typeparam>
        /// <typeparam name="TDecorator">The decorator type that will be used to wrap the original service type.
        /// </typeparam>
        /// <param name="services">The collection of services to act on.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        public static IServiceCollection XTryDecorate<TService, TDecorator>(this IServiceCollection services)
            where TService : class
            where TDecorator : class, TService
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            return services.XTryDecorate(typeof(TService), typeof(TDecorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <typeparamref name="TService"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <typeparamref name="TService"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <typeparam name="TService">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</typeparam>
        /// <param name="services">The collection of services to act on.</param>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection XTryDecorate<TService>(
            this IServiceCollection services,
            Func<TService, IServiceProvider, TService> decorator)
            where TService : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = decorator ?? throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(typeof(TService), serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <paramref name="serviceType"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <paramref name="serviceType"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <param name="services">The collection of services to act on.</param>
        /// <param name="serviceType">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</param>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceType"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection XTryDecorate(
            this IServiceCollection services,
            Type serviceType,
            Func<object, IServiceProvider, object> decorator)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = decorator ?? throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(serviceType, serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <typeparamref name="TService"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <typeparamref name="TService"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <typeparam name="TService">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</typeparam>
        /// <param name="services">The collection of services to act on.</param>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection XTryDecorate<TService>(
            this IServiceCollection services,
            Func<TService, TService> decorator)
            where TService : class
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = decorator ?? throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(typeof(TService), serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decorator"/> function decorator is returned, wrapping the
        /// original registered <paramref name="serviceType"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decorator"/> function. Multiple decorators may be applied
        /// to the same <paramref name="serviceType"/>. By default, a new <paramref name="decorator"/> function
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <param name="services">The collection of services to act on.</param>
        /// <param name="serviceType">The service type that will be wrapped by the given
        /// <paramref name="decorator"/>.</param>
        /// <param name="decorator">The decorator function type that will be used to wrap the original service type.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceType"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decorator"/> argument is <c>null</c>.</exception>
        public static IServiceCollection XTryDecorate(
            this IServiceCollection services,
            Type serviceType,
            Func<object, object> decorator)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _ = decorator ?? throw new ArgumentNullException(nameof(decorator));

            return services.DecorateDescriptors(serviceType, serviceDescriptor => serviceDescriptor.DecorateDescriptor(decorator));
        }

        /// <summary>
        /// Ensures that the supplied <paramref name="decoratorType"/> decorator is returned, wrapping the
        /// original registered <paramref name="serviceType"/>, by injecting that service type into the
        /// constructor of the supplied <paramref name="decoratorType"/>. Multiple decorators may be applied
        /// to the same <paramref name="serviceType"/>. By default, a new <paramref name="decoratorType"/>
        /// instance will be returned on each request (according the
        /// <see langword="Transient">Transient</see> lifestyle), independently of the lifestyle of the
        /// wrapped service.
        /// <para>
        /// Multiple decorators can be applied to the same service type. The order in which they are registered
        /// is the order they get applied in. This means that the decorator that gets registered first, gets
        /// applied first, which means that the next registered decorator, will wrap the first decorator, which
        /// wraps the original service type.
        /// </para>
        /// </summary>
        /// <param name="services">The collection of services to act on.</param>
        /// <param name="serviceType">The service type that will be wrapped by the given decorator.</param>
        /// <param name="decoratorType">The decorator type that will be used to wrap the original service type.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceType"/> argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="decoratorType"/> argument is <c>null</c>.</exception>
        public static IServiceCollection XTryDecorate(
            this IServiceCollection services,
            Type serviceType,
            Type decoratorType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _ = decoratorType ?? throw new ArgumentNullException(nameof(decoratorType));

            return serviceType.GetTypeInfo().IsGenericTypeDefinition && decoratorType.GetTypeInfo().IsGenericTypeDefinition
                ? services.DecorateOpenGenerics(serviceType, decoratorType)
                : services.DecorateDescriptors(serviceType, serviceDescriptor => serviceDescriptor.DecorateDescriptor(decoratorType));
        }

        private static IServiceCollection DecorateOpenGenerics(
            this IServiceCollection services,
            Type serviceType,
            Type decoratorType)
        {
            foreach (var argument in services.GetArgumentTypes(serviceType))
            {
                if (serviceType.TryMakeGenericType(out var closedServiceType, out _, argument)
                    && decoratorType.TryMakeGenericType(out var closedDecoratorType, out _, argument))
                {
                    services.DecorateDescriptors(closedServiceType!, descriptor => descriptor.DecorateDescriptor(closedDecoratorType!));
                }
            }

            return services;
        }

        private static IServiceCollection DecorateDescriptors(
            this IServiceCollection services,
            Type serviceType,
            Func<ServiceDescriptor, ServiceDescriptor> decorator)
        {
            foreach (var descriptor in services.GetServiceDescriptors(serviceType))
            {
                var index = services.IndexOf(descriptor);
                services[index] = decorator(descriptor);
            }

            return services;
        }

        private static ICollection<Type[]> GetArgumentTypes(
            this IServiceCollection services,
            Type serviceType)
            => services
                .Where(x => !x.ServiceType.IsGenericTypeDefinition && IsSameGenericType(x.ServiceType, serviceType))
                .Select(x => x.ServiceType.GenericTypeArguments)
                .ToArray();

        private static bool IsSameGenericType(Type t1, Type t2)
            => t1.IsGenericType
                && t2.IsGenericType
                && t1.GetGenericTypeDefinition() == t2.GetGenericTypeDefinition();

        private static ICollection<ServiceDescriptor> GetServiceDescriptors(
            this IServiceCollection services,
            Type serviceType)
            => services.Where(service => service.ServiceType == serviceType).ToArray();

        private static ServiceDescriptor DecorateDescriptor(
            this ServiceDescriptor descriptor,
            Type decoratorType)
            => descriptor.WithFactory(
                provider => ActivatorUtilities.CreateInstance(provider, decoratorType, new object[] { provider.GetInstance(descriptor) }));

        private static ServiceDescriptor DecorateDescriptor<TService>(
            this ServiceDescriptor descriptor,
            Func<TService, IServiceProvider, TService> decorator)
            where TService : class
            => descriptor.WithFactory(provider => decorator((TService)provider.GetInstance(descriptor), provider));

        private static ServiceDescriptor DecorateDescriptor<TService>(
            this ServiceDescriptor descriptor,
            Func<TService, TService> decorator)
            where TService : class
            => descriptor.WithFactory(provider => decorator((TService)provider.GetInstance(descriptor)));

        private static ServiceDescriptor WithFactory(this ServiceDescriptor descriptor, Func<IServiceProvider, object> factory)
        {
            _ = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            _ = factory ?? throw new ArgumentNullException(nameof(factory));

            return ServiceDescriptor.Describe(descriptor.ServiceType, factory, descriptor.Lifetime);
        }

        private static object GetInstance(this IServiceProvider serviceProvider, ServiceDescriptor descriptor)
        {
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _ = descriptor ?? throw new ArgumentNullException(nameof(descriptor));

            if (descriptor.ImplementationInstance != null)
                return descriptor.ImplementationInstance;

            if (descriptor.ImplementationType != null)
                return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, descriptor.ImplementationType);

            if (descriptor.ImplementationFactory is { })
                return descriptor.ImplementationFactory(serviceProvider);

            throw new InvalidOperationException($"Unable to get instance from descriptor.");
        }
    }
}

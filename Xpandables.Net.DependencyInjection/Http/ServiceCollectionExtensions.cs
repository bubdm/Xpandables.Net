
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
using System.IO;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Http;
using Xpandables.Net.Types;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        const string HttpHeaderAccessorAssemblyName = "Xpandables.NetCore.dll";
        const string HttpHeaderAccessorName = "HttpHeaderAccessor";

        /// <summary>
        /// Adds the default HTTP request header values accessor that implements the <see cref="IHttpHeaderAccessor"/> from "Xpandables.NetCore.dll" assembly.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpHeaderAccessorExtended(this IServiceCollection services)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, HttpHeaderAccessorAssemblyName);
            if (path.TryLoadAssembly(out var assembly, out var exception))
            {
                var httpHeaderAccessor = assembly
                    .GetExportedTypes()
                    .First(type => type.Name.Equals(HttpHeaderAccessorName, StringComparison.InvariantCulture));

                services.AddScoped(typeof(IHttpHeaderAccessor), httpHeaderAccessor);
            }
            else
            {
                throw new InvalidOperationException($"Type not found : {HttpHeaderAccessorName}. Add reference to {HttpHeaderAccessorAssemblyName}", exception);
            }

            return services;
        }

        /// <summary>
        /// Adds a delegate that will be used to provide the authorization token before request execution
        /// using an implementation of <see cref="IHttpTokenAccessor"/>. You can register the default implementation using
        /// the <see cref="AddXHttpTokenAccessor(IServiceCollection)"/>.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IHttpClientBuilder.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
        public static IHttpClientBuilder ConfigureXPrimaryAuthorizationTokenHandler(this IHttpClientBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.ConfigurePrimaryHttpMessageHandler(provider =>
            {
                var httpTokenProvider = provider.GetRequiredService<IHttpTokenAccessor>();
                return new AuthorizationHttpTokenHandler(httpTokenProvider);
            });

            return builder;
        }

        /// <summary>
        /// Adds a delegate that will be used to provide the authorization token before request execution
        /// using the token provider function.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IHttpClientBuilder.</param>
        /// <param name="tokenProvider">The delegate token provider to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenProvider"/> is null.</exception>
        public static IHttpClientBuilder ConfigureXPrimaryAuthorizationTokenHandler(
            this IHttpClientBuilder builder, HttpTokenAccessorDelegate tokenProvider)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));

            builder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpTokenProvider = new HttpTokenAccessorBuilder(tokenProvider);
                return new AuthorizationHttpTokenHandler(httpTokenProvider);
            });

            return builder;
        }

        /// <summary>
        /// Adds the specified HTTP request header values accessor that implements the <see cref="IHttpHeaderAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpHeaderAccessor">The type of HTTP request header.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpHeaderAccessor<THttpHeaderAccessor>(
            this IServiceCollection services)
            where THttpHeaderAccessor : class, IHttpHeaderAccessor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpHeaderAccessor, THttpHeaderAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the default HTTP request token accessor that implements the <see cref="IHttpTokenAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenAccessor(this IServiceCollection services)
            => services.AddXHttpTokenAccessor<HttpTokenAccessor>();

        /// <summary>
        /// Adds the specified HTTP request token accessor.
        /// The type should implement the <see cref="IHttpTokenAccessor"/>.
        /// </summary>
        /// <typeparam name="THttpTokenAccessor">The type of HTTP request token.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenAccessor<THttpTokenAccessor>(this IServiceCollection services)
            where THttpTokenAccessor : class, IHttpTokenAccessor
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpTokenAccessor, THttpTokenAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the specified token engine to the services collection.
        /// </summary>
        /// <typeparam name="THttpTokenEngine">The token engine type.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenEngine<THttpTokenEngine>(this IServiceCollection services)
            where THttpTokenEngine : class, IHttpTokenEngine
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            return services.AddXHttpTokenEngine(typeof(THttpTokenEngine));
        }

        /// <summary>
        /// Adds the specified token engine to the services collection with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="tokenEngineType">The type that implements <see cref="IHttpTokenEngine"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="tokenEngineType"/> is null.</exception>
        public static IServiceCollection AddXHttpTokenEngine(this IServiceCollection services, Type tokenEngineType)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = tokenEngineType ?? throw new ArgumentNullException(nameof(tokenEngineType));

            services.AddScoped(typeof(IHttpTokenEngine), tokenEngineType);
            return services;
        }
    }
}

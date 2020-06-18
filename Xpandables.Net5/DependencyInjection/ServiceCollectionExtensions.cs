
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

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides with lazy instance resolution.
    /// </summary>
    /// <typeparam name="T">The type to be resolved.</typeparam>
    public sealed class LazyResolved<T> : Lazy<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyResolved{T}" /> class that uses a preinitialized specified value from the service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for preinitialized value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public LazyResolved(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<T>()) { }
    }
    /// <summary>
    /// Provides method to register lazy.
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
    }
}

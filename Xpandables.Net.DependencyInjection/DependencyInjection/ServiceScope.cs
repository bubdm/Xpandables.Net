
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
using System.Collections.Generic;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// The implementation of <see cref="IServiceScope{TService}"/>.
    /// </summary>
    /// <typeparam name="TService">The type of service object to get.</typeparam>
    public sealed class ServiceScope<TService> : IServiceScope<TService>
        where TService : notnull
    {
        private readonly IServiceScope _serviceScope;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceScope{TService}"/> with the non generic <see cref="IServiceScope"/>.
        /// </summary>
        /// <param name="serviceScope">The non generic service scope.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceScope"/> is null.</exception>
        public ServiceScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        /// <summary>
        /// Get service of type <typeparamref name="TService" /> from the <see cref="IServiceProvider" />.
        /// </summary>
        /// <returns>A service object of type <typeparamref name="TService" />.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type <typeparamref name="TService" />.</exception>
        public TService GetRequiredService()
        {
            return _serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        /// <summary>
        ///  Get service of type <typeparamref name="TService" /> from the <see cref="IServiceProvider" />.
        /// </summary>
        /// <returns> A service object of type <typeparamref name="TService" /> or null if there is no such service.</returns>
        public TService? GetService()
        {
            return _serviceScope.ServiceProvider.GetService<TService>();
        }

        /// <summary>
        /// Get an enumeration of services of type <typeparamref name="TService" /> from the <see cref="IServiceProvider" />.
        /// </summary>
        /// <returns> An enumeration of services of type <typeparamref name="TService" />.</returns>
        public IEnumerable<TService> GetServices()
        {
            return _serviceScope.ServiceProvider.GetServices<TService>();
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _serviceScope?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

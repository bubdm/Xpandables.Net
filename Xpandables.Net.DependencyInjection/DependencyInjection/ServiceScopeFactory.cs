
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

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// The implementation of <see cref="IServiceScopeFactory{TService}"/>.
    /// </summary>
    /// <typeparam name="TService">The type of service object to get.</typeparam>
    public sealed class ServiceScopeFactory<TService> : IServiceScopeFactory<TService>
        where TService : notnull
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceScopeFactory{TService}"/> with the non-generic <see cref="IServiceScopeFactory"/>.
        /// </summary>
        /// <param name="serviceScopeFactory">The service scope factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceScopeFactory"/> is null.</exception>
        public ServiceScopeFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        /// Creates a new <see cref="IServiceScope{TService}" /> that can be used to resolve scoped <typeparamref name="TService" />.
        /// </summary>
        /// <returns>An <see cref="IServiceScope{TService}" />  that can be used to resolve scoped <typeparamref name="TService" />.</returns>
        public IServiceScope<TService> CreateScope()
            => new ServiceScope<TService>(_serviceScopeFactory.CreateScope());
    }
}

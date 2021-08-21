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

namespace Xpandables.Net.UnitOfWorks;

/// <summary>
/// Default implementation of <see cref="IUnitOfWorkContextFactory"/> using <see cref="IServiceProvider"/>.
/// </summary>
public class UnitOfWorkContextFactory : IUnitOfWorkContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="UnitOfWorkContextFactory"/> with the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to act with.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
    public UnitOfWorkContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    TUnitOfWorkContext IUnitOfWorkContextFactory.CreateUnitOfWorkContext<TUnitOfWorkContext>() => _serviceProvider.GetRequiredService<TUnitOfWorkContext>();
}

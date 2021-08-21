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

namespace Xpandables.Net.DependencyInjection;

/// <summary>
/// A factory for creating instance of <see cref="IServiceScope{TService}"/> which is used to create instances of <typeparamref name="TService"/> within a scope.
/// </summary>
/// <typeparam name="TService">The type of service object to get.</typeparam>
public interface IServiceScopeFactory<out TService>
    where TService : notnull
{
    /// <summary>
    /// Creates a new <see cref="IServiceScope{TService}"/> that can be used to resolve scoped <typeparamref name="TService"/>.
    /// </summary>
    /// <returns>An <see cref="IServiceScope{TService}"/>  that can be used to resolve scoped <typeparamref name="TService"/>.</returns>
    IServiceScope<TService> CreateScope();
}

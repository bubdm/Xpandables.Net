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
/// The <see cref="IDisposable.Dispose"/> method ends the scope lifetime. Once Dispose is called, any scoped <typeparamref name="TService"/>s 
/// that have been resolved from <see cref="IServiceScope{TService}"/> will be disposed.
/// </summary>
/// <typeparam name="TService">The type of service object to get.</typeparam>
public interface IServiceScope<out TService> : IDisposable
    where TService : notnull
{
    /// <summary>
    /// Get service of type <typeparamref name="TService"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <returns>A service object of type <typeparamref name="TService"/>.</returns>
    /// <exception cref="InvalidOperationException">There is no service of type <typeparamref name="TService"/>.</exception>
    TService GetRequiredService();

    /// <summary>
    ///  Get service of type <typeparamref name="TService"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <returns> A service object of type <typeparamref name="TService"/> or null if there is no such service.</returns>
    TService? GetService();

    /// <summary>
    /// Get an enumeration of services of type <typeparamref name="TService"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <returns> An enumeration of services of type <typeparamref name="TService"/>.</returns>
    IEnumerable<TService> GetServices();
}


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

namespace Xpandables.Net5.Registrations.Interfaces
{
    /// <summary>
    /// Provides with methods to select life times.
    /// </summary>
    public interface ILifetimeSelector : IServiceTypeSelector
    {
        /// <summary>
        /// Registers each matching concrete type with <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        IImplementationTypeSelector WithSingletonLifetime();

        /// <summary>
        /// Registers each matching concrete type with <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        IImplementationTypeSelector WithScopedLifetime();

        /// <summary>
        /// Registers each matching concrete type with <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        IImplementationTypeSelector WithTransientLifetime();

        /// <summary>
        /// Registers each matching concrete type with the specified <paramref name="lifetime"/>.
        /// </summary>
        IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime);
    }
}

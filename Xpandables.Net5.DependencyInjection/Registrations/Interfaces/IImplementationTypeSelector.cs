
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

namespace Xpandables.Net5.DependencyInjection.Registrations.Interfaces
{
    /// <summary>
    /// Provides with methods to add registrations.
    /// </summary>
    public interface IImplementationTypeSelector : IAssemblySelector
    {
        /// <summary>
        /// Adds all public, non-abstract classes from the selected assemblies to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        IServiceTypeSelector AddClasses();

        /// <summary>
        /// Adds all non-abstract classes from the selected assemblies to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="publicOnly">Specifies whether too add public types only.</param>
        IServiceTypeSelector AddClasses(bool publicOnly);

        /// <summary>
        /// Adds all public, non-abstract classes from the selected assemblies that
        /// matches the requirements specified in the <paramref name="action"/>
        /// to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="action">The filtering action.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="action"/> argument is <c>null</c>.</exception>
        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action);

        /// <summary>
        /// Adds all non-abstract classes from the selected assemblies that
        /// matches the requirements specified in the <paramref name="action"/>
        /// to the <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
        /// </summary>
        /// <param name="action">The filtering action.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="action"/> argument is <c>null</c>.</exception>
        /// <param name="publicOnly">Specifies whether too add public types only.</param>
        IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly);
    }
}

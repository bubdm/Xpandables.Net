
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
using System.Reflection;

namespace Xpandables.Net.DependencyInjection.Interfaces
{
    /// <summary>
    /// Provides with methods selector.
    /// </summary>
    public interface IServiceTypeSelector : IImplementationTypeSelector
    {
        /// <summary>
        /// Registers each matching concrete type as itself.
        /// </summary>
        ILifetimeSelector AsSelf();

        /// <summary>
        /// Registers each matching concrete type as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to register as.</typeparam>
        ILifetimeSelector As<T>();

        /// <summary>
        /// Registers each matching concrete type as each of the specified <paramref name="types" />.
        /// </summary>
        /// <param name="types">The types to register as.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(params Type[] types);

        /// <summary>
        /// Registers each matching concrete type as each of the specified <paramref name="types" />.
        /// </summary>
        /// <param name="types">The types to register as.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(IEnumerable<Type> types);

        /// <summary>
        /// Registers each matching concrete type as all of its implemented interfaces.
        /// </summary>
        ILifetimeSelector AsImplementedInterfaces();

        /// <summary>
        /// Registers each matching concrete type as all of its implemented interfaces, by returning an instance of the main type
        /// </summary>
        ILifetimeSelector AsSelfWithInterfaces();

        /// <summary>
        /// Registers the type with the first found matching interface name.  (e.g. ClassName is matched to IClassName)
        /// </summary>
        ILifetimeSelector AsMatchingInterface();

        /// <summary>
        /// Registers the type with the first found matching interface name.  (e.g. ClassName is matched to IClassName)
        /// </summary>
        /// <param name="action">Filter for matching the Type to an implementing interface</param>
        ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action);

        /// <summary>
        /// Registers each matching concrete type as each of the types returned
        /// from the <paramref name="selector"/> function.
        /// </summary>
        /// <param name="selector">A function to select service types based on implementation types.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="selector"/> argument is <c>null</c>.</exception>
        ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector);

        /// <summary>
        /// Registers each matching concrete type according to their <see cref="ServiceDescriptorAttribute"/>.
        /// </summary>
        IImplementationTypeSelector UsingAttributes();

        /// <summary>
        /// Applies the specified registration strategy.
        /// </summary>
        /// <param name="registrationStrategy">The strategy to be applied.</param>
        IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy);
    }
}

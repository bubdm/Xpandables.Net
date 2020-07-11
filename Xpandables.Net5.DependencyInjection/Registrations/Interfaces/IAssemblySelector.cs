
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
using Microsoft.Extensions.DependencyModel;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xpandables.Net5.DependencyInjection.Registrations.Interfaces
{
    /// <summary>
    /// Provides with methods to scan assemblies.
    /// </summary>
    public interface IAssemblySelector
    {
        /// <summary>
        /// Will scan for types from the calling assembly.
        /// </summary>
        IImplementationTypeSelector FromCallingAssembly();

        /// <summary>
        /// Will scan for types from the currently executing assembly.
        /// </summary>
        IImplementationTypeSelector FromExecutingAssembly();

        /// <summary>
        /// Will scan for types from the entry assembly.
        /// </summary>
        IImplementationTypeSelector FromEntryAssembly();

        /// <summary>
        /// Will load and scan all runtime libraries referenced by the currently executing application.
        /// Calling this method is equivalent to calling <see cref="FromDependencyContext(DependencyContext)"/> and passing in <see cref="DependencyContext.Default"/>.
        /// </summary>
        /// <remarks>
        /// If loading <see cref="DependencyContext.Default"/> fails, this method will fall back to calling <see cref="FromAssemblyDependencies(Assembly)"/>,
        /// using the entry assembly.
        /// </remarks>
        IImplementationTypeSelector FromApplicationDependencies();

        /// <summary>
        /// Will load and scan all runtime libraries referenced by the currently executing application.
        /// Calling this method is equivalent to calling <see cref="FromDependencyContext(DependencyContext, Func{Assembly, bool})"/> and passing in <see cref="DependencyContext.Default"/>.
        /// </summary>
        /// <remarks>
        /// If loading <see cref="DependencyContext.Default"/> fails, this method will fall back to calling <see cref="FromAssemblyDependencies(Assembly)"/>,
        /// using the entry assembly.
        /// </remarks>
        /// <param name="predicate">The predicate to match assemblies.</param>
        IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate);

        /// <summary>
        /// Will load and scan all runtime libraries referenced by the currently specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly whose dependencies should be scanned.</param>
        IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly);

        /// <summary>
        /// Will load and scan all runtime libraries in the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The dependency context.</param>
        IImplementationTypeSelector FromDependencyContext(DependencyContext context);

        /// <summary>
        /// Will load and scan all runtime libraries in the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The dependency context.</param>
        /// <param name="predicate">The predicate to match assemblies.</param>
        IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate);

        /// <summary>
        /// Will scan for types from the assembly of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type in which assembly that should be scanned.</typeparam>
        IImplementationTypeSelector FromAssemblyOf<T>();

        /// <summary>
        /// Will scan for types from the assemblies of each <see cref="Type"/> in <paramref name="types"/>.
        /// </summary>
        /// <param name="types">The types in which assemblies that should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssembliesOf(params Type[] types);

        /// <summary>
        /// Will scan for types from the assemblies of each <see cref="Type"/> in <paramref name="types"/>.
        /// </summary>
        /// <param name="types">The types in which assemblies that should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="types"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types);

        /// <summary>
        /// Will scan for types in each <see cref="Assembly"/> in <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies to should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="assemblies"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies);

        /// <summary>
        /// Will scan for types in each <see cref="Assembly"/> in <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">The assemblies to should be scanned.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="assemblies"/> argument is <c>null</c>.</exception>
        IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies);
    }
}

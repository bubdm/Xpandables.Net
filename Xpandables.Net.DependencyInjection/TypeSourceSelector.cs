
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
using Microsoft.Extensions.DependencyModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xpandables.Net.DependencyInjection.Interfaces;
using Xpandables.Net.DependencyInjection.Internals;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// <see cref="ITypeSourceSelector"/> and <see cref="ISelector"/> implementation.
    /// </summary>
    public class TypeSourceSelector : ITypeSourceSelector, ISelector
    {
#pragma warning disable CS1591

        private List<ISelector> Selectors { get; } = new List<ISelector>();

        /// <inheritdoc />
        public IImplementationTypeSelector FromAssemblyOf<T>()
        {
            return InternalFromAssembliesOf(new[] { typeof(T).GetTypeInfo() });
        }

        public IImplementationTypeSelector FromCallingAssembly()
        {
            return FromAssemblies(Assembly.GetCallingAssembly());
        }

        public IImplementationTypeSelector FromExecutingAssembly()
        {
            return FromAssemblies(Assembly.GetExecutingAssembly());
        }

        public IImplementationTypeSelector FromEntryAssembly()
        {
            return FromAssemblies(Assembly.GetEntryAssembly()!);
        }

        public IImplementationTypeSelector FromApplicationDependencies()
        {
            return FromApplicationDependencies(_ => true);
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate)
        {
            try
            {
                return FromDependencyContext(DependencyContext.Default, predicate);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                // Something went wrong when loading the DependencyContext, fall
                // back to loading all referenced assemblies of the entry assembly...
                return FromAssemblyDependencies(Assembly.GetEntryAssembly()!);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
        {
            return FromDependencyContext(context, _ => true);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var assemblies = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Select(Assembly.Load)
                .Where(predicate)
                .ToArray();

            return InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var assemblies = new List<Assembly> { assembly };

            try
            {
                var dependencyNames = assembly.GetReferencedAssemblies();

                foreach (var dependencyName in dependencyNames)
                {
                    try
                    {
                        // Try to load the referenced assembly...
                        assemblies.Add(Assembly.Load(dependencyName));
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch
                    {
                        // Failed to load assembly. Skip it.
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }

                return InternalFromAssemblies(assemblies);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                return InternalFromAssemblies(assemblies);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            return InternalFromAssembliesOf(types.Select(x => x.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            return InternalFromAssembliesOf(types.Select(t => t.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            return InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            return InternalFromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddTypes(params Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            Selectors.Add(selector);

            return selector.AddClasses();
        }

        public IServiceTypeSelector AddTypes(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            Selectors.Add(selector);

            return selector.AddClasses();
        }

        public void Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            foreach (var selector in Selectors)
            {
                selector.Populate(services, registrationStrategy);
            }
        }

        private IImplementationTypeSelector InternalFromAssembliesOf(IEnumerable<TypeInfo> typeInfos)
        {
            return InternalFromAssemblies(typeInfos.Select(t => t.Assembly));
        }

        private IImplementationTypeSelector InternalFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return AddSelector(assemblies.SelectMany(asm => asm.DefinedTypes).Select(x => x.AsType()));
        }

        private IImplementationTypeSelector AddSelector(IEnumerable<Type> types)
        {
            var selector = new ImplementationTypeSelector(this, types);

            Selectors.Add(selector);

            return selector;
        }
#pragma warning restore CS1591
    }
}


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
using Xpandables.Net.Types;

namespace Xpandables.Net.DependencyInjection.Internals
{
    internal class ServiceTypeSelector : IServiceTypeSelector, ISelector
    {
        public ServiceTypeSelector(IImplementationTypeSelector inner, IEnumerable<Type> types)
        {
            Inner = inner;
            Types = types;
        }

        private IImplementationTypeSelector Inner { get; }

        private IEnumerable<Type> Types { get; }

        private List<ISelector> Selectors { get; } = new List<ISelector>();

        private RegistrationStrategy? RegistrationStrategy { get; set; }

        public ILifetimeSelector AsSelf()
        {
            return As(t => new[] { t });
        }

        public ILifetimeSelector As<T>()
        {
            return As(typeof(T));
        }

        public ILifetimeSelector As(params Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            return As(types.AsEnumerable());
        }

        public ILifetimeSelector As(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            return AddSelector(Types.Select(t => new TypeMap(t, types)), Enumerable.Empty<TypeFactoryMap>());
        }

        public ILifetimeSelector AsImplementedInterfaces()
        {
            return AsTypeInfo(t => t.ImplementedInterfaces
                .Where(x => x.HasMatchingGenericArity(t))
                .Select(x => x.GetRegistrationType(t)));
        }

        public ILifetimeSelector AsSelfWithInterfaces()
        {
            IEnumerable<Type> Selector(TypeInfo info)
            {
                if (info.IsGenericTypeDefinition)
                {
                    // This prevents trying to register open generic types
                    // with an ImplementationFactory, which is unsupported.
                    return Enumerable.Empty<Type>();
                }

                return info.ImplementedInterfaces
                    .Where(x => x.HasMatchingGenericArity(info))
                    .Select(x => x.GetRegistrationType(info));
            }

            return AddSelector(
                Types.Select(t => new TypeMap(t, new[] { t })),
                Types.Select(t => new TypeFactoryMap(x => x.GetRequiredService(t), Selector(t.GetTypeInfo()))));
        }

        public ILifetimeSelector AsMatchingInterface()
        {
            return AsMatchingInterface(null);
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter>? action)
        {
            return AsTypeInfo(t => t.FindMatchingInterface(action));
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return AddSelector(Types.Select(t => new TypeMap(t, selector(t))), Enumerable.Empty<TypeFactoryMap>());
        }

        public IImplementationTypeSelector UsingAttributes()
        {
            var selector = new AttributeSelector(Types);

            Selectors.Add(selector);

            return this;
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
        {
            RegistrationStrategy = registrationStrategy ?? throw new ArgumentNullException(nameof(registrationStrategy));
            return this;
        }

        #region Chain Methods

        public IImplementationTypeSelector FromCallingAssembly()
        {
            return Inner.FromCallingAssembly();
        }

        public IImplementationTypeSelector FromExecutingAssembly()
        {
            return Inner.FromExecutingAssembly();
        }

        public IImplementationTypeSelector FromEntryAssembly()
        {
            return Inner.FromEntryAssembly();
        }

        public IImplementationTypeSelector FromApplicationDependencies()
        {
            return Inner.FromApplicationDependencies();
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate)
        {
            return Inner.FromApplicationDependencies(predicate);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
        {
            return Inner.FromAssemblyDependencies(assembly);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
        {
            return Inner.FromDependencyContext(context);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate)
        {
            return Inner.FromDependencyContext(context, predicate);
        }

        public IImplementationTypeSelector FromAssemblyOf<T>()
        {
            return Inner.FromAssemblyOf<T>();
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
        {
            return Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
        {
            return Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
        {
            return Inner.FromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return Inner.FromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddClasses()
        {
            return Inner.AddClasses();
        }

        public IServiceTypeSelector AddClasses(bool publicOnly)
        {
            return Inner.AddClasses(publicOnly);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action)
        {
            return Inner.AddClasses(action);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly)
        {
            return Inner.AddClasses(action, publicOnly);
        }

        #endregion

        internal void PropagateLifetime(ServiceLifetime lifetime)
        {
            foreach (var selector in Selectors.OfType<LifetimeSelector>())
            {
                selector.Lifetime = lifetime;
            }
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            if (Selectors.Count == 0)
            {
                AsSelf();
            }

            var strategy = RegistrationStrategy ?? registrationStrategy;

            foreach (var selector in Selectors)
            {
                selector.Populate(services, strategy);
            }
        }

        private ILifetimeSelector AddSelector(IEnumerable<TypeMap> types, IEnumerable<TypeFactoryMap> factories)
        {
            var selector = new LifetimeSelector(this, types, factories);

            Selectors.Add(selector);

            return selector;
        }

        private ILifetimeSelector AsTypeInfo(Func<TypeInfo, IEnumerable<Type>> selector)
        {
            return As(t => selector(t.GetTypeInfo()));
        }
    }

    internal static class Helpers
    {
        /// <summary>
        /// Find matching interface by name C# interface name convention.  Optionally use a filter.
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindMatchingInterface(this TypeInfo typeInfo, Action<TypeInfo, IImplementationTypeFilter>? action)
        {
            var matchingInterfaceName = $"I{typeInfo.Name}";

            var matchedInterfaces = GetImplementedInterfacesToMap(typeInfo)
                .Where(x => string.Equals(x.Name, matchingInterfaceName, StringComparison.Ordinal))
                .ToArray();

            Type? type;
            if (action is null)
            {
                type = matchedInterfaces.FirstOrDefault();
            }
            else
            {
                var filter = new ImplementationTypeFilter(matchedInterfaces);

                action(typeInfo, filter);

                type = filter.Types.FirstOrDefault();
            }

            if (type is null)
                yield break;

            yield return type;
        }

        private static IEnumerable<Type> GetImplementedInterfacesToMap(TypeInfo typeInfo)
        {
            if (!typeInfo.IsGenericType)
                return typeInfo.ImplementedInterfaces;

            if (!typeInfo.IsGenericTypeDefinition)
                return typeInfo.ImplementedInterfaces;

            return FilterMatchingGenericInterfaces(typeInfo);
        }

        private static IEnumerable<Type> FilterMatchingGenericInterfaces(TypeInfo typeInfo)
        {
            var genericTypeParameters = typeInfo.GenericTypeParameters;

            foreach (var current in typeInfo.ImplementedInterfaces)
            {
                var currentTypeInfo = current.GetTypeInfo();

                if (currentTypeInfo.IsGenericType && currentTypeInfo.ContainsGenericParameters
                    && GenericParametersMatch(genericTypeParameters, currentTypeInfo.GenericTypeArguments))
                {
                    yield return currentTypeInfo.GetGenericTypeDefinition();
                }
            }
        }

        private static bool GenericParametersMatch(IReadOnlyList<Type> parameters, IReadOnlyList<Type> interfaceArguments)
        {
            if (parameters.Count != interfaceArguments.Count)
                return false;

            for (var i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] != interfaceArguments[i])
                    return false;
            }

            return true;
        }
    }
}

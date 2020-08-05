
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
using System.Reflection;

using Xpandables.Net.DependencyInjection.Interfaces;
using Xpandables.Net.Extensions;

namespace Xpandables.Net.DependencyInjection.Internals
{
    internal sealed class LifetimeSelector : ILifetimeSelector, ISelector
    {
        public LifetimeSelector(ServiceTypeSelector inner, IEnumerable<TypeMap> typeMaps, IEnumerable<TypeFactoryMap> typeFactoryMaps)
        {
            Inner = inner;
            TypeMaps = typeMaps;
            TypeFactoryMaps = typeFactoryMaps;
        }

        private ServiceTypeSelector Inner { get; }

        private IEnumerable<TypeMap> TypeMaps { get; }

        private IEnumerable<TypeFactoryMap> TypeFactoryMaps { get; }

        public ServiceLifetime? Lifetime { get; set; }

        public IImplementationTypeSelector WithSingletonLifetime()
        {
            return WithLifetime(ServiceLifetime.Singleton);
        }

        public IImplementationTypeSelector WithScopedLifetime()
        {
            return WithLifetime(ServiceLifetime.Scoped);
        }

        public IImplementationTypeSelector WithTransientLifetime()
        {
            return WithLifetime(ServiceLifetime.Transient);
        }

        public IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime)
        {
            Inner.PropagateLifetime(lifetime);

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

        public ILifetimeSelector AsSelf()
        {
            return Inner.AsSelf();
        }

        public ILifetimeSelector As<T>()
        {
            return Inner.As<T>();
        }

        public ILifetimeSelector As(params Type[] types)
        {
            return Inner.As(types);
        }

        public ILifetimeSelector As(IEnumerable<Type> types)
        {
            return Inner.As(types);
        }

        public ILifetimeSelector AsImplementedInterfaces()
        {
            return Inner.AsImplementedInterfaces();
        }

        public ILifetimeSelector AsSelfWithInterfaces()
        {
            return Inner.AsSelfWithInterfaces();
        }

        public ILifetimeSelector AsMatchingInterface()
        {
            return Inner.AsMatchingInterface();
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action)
        {
            return Inner.AsMatchingInterface(action);
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector)
        {
            return Inner.As(selector);
        }

        public IImplementationTypeSelector UsingAttributes()
        {
            return Inner.UsingAttributes();
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
        {
            return Inner.UsingRegistrationStrategy(registrationStrategy);
        }

        #endregion

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy strategy)
        {
            if (!Lifetime.HasValue)
            {
                Lifetime = ServiceLifetime.Transient;
            }

            strategy ??= RegistrationStrategy.Append;

            foreach (var typeMap in TypeMaps)
            {
                foreach (var serviceType in typeMap.ServiceTypes)
                {
                    var implementationType = typeMap.ImplementationType;

                    if (!implementationType.IsAssignableTo(serviceType))
                    {
                        throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" is not assignable to ""${serviceType.ToFriendlyName()}"".");
                    }

                    var descriptor = new ServiceDescriptor(serviceType, implementationType, Lifetime.Value);

                    strategy.Apply(services, descriptor);
                }
            }

            foreach (var typeFactoryMap in TypeFactoryMaps)
            {
                foreach (var serviceType in typeFactoryMap.ServiceTypes)
                {
                    var descriptor = new ServiceDescriptor(serviceType, typeFactoryMap.ImplementationFactory, Lifetime.Value);

                    strategy.Apply(services, descriptor);
                }
            }
        }
    }
}

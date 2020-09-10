
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

using System;
using System.Collections.Generic;

using Xpandables.Net.Types;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides with an attributes that contains information about the decorated descriptor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ServiceDescriptorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptorAttribute"/>.
        /// </summary>
        public ServiceDescriptorAttribute() : this(null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptorAttribute"/> with a service type.
        /// </summary>
        /// <param name="serviceType"></param>
        public ServiceDescriptorAttribute(Type? serviceType) : this(serviceType, ServiceLifetime.Transient) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ServiceDescriptorAttribute"/> with a service type and a life time.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="lifetime">The default life time.</param>
        public ServiceDescriptorAttribute(Type? serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Gets the service type if defined.
        /// </summary>
        public Type? ServiceType { get; }

        /// <summary>
        /// Gets the current life time.
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// returns a collection of service types from the type.
        /// </summary>
        /// <param name="fallbackType">The type to act on.</param>
        public IEnumerable<Type> GetServiceTypes(Type fallbackType)
        {
            if (ServiceType is null)
            {
                yield return fallbackType;

                foreach (var type in fallbackType.GetBaseTypes())
                {
                    if (type == typeof(object))
                        continue;

                    yield return type;
                }

                yield break;
            }

            if (!fallbackType.IsAssignableTo(ServiceType))
            {
                throw new InvalidOperationException(
                    $@"Type ""{fallbackType.ToFriendlyName()}"" is not assignable to ""{ServiceType.ToFriendlyName()}"".");
            }

            yield return ServiceType;
        }
    }
}


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
using System.Linq;
using System.Reflection;

using Xpandables.Net5.DependencyInjection.Registrations.Interfaces;
using Xpandables.Net5.Helpers;

namespace Xpandables.Net5.DependencyInjection.Registrations
{
    internal class AttributeSelector : ISelector
    {
        public AttributeSelector(IEnumerable<Type> types) => Types = types;

        private IEnumerable<Type> Types { get; }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy options)
        {
            var strategy = options ?? RegistrationStrategy.Append;

            foreach (var type in Types)
            {
                var typeInfo = type.GetTypeInfo();

                var attributes = typeInfo.GetCustomAttributes<ServiceDescriptorAttribute>().ToArray();

                // Check if the type has multiple attributes with same ServiceType.
                var duplicates = GetDuplicates(attributes);

                if (duplicates.Any())
                {
                    throw new InvalidOperationException(
                        $@"Type ""{type.ToFriendlyName()}"" has multiple ServiceDescriptor attributes with the same service type.");
                }

                foreach (var attribute in attributes)
                {
                    foreach (var serviceType in attribute.GetServiceTypes(type))
                    {
                        var descriptor = new ServiceDescriptor(serviceType, type, attribute.Lifetime);
                        strategy.Apply(services, descriptor);
                    }
                }
            }
        }

        private static IEnumerable<ServiceDescriptorAttribute> GetDuplicates(IEnumerable<ServiceDescriptorAttribute> attributes)
            => attributes.GroupBy(s => s.ServiceType).SelectMany(grp => grp.Skip(1));
    }
}

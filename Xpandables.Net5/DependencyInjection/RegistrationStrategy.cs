
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides with the base class for registration strategies.
    /// </summary>
    public abstract class RegistrationStrategy
    {
        /// <summary>
        /// Skips registrations for services that already exists.
        /// </summary>
        public static readonly RegistrationStrategy Skip = new SkipRegistrationStrategy();

        /// <summary>
        /// Appends a new registration for existing services.
        /// </summary>
        public static readonly RegistrationStrategy Append = new AppendRegistrationStrategy();

        /// <summary>
        /// Throws when trying to register an existing service.
        /// </summary>
        public static readonly RegistrationStrategy Throw = new ThrowRegistrationStrategy();

        /// <summary>
        /// Replaces existing service registrations using <see cref="ReplacementBehavior.Default"/>.
        /// </summary>
        public static RegistrationStrategy Replace() => Replace(ReplacementBehavior.Default);

        /// <summary>
        /// Replaces existing service registrations based on the specified <see cref="ReplacementBehavior"/>.
        /// </summary>
        /// <param name="behavior">The behavior to use when replacing services.</param>
        public static RegistrationStrategy Replace(ReplacementBehavior behavior) => new ReplaceRegistrationStrategy(behavior);

        /// <summary>
        /// Applies the <see cref="ServiceDescriptor"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="descriptor">The descriptor to apply.</param>
        public abstract void Apply(IServiceCollection services, ServiceDescriptor descriptor);

        private sealed class SkipRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor) => services.TryAdd(descriptor);
        }

        private sealed class AppendRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor) => services.Add(descriptor);
        }

        private sealed class ThrowRegistrationStrategy : RegistrationStrategy
        {
            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
            {
                if (services.HasRegistration(descriptor.ServiceType))
                {
                    throw new InvalidOperationException(
                        $"A service of type '{TypeNameHelpers.GetTypeDisplayName(descriptor.ServiceType)}' has already been registered.");
                }

                services.Add(descriptor);
            }
        }

        private sealed class ReplaceRegistrationStrategy : RegistrationStrategy
        {
            public ReplaceRegistrationStrategy(ReplacementBehavior behavior) => Behavior = behavior;

            private ReplacementBehavior Behavior { get; }

            public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
            {
                var behavior = Behavior;

                if (behavior == ReplacementBehavior.Default)
                {
                    behavior = ReplacementBehavior.ServiceType;
                }

                if ((behavior & ReplacementBehavior.ServiceType) != 0)
                {
                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        if (services[i].ServiceType == descriptor.ServiceType)
                        {
                            services.RemoveAt(i);
                        }
                    }
                }

                if ((behavior & ReplacementBehavior.ImplementationType) != 0)
                {
                    for (var i = services.Count - 1; i >= 0; i--)
                    {
                        if (services[i].ImplementationType == descriptor.ImplementationType)
                        {
                            services.RemoveAt(i);
                        }
                    }
                }

                services.Add(descriptor);
            }
        }
    }
}

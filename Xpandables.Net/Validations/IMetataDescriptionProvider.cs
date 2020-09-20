
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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Enumerables;

using static System.FormattableString;

namespace Xpandables.Net.Validations
{
    /// <summary>
    /// Allows an application author to add type description provider for any class.
    /// For more information about description and others, see the <see cref="TypeDescriptor"/> class.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// Any operation that does not deliver or do what it promises to do should throw an exception. 
    /// </summary>
    public interface IMetataDescriptionProvider
    {
        /// <summary>
        /// Adds the type description provider specified by the meta-data description name for the current instance.
        /// The meta-data class must be name "{<paramref name="metadataDescriptionTypeName"/>}", must be public, and available from
        /// the <paramref name="instance"/> assembly or any custom loaded assembly in the current application domain.
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="instance">Contains the instance to which the meta-data type description will be added.</param>
        /// <param name="metadataDescriptionTypeName">Contains the name of the type used to find the meta-data class for 
        /// the current <paramref name="instance"/>.</param>
        /// <param name="assemblies">The collection of assemblies to search in.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="metadataDescriptionTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="assemblies"/> is null.</exception>
        public void AddProvider<TSource>(TSource instance, string metadataDescriptionTypeName, Assembly[] assemblies)
            where TSource : class
            => AddProviderFromStringName(instance, metadataDescriptionTypeName, assemblies);

        /// <summary>
        /// Adds the specified type description provider to the instance.
        /// Any operation that does not deliver or do what it promises to do should throw an exception. 
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="instance">Contains the instance to which the meta-data type description will be added.</param>
        /// <param name="metadataDescriptionType">Contains the meta-data type description to add to the current <paramref name="instance"/>.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="metadataDescriptionType"/> is null.</exception>
        public void AddProvider<TSource>(TSource instance, Type metadataDescriptionType)
            where TSource : class
            => AddMetadataDescriptionType(instance, metadataDescriptionType);

        /// <summary>
        /// Adds the type description provider specified by its name for the current instance.
        /// The meta-data class must be name "{InstanceTypeName}{Metadata}", must be public, 
        /// and available from the <paramref name="instance"/> assembly or any custom loaded assembly in the current application domain.
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="instance">Contains the instance to which the meta-data type description will be added.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when the <paramref name="instance"/> is null.</exception>
        public void AddProvider<TSource>(TSource instance)
            where TSource : class
        {
            _ = instance ?? throw new ArgumentNullException(nameof(instance));

            AddProviderFromStringName(
                           instance,
                           Invariant($"{instance.GetType().Name}Metadata"),
                           new[] { instance.GetType().Assembly });
        }

        /// <summary>
        /// Adds the description provider from the description type.
        /// </summary>
        /// <typeparam name="TSource">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="descriptionType">The description type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="descriptionType"/> is null.</exception>
        public static void AddProviderFromType<TSource>(TSource instance, Type descriptionType)
            where TSource : class
        {
            _ = instance ?? throw new ArgumentNullException(nameof(instance));
            _ = descriptionType ?? throw new ArgumentNullException(nameof(descriptionType));

            AddMetadataDescriptionType(instance, descriptionType);
        }

        /// <summary>
        /// Adds the description provider from the description type name.
        /// </summary>
        /// <typeparam name="TSource">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="metadataDescriptionTypeName">The meta data description type name.</param>
        /// <param name="assemblies">The collection of assemblies to search in.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="metadataDescriptionTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        [DebuggerStepThrough]
        public static void AddProviderFromStringName<TSource>(
             TSource instance,
             string metadataDescriptionTypeName,
             Assembly[] assemblies)
            where TSource : class
        {
            _ = instance ?? throw new ArgumentNullException(nameof(instance));
            _ = metadataDescriptionTypeName ?? throw new ArgumentNullException(nameof(metadataDescriptionTypeName));
            _ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

            var descriptionType = GetDescriptionTypeFromName(instance, metadataDescriptionTypeName, assemblies);
            AddMetadataDescriptionType(instance, descriptionType);
        }

        /// <summary>
        /// Returns the description type if found.
        /// </summary>
        /// <typeparam name="TSource">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="metadataDescriptionTypeName">The meta data description type name.</param>
        /// <param name="assemblies">The collection of assemblies to search in.</param>
        /// <returns>If found, returns the description type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="metadataDescriptionTypeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblies"/> is null.</exception>
        [DebuggerStepThrough]
        public static Type? GetDescriptionTypeFromName<TSource>(
             TSource instance,
             string metadataDescriptionTypeName,
             Assembly[] assemblies)
            where TSource : class
        {
            _ = instance ?? throw new ArgumentNullException(nameof(instance));
            _ = metadataDescriptionTypeName ?? throw new ArgumentNullException(nameof(metadataDescriptionTypeName));
            _ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

            return assemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                ?.FirstOrDefault(type => type.Name == metadataDescriptionTypeName);
        }

        /// <summary>
        /// Adds the description provider to the instance, if specified or adds the description from the current type;
        /// </summary>
        /// <typeparam name="TSource">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="descriptionType">The description <typeparamref name="TSource"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is null.</exception>
        [DebuggerStepThrough]
        public static void AddMetadataDescriptionType<TSource>(TSource instance, Type? descriptionType)
            where TSource : class
        {
            _ = instance ?? throw new ArgumentNullException(nameof(instance));

            if (descriptionType is { })
                TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(instance.GetType(), descriptionType), instance.GetType());
            else
                TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(instance.GetType()), instance.GetType());
        }

    }
}

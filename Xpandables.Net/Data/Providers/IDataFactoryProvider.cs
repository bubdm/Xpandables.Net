
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
using System.Data.Common;
using System.IO;
using System.Reflection;

using Xpandables.Net.Types;

namespace Xpandables.Net.Data.Providers
{
    /// <summary>
    /// Allows an application author to return a data provider factory from the specified provider type.
    /// The default implementation class is <see cref="DataFactoryProvider"/>.
    /// Contains default implementation.
    /// </summary>
    public interface IDataFactoryProvider
    {
        /// <summary>
        /// Returns an instance of the data provider factory matching the specified provider type.
        /// </summary>
        /// <param name="providerType">The provider type to find factory.</param>
        /// <returns>An instance of <see cref="DbProviderFactory" /> if found, otherwise an empty or exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType" /> is null.</exception>
        public DbProviderFactory? GetProviderFactory(DataProviderType providerType)
        {
            if (providerType is null) throw new ArgumentNullException(nameof(providerType));

            return GetProviderFactoryInstance(providerType) as DbProviderFactory;
        }

        private static object GetProviderFactoryInstance(DataProviderType dataProviderType)
        {
            if (dataProviderType.DisplayName.TryGetTypeFromTypeName(out var type, out var typeException)
                && type!.TryTypeInvokeMember(
                    out var provider,
                    out var providerException,
                    "Instance",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                    default,
                    type!,
                    default))
            {
                return provider!;
            }

            if (FormatDisplayName(dataProviderType.DisplayName).TryLoadAssembly(out var assembly, out var assemblyException)
                && Array.Find(assembly!.GetExportedTypes(), p => p.FullName == dataProviderType.ProviderFactoryTypeName) is Type providerType
                && providerType.TryTypeInvokeMember(
                    out var assemblyProvider,
                    out var assemblyProviderException,
                    "Instance",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                    default,
                    providerType,
                    default))
            {
                return assemblyProvider!;
            }

            return (typeException, assemblyException) switch
            {
                (null, Exception ex) => throw ex,
                (Exception ex1, Exception ex2) => throw new AggregateException(ex1, ex2),
                (Exception ex, null) => throw ex,
                (null, null) => throw new NotSupportedException()
            };

            static string FormatDisplayName(string displayName)
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, $"{ displayName}.dll");
            }
        }
    }
}

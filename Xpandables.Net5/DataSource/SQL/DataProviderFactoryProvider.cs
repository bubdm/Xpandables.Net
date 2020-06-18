
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
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// The default implementation to return data provider factory from provider type.
    /// </summary>
    public sealed class DataProviderFactoryProvider : IDataProviderFactoryProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="DataProviderFactoryProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceProvider"/> is null.</exception>
        public DataProviderFactoryProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

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

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            static string FormatDisplayName(string displayName)
                => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, $"{ displayName}.dll");
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType. -or- null if there is no service object of type serviceType.</returns>
        public object GetService(Type serviceType) => _serviceProvider.GetRequiredService(serviceType);
    }
}

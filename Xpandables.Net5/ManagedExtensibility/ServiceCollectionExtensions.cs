
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

using Xpandables.Net5.Helpers;
using Xpandables.Net5.ManagedExtensibility;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net5.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register exports.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures registration of services using the <see cref="IAddServiceExport"/> implementations found in the current application path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IServiceCollection AddXServiceExport(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            services.AddXServiceExport(configuration, _ => { });
            return services;
        }

        /// <summary>
        /// Adds and configures registration of services using the<see cref="IAddServiceExport"/> implementations found in the path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ExportServiceOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureOptions"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IServiceCollection AddXServiceExport(
            this IServiceCollection services, IConfiguration configuration, Action<ExportServiceOptions> configureOptions)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _ = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));

            var definedOptions = new ExportServiceOptions();
            configureOptions.Invoke(definedOptions);
            services.AddServiceExport(configuration, definedOptions);

            return services;
        }

        /// <summary>
        /// Adds exports services matching the specified options.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="options">The export options.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public static void AddServiceExport(this IServiceCollection services, IConfiguration configuration, ExportServiceOptions options)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            try
            {
                using var directoryCatalog = options.SearchSubDirectories
                    ? new RecursiveDirectoryCatalog(options.Path, options.SearchPattern)
                    : (ComposablePartCatalog)new DirectoryCatalog(options.Path, options.SearchPattern);

                var importDefinition = BuildAddImportDefinition();

                using var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(directoryCatalog);

                using var compositionContainer = new CompositionContainer(aggregateCatalog);
                var exportServices = compositionContainer
                    .GetExports(importDefinition)
                    .Select(def => def.Value)
                    .OfType<IAddServiceExport>();

                foreach (var export in exportServices)
                    export.AddServices(services, configuration);
            }
            catch (Exception exception) when (exception is NotSupportedException
                                            || exception is System.IO.DirectoryNotFoundException
                                            || exception is UnauthorizedAccessException
                                            || exception is ArgumentException
                                            || exception is System.IO.PathTooLongException
                                            || exception is ReflectionTypeLoadException)
            {
                throw new InvalidOperationException("Adding exports failed. See inner exception.", exception);
            }
        }

        private static ImportDefinition BuildAddImportDefinition()
            => new ImportDefinition(
                    _ => true,
                    typeof(IAddServiceExport).FullName,
                    ImportCardinality.ZeroOrMore,
                    false,
                    false);
    }
}

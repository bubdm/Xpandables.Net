
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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Provides method to register exports.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures registration of services using the<see cref="IAddServiceExport"/> implementations found in the current application path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IServiceCollection AddXServiceExport(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddXServiceExport(configuration, _ => new ExportServiceOptions());
            return services;
        }

        /// <summary>
        /// Adds and configures registration of services using the<see cref="IAddServiceExport"/> implementations found in the path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ExportServiceOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureOptions"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IServiceCollection AddXServiceExport(
            this IServiceCollection services, IConfiguration configuration, Action<ExportServiceOptions> configureOptions)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var definedOptions = new ExportServiceOptions();
            configureOptions.Invoke(definedOptions);
            services.AddServiceExport(configuration, definedOptions);

            return services;
        }

        private static void AddServiceExport(this IServiceCollection services, IConfiguration configuration, ExportServiceOptions options)
        {
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
                                            || exception is IO.DirectoryNotFoundException
                                            || exception is UnauthorizedAccessException
                                            || exception is ArgumentException
                                            || exception is IO.PathTooLongException
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


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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace Xpandables.Net5.ManagedExtensibility
{
    /// <summary>
    /// Provides with methods to register export services.
    /// </summary>
    public sealed class ExportServiceRegister
    {
        /// <summary>
        /// Adds exports services matching the specified options.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="options">The export options.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public static void AddServiceExport(object services, object configuration, ExportServiceOptions options)
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

        /// <summary>
        /// Uses exports services matching the specified options with application builder and environment.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to act on.</param>
        /// <param name="webHostEnvironment">The web hosting environment instance.</param>
        /// <param name="options">The export options.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="applicationBuilder"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="webHostEnvironment"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public static void UseServiceExport(object applicationBuilder, object webHostEnvironment, ExportServiceOptions options)
        {
            _ = applicationBuilder ?? throw new ArgumentNullException(nameof(applicationBuilder));
            _ = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _ = options ?? throw new ArgumentNullException(nameof(options));
            try
            {
                using var directoryCatalog = options.SearchSubDirectories
                    ? new RecursiveDirectoryCatalog(options.Path, options.SearchPattern)
                    : (ComposablePartCatalog)new DirectoryCatalog(options.Path, options.SearchPattern);

                var importDefinition = BuildUseImportDefinition();

                using var aggregateCatalog = new AggregateCatalog();
                aggregateCatalog.Catalogs.Add(directoryCatalog);

                using var compositionContainer = new CompositionContainer(aggregateCatalog);
                var exportServices = compositionContainer
                    .GetExports(importDefinition)
                    .Select(def => def.Value)
                    .OfType<IUseServiceExport>();

                foreach (var export in exportServices)
                    export.UseServices(applicationBuilder, webHostEnvironment);
            }
            catch (Exception exception) when (exception is NotSupportedException
                                            || exception is System.IO.DirectoryNotFoundException
                                            || exception is UnauthorizedAccessException
                                            || exception is ArgumentException
                                            || exception is System.IO.PathTooLongException
                                            || exception is ReflectionTypeLoadException)
            {
                throw new InvalidOperationException("Using exports failed. See inner exception.", exception);
            }
        }

        private static ImportDefinition BuildUseImportDefinition()
            => new ImportDefinition(
                    _ => true,
                    typeof(IUseServiceExport).FullName,
                    ImportCardinality.ZeroOrMore,
                    false,
                    false);

        private static ImportDefinition BuildAddImportDefinition()
            => new ImportDefinition(
                    _ => true,
                    typeof(IAddServiceExport).FullName,
                    ImportCardinality.ZeroOrMore,
                    false,
                    false);
    }
}

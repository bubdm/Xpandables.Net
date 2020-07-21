
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

using Xpandables.Net.ManagedExtensibility;

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net.DependencyInjection
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Provides method to register exports.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures application services using the<see cref="IUseServiceExport"/> implementations found in the current application path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="application">The collection of services.</param>
        /// <param name="environment">The web hosting environment instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="application"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IApplicationBuilder UseXServiceExport(this IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (application is null) throw new ArgumentNullException(nameof(application));
            return application.UseXServiceExport(environment, _ => { });
        }

        /// <summary>
        /// Adds and configures application services using the<see cref="IUseServiceExport"/> implementations found in the path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="application">The application builder instance.</param>
        /// <param name="environment">The </param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ExportServiceOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="application"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="environment"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureOptions"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IApplicationBuilder UseXServiceExport(
            this IApplicationBuilder application, IWebHostEnvironment environment, Action<ExportServiceOptions> configureOptions)
        {
            if (application is null) throw new ArgumentNullException(nameof(application));
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var definedOptions = new ExportServiceOptions();
            configureOptions.Invoke(definedOptions);
            application.UseServiceExport(environment, definedOptions);

            return application;
        }

        private static void UseServiceExport(this IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment, ExportServiceOptions options)
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
    }
}


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
using Microsoft.Extensions.DependencyInjection;

using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

using Xpandables.Net.Correlations;
using Xpandables.Net.Extensibility;
using Xpandables.Net.Http;
using Xpandables.Net.Middlewares;
using Xpandables.Net.Razors;

namespace Xpandables.Net.DependencyInjection
{
    /// <summary>
    /// Provides method to register services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default correlation context implementation type to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXCorrelationContext(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<ICorrelationContext, CorrelationContext>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="DataContextTenantMiddleware"/> type to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseXDataContextTenantMiddleware(this IApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.UseMiddleware<DataContextTenantMiddleware>();

            return builder;
        }

        /// <summary>
        /// Adds the HTTP context header values accessor that implements the <see cref="IHttpHeaderAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXHttpHeaderAccessor(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IHttpHeaderAccessor, HttpHeaderAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IRazorViewRenderer"/> implementation.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddXRazorViewRenderer(this IServiceCollection services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.AddScoped<IRazorViewRenderer, RazorViewRenderer>();
            services.AddSingleton<IRazorModelViewCollection, RazorModelViewCollection>();
            return services;
        }

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

        private static void UseServiceExport(
            this IApplicationBuilder application, IWebHostEnvironment environment, ExportServiceOptions options)
        {
            try
            {
                using var directoryCatalog = options.SearchSubDirectories
                    ? new UseRecursiveDirectoryCatalog(options.Path, options.SearchPattern)
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
                    export.UseServices(application, environment);
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
            => new(
                    _ => true,
                    typeof(IUseServiceExport).FullName,
                    ImportCardinality.ZeroOrMore,
                    false,
                    false);
    }
}


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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        /// Allows configuration of Xpandable Services.
        /// </summary>
        /// <param name="builder">The configuration builder to act on.</param>
        /// <returns>The Xpandable application builder.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="builder"/> is null.</exception>
        public static IXpandableApplicationBuilder UseXpandableApplications(this IApplicationBuilder builder)
            => new XpandableApplicationBuilder(builder);

        /// <summary>
        /// Adds the default <see cref="OperationResultConfigureJsonOptions"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultConfigureJsonOptions(this IXpandableServiceBuilder services)
            => services.AddXOperationResultConfigureJsonOptions<OperationResultConfigureJsonOptions>();

        /// <summary>
        /// Adds the specified <typeparamref name="TOperationResultConfigureJsonOptions"/> to the services.
        /// </summary>
        /// <typeparam name="TOperationResultConfigureJsonOptions">the type of operation result JSON configure.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultConfigureJsonOptions<TOperationResultConfigureJsonOptions>(this IXpandableServiceBuilder services)
            where TOperationResultConfigureJsonOptions : OperationResultConfigureJsonOptions
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<IConfigureOptions<JsonOptions>, TOperationResultConfigureJsonOptions>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="OperationResultConfigureMvcOptions"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultConfigureMvcOptions(this IXpandableServiceBuilder services)
            => services.AddXOperationResultConfigureMvcOptions<OperationResultConfigureMvcOptions>();

        /// <summary>
        /// Adds the specified <typeparamref name="TOperationResultConfigureMvcOptions"/> to the services.
        /// </summary>
        /// <typeparam name="TOperationResultConfigureMvcOptions">the type of operation result MVC configure.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultConfigureMvcOptions<TOperationResultConfigureMvcOptions>(this IXpandableServiceBuilder services)
            where TOperationResultConfigureMvcOptions : OperationResultConfigureMvcOptions
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddSingleton<IConfigureOptions<MvcOptions>, TOperationResultConfigureMvcOptions>();
            return services;
        }

        /// <summary>
        /// Adds the default correlation context implementation type to the services with scoped life time.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXCorrelationContext(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<ICorrelationContext, CorrelationContext>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="DataContextMultiTenancyMiddleware"/> type to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IXpandableApplicationBuilder UseXDataContextMultiTenancyMiddleware(this IXpandableApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Builder.UseMiddleware<DataContextMultiTenancyMiddleware>();

            return builder;
        }

        /// <summary>
        /// Adds the default <see cref="CorrelationMiddleware"/> type to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IXpandableApplicationBuilder UseXCorrelationMiddleware(this IXpandableApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            builder.Builder.UseMiddleware<CorrelationMiddleware>();

            return builder;
        }

        /// <summary>
        /// Adds the <see cref="OperationResultExceptionController"/> to the services.
        /// This controller is used to handle exceptions before target controller get called.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultExceptionController(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<OperationResultExceptionController>();
            return services;
        }

        /// <summary>
        /// Registers the <see cref="OperationResultExceptionMiddleware"/> to the services.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXOperationResultExceptionMiddleware(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<OperationResultExceptionController>();
            services.Services.AddScoped<OperationResultExceptionMiddleware>();
            return services;
        }

        /// <summary>
        /// Adds the <see cref="OperationResultExceptionMiddleware"/>  type to the application's request pipeline.
        /// <para></para>
        /// <para>Make sure to register the <see cref="OperationResultExceptionMiddleware"/> using the <see cref="AddXOperationResultExceptionMiddleware(IXpandableServiceBuilder)"/> method.</para>
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IXpandableApplicationBuilder UseXOperationResultExceptionMiddleware(this IXpandableApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Builder.UseMiddleware<OperationResultExceptionMiddleware>();

            return builder;
        }

        /// <summary>
        /// Adds the HTTP context header values accessor that implements the <see cref="IHttpHeaderAccessor"/>.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXHttpHeaderAccessor(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IHttpHeaderAccessor, HttpHeaderAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IRazorViewRenderer"/> implementation.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IXpandableServiceBuilder AddXRazorViewRenderer(this IXpandableServiceBuilder services)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            services.Services.AddScoped<IRazorViewRenderer, RazorViewRenderer>();
            services.Services.AddSingleton<IRazorModelViewCollection, RazorModelViewCollection>();
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
        public static IXpandableApplicationBuilder UseXServiceExport(this IXpandableApplicationBuilder application, IWebHostEnvironment environment)
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
        public static IXpandableApplicationBuilder UseXServiceExport(
            this IXpandableApplicationBuilder application, IWebHostEnvironment environment, Action<ExportServiceOptions> configureOptions)
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
            this IXpandableApplicationBuilder application, IWebHostEnvironment environment, ExportServiceOptions options)
        {
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
                    export.UseServices(application.Builder, environment);
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

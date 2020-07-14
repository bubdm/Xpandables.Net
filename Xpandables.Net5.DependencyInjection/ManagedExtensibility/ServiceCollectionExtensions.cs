
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

using System;
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
        const string ExportServiceRegisterAssemblyName = "Xpandables.Net5.ManagedExtensibility.dll";
        const string ExportServiceRegisterName = "ExportServiceRegister";
        const string AddServiceMethodName = "AddServiceExport";

        /// <summary>
        /// Adds and configures registration of services using the <see cref="IAddServiceExport"/> implementations found in the current application path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IServiceCollection AddXServiceExport(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddXServiceExport(_ => { });
            return services;
        }

        /// <summary>
        /// Adds and configures registration of services using the<see cref="IAddServiceExport"/> implementations found in the path.
        /// This method is used with MEF : Managed Extensibility Framework.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ExportServiceOptions"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configureOptions"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public static IServiceCollection AddXServiceExport(
            this IServiceCollection services, Action<ExportServiceOptions> configureOptions)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var definedOptions = new ExportServiceOptions();
            configureOptions.Invoke(definedOptions);

            if (ExportServiceRegisterAssemblyName.TryLoadAssembly(out var assembly, out var exception))
            {
                var exportServiceRegister = assembly
                    .GetExportedTypes()
                    .First(type => type.Name.Equals(ExportServiceRegisterName, StringComparison.InvariantCulture));

                if (exportServiceRegister
                    .TryTypeInvokeMember(
                        out _,
                        out var ex,
                        AddServiceMethodName,
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod,
                        default,
                        exportServiceRegister,
                        new object[] { services, definedOptions }))
                    return services;

                throw new InvalidOperationException($"{ExportServiceRegisterName} execution failed.", ex);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Type not found : {ExportServiceRegisterName}. Add reference to {ExportServiceRegisterAssemblyName}",
                    exception);
            }
        }
    }
}

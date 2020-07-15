
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

#pragma warning disable ET002 // Namespace does not match file path or default namespace
namespace Xpandables.Net5.ManagedExtensibility
#pragma warning restore ET002 // Namespace does not match file path or default namespace
{
    /// <summary>
    /// Class helper to implement <see cref="IUseServiceExportExtended"/>.
    /// The implementation class must be decorated with the attribute <see langword="Export(typeof(IAddServiceExport))"/>
    /// with the type of <see cref="IAddServiceExport"/> as contract type.
    /// </summary>
    public abstract class UserServiceExportExtended : IUseServiceExportExtended
    {
        /// <summary>
        /// When implemented, this method should configure application services.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to act on.</param>
        /// <param name="webHostEnvironment">The web hosting environment instance.</param>
        public abstract void UseServices(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment);

        /// <summary>
        /// When implemented, this method should configure application services.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to act on.</param>
        /// <param name="webHostEnvironment">The web hosting environment instance.</param>
        public void UseServices(object applicationBuilder, object webHostEnvironment)
            => UseServices((IApplicationBuilder)applicationBuilder, (IWebHostEnvironment)webHostEnvironment);
    }
}

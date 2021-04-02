
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

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Specifies the name of the tenant.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataContextTenantAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataContextTenantAttribute"/> with the tenant name.
        /// </summary>
        /// <param name="tenantName">The tenant name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="tenantName"/> is null.</exception>
        public DataContextTenantAttribute(string tenantName)
        {
            TenantName = tenantName ?? throw new ArgumentNullException(nameof(tenantName));
        }

        /// <summary>
        /// Gets the tenant name.
        /// </summary>
        public string TenantName { get; }
    }
}

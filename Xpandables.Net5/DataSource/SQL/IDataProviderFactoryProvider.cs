
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
using System.Data.Common;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Allows an application author to return a data provider factory from the specified provider type.
    /// A default implementation is <see cref="DataProviderFactoryProvider"/>.
    /// </summary>
    public interface IDataProviderFactoryProvider : IServiceProvider
    {
        /// <summary>
        /// Returns an instance of the data provider factory matching the specified provider type.
        /// </summary>
        /// <param name="providerType">The provider type to find factory.</param>
        /// <returns>An instance of <see cref="DbProviderFactory"/> if found, otherwise an empty or exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        DbProviderFactory? GetProviderFactory(DataProviderType providerType);
    }
}

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
using System.Threading.Tasks;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Allows an application author to return a <see cref="DataConnectionContext"/> to be used with <see cref="IDataBase"/>.
    /// Note that you must register this interface as a singleton to take advantage of the caching process.
    /// </summary>
    public interface IDataConnectionContextProvider
    {
        /// <summary>
        /// Provides with a database connection using the provider and the connection string.
        /// </summary>
        /// <param name="dataConnection">The data connection to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataConnection"/> is null.</exception>
        Task<DataConnectionContext> GetDataConnectionContextAsync(IDataConnection dataConnection);
    }
}

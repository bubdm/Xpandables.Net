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

using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.EntityFramework
{
    /// <summary>
    /// Provides with a method to return the ambient data context instance.
    /// You need to register the class implementation to the services collection using and extension method.
    /// </summary>
    public interface IDataContextProvider
    {
        /// <summary>
        /// Asynchronously returns an instance that will contain the ambient data context according to the environment.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An instance of context that implements <see cref="IDataContext" />.</returns>
        Task<IDataContext> GetDataContextAsync(CancellationToken cancellationToken = default);
    }
}
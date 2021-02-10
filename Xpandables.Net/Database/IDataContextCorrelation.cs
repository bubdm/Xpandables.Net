
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
    /// Allows use of multiple data context that need to be shared across asynchronous control flows.
    /// </summary>
    public interface IDataContextCorrelation
    {
        /// <summary>
        /// Gets the name of the ambient data context.
        /// </summary>
        string? CurrentDataContextName { get; }

        /// <summary>
        /// Returns an instance of the ambient data context.
        /// </summary>
        /// <returns><see cref="IDataContext"/> derived class.</returns>
        IDataContext GetDataContext();

        /// <summary>
        /// Sets the ambient data context name.
        /// </summary>
        /// <typeparam name="TDataContext">The type of the data context.</typeparam>
        void SetCurrentDataContextName<TDataContext>()
            where TDataContext : class, IDataContext;

        /// <summary>
        /// Sets the ambient data context name.
        /// </summary>
        /// <param name="name">The name of the data context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        void SetCurrentDataContextName(string name);
    }
}

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

using Xpandables.Net.Correlations;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Represents a method signature to be used to handle data context factory using <see cref="CorrelationCollection{TKey, TValue}"/>.
    /// </summary>
    /// <param name="serviceProvider">The active service provider.</param>
    /// <param name="correlationCollection">The target correlation context that contains data context information.</param>
    /// <returns>An implementation derived type from <see cref="IDataContext"/>.</returns>
    public delegate IDataContext DataContextFactory(IServiceProvider serviceProvider, CorrelationCollection<string, string> correlationCollection);

    /// <summary>
    /// Provides with method for creating derived <see cref="IDataContext"/> instances.
    /// </summary>
    public interface IDataContextFactory
    {
        /// <summary>
        /// Returns the ambient instance of a derived <see cref="IDataContext"/>.
        /// </summary>
        /// <returns>An new derived of <see cref="IDataContext"/> instance.</returns>
        IDataContext GetDataContext();
    }
}

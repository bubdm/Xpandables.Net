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
    /// Provides with method for creating derived <see cref="IDataContext"/> in multi-tenancy.
    /// </summary>
    public interface IDataContextTenant
    {
        /// <summary>
        /// The factory used to retrieve an implementation of <see cref="IDataContext"/>.
        /// </summary>
        Func<IDataContext> Factory { get; }

        /// <summary>
        /// Gets the unique identifier for the tenant.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Provides with method for creating <typeparamref name="TDataContext"/> in multi-tenancy.
    /// </summary>
    /// <typeparam name="TDataContext">The type of the data context.</typeparam>
    public interface IDataContextTenant<out TDataContext> : IDataContextTenant
        where TDataContext : class, IDataContext
    {
        /// <summary>
        /// The factory used to retrieve an instance of <typeparamref name="TDataContext"/>.
        /// </summary>
        new Func<TDataContext> Factory { get; }
    }
}

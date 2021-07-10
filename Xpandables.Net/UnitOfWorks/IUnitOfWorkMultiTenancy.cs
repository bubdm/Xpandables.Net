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

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Provides with method for creating derived <see cref="IUnitOfWork"/> in multi-tenancy.
    /// </summary>
    public interface IUnitOfWorkMultiTenancy
    {
        /// <summary>
        /// The factory used to retrieve an implementation of <see cref="IUnitOfWork"/>.
        /// </summary>
        Func<IUnitOfWork> Factory { get; }

        /// <summary>
        /// Gets the unique identifier for the tenant.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Provides with method for creating <typeparamref name="TUnitOfWork"/> in multi-tenancy.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The type of the data context.</typeparam>
    public interface IUnitOfWorkMultiTenancy<out TUnitOfWork> : IUnitOfWorkMultiTenancy
        where TUnitOfWork : class, IUnitOfWork
    {
        /// <summary>
        /// The factory used to retrieve an instance of <typeparamref name="TUnitOfWork"/>.
        /// </summary>
        new Func<TUnitOfWork> Factory { get; }
    }
}

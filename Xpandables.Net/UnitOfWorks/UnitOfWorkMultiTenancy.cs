
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
    /// Implementation of <see cref="IUnitOfWorkMultiTenancy{TUnitOfWork}"/> for multi-tenancy.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The type of unit of work.</typeparam>
    public sealed class UnitOfWorkMultiTenancy<TUnitOfWork> : IUnitOfWorkMultiTenancy<TUnitOfWork>
        where TUnitOfWork : class, IUnitOfWork
    {
        ///<inheritdoc/>
        public Func<TUnitOfWork> Factory { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UnitOfWorkMultiTenancy{TUnitOfWork}"/> class with the factory of the target type.
        /// The unique identifier of the tenant will be the name of the type.
        /// </summary>
        /// <param name="factory">The factory for <typeparamref name="TUnitOfWork"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="factory"/> is null.</exception>
        public UnitOfWorkMultiTenancy(Func<TUnitOfWork> factory)
            : this(typeof(TUnitOfWork).Name, factory) { }

        /// <summary>
        /// Initializes a new instance of <see cref="UnitOfWorkMultiTenancy{TUnitOfWork}"/> class with the factory of the target type.
        /// </summary>
        /// <param name="name">The unique identifier of the tenant.</param>
        /// <param name="factory">The factory for <typeparamref name="TUnitOfWork"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        public UnitOfWorkMultiTenancy(string name, Func<TUnitOfWork> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        ///<inheritdoc/>
        public string Name { get; }

        Func<IUnitOfWork> IUnitOfWorkMultiTenancy.Factory => () => Factory();
    }
}

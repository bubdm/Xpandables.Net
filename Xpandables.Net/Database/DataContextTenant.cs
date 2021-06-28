
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
    /// Implementation of <see cref="IDataContextTenant{TDataContext}"/> for multi-tenancy.
    /// </summary>
    /// <typeparam name="TDataContext">The type of data context.</typeparam>
    public sealed class DataContextTenant<TDataContext> : IDataContextTenant<TDataContext>
        where TDataContext : class, IDataContext
    {
        ///<inheritdoc/>
        public Func<TDataContext> Factory { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextTenant{TDataContext}"/> class with the factory of the target type.
        /// The unique identifier of the tenant will be the name of the type.
        /// </summary>
        /// <param name="factory">The factory for <typeparamref name="TDataContext"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="factory"/> is null.</exception>
        public DataContextTenant(Func<TDataContext> factory)
            : this(typeof(TDataContext).Name, factory) { }

        /// <summary>
        /// Initializes a new instance of <see cref="DataContextTenant{TDataContext}"/> class with the factory of the target type.
        /// </summary>
        /// <param name="name">The unique identifier of the tenant.</param>
        /// <param name="factory">The factory for <typeparamref name="TDataContext"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="factory"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        public DataContextTenant(string name, Func<TDataContext> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        ///<inheritdoc/>
        public string Name { get; }

        Func<IDataContext> IDataContextTenant.Factory => () => Factory();
    }
}


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
using System.Collections.Generic;
using System.Data.Common;

namespace Xpandables.Net.Data.Connections
{
    /// <summary>
    /// Contains information of <see cref="DbConnection"/> and <see cref="DbProviderFactory"/>
    /// </summary>
    public sealed class DataConnectionContext : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataConnectionContext"/>.
        /// </summary>
        /// <param name="dbConnection">The connection to the data base.</param>
        /// <param name="dbProviderFactory">The data base provider factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbConnection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dbProviderFactory"/> is null.</exception>
        public DataConnectionContext(DbConnection dbConnection, DbProviderFactory dbProviderFactory)
        {
            DbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            DbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
        }

        /// <summary>
        /// Makes a copy of the current instance.
        /// </summary>
        /// <param name="source">The data connection to be copied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public DataConnectionContext(DataConnectionContext source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            DbConnection = source.DbConnection;
            DbProviderFactory = source.DbProviderFactory;
        }

        /// <summary>
        /// Gets the connection to the data base.
        /// </summary>
        public DbConnection DbConnection { get; }

        /// <summary>
        /// Gets the data base provider factory.
        /// </summary>
        public DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DbConnection;
            yield return DbProviderFactory;
        }
    }
}

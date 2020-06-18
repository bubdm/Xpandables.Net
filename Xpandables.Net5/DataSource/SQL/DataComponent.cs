
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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace System.Design.DataSource.SQL
{
    /// <summary>
    /// Contains component execution for <see cref="DataExecutable{T}"/>.
    /// </summary>
    public sealed class DataComponent : ValueObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataComponent"/>.
        /// </summary>
        /// <param name="command">The database command instance.</param>
        /// <param name="adapter">The database adapter to act with.</param>
        /// <param name="commandType">the command type</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="adapter"/> is null.</exception>
        public DataComponent(DbCommand command, DbDataAdapter adapter, CommandType commandType)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            CommandType = commandType;
        }

        /// <summary>
        /// Get the Database command instance.
        /// </summary>
        public DbCommand Command { get; }

        /// <summary>
        /// Gets the database adapter instance.
        /// </summary>
        public DbDataAdapter Adapter { get; }

        /// <summary>
        /// Gets the command type.
        /// </summary>
        internal CommandType CommandType { get; }

        /// <summary>
        /// When implemented in derived class, this method will provide the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Command;
            yield return Adapter;
        }
    }
}

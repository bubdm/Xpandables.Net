
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
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;

using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Defines the context of an executable.
    /// </summary>
    public sealed class DataExecutableContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableContext"/> class with the argument and component.
        /// </summary>
        /// <param name="argument">The context argument.</param>
        /// <param name="component">The component argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="component"/> is null.</exception>
        public DataExecutableContext(DataArgument argument, DataComponent component)
        {
            Argument = argument ?? throw new ArgumentNullException(nameof(argument));
            Component = component ?? throw new ArgumentNullException(nameof(component));
        }

        /// <summary>
        /// Gets the argument for the executable.
        /// </summary>
        public DataArgument Argument { get; }
        
        /// <summary>
        /// Gets the component needed by the executable.
        /// </summary>
        public DataComponent Component { get; }

        /// <summary>
        /// Contains the argument execution for <see cref="IDataExecutable{T}"/>.
        /// </summary>
#pragma warning disable CA1034 // Nested types should not be visible
        public sealed class DataArgument
#pragma warning restore CA1034 // Nested types should not be visible
        {
            /// <summary>
            /// Initializes a new instance of <see cref="DataArgument"/>.
            /// </summary>
            /// <param name="options">The execution options.</param>
            /// <param name="commandText">The string command to act with.</param>
            /// <param name="parameters">The parameters for the command.</param>
            /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
            /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
            public DataArgument(DataOptions options, string commandText, object[]? parameters = default)
            {
                Options = options ?? throw new ArgumentNullException(nameof(options));
                CommandText = commandText ?? throw new ArgumentNullException(nameof(commandText));
                Parameters = parameters?.Any() == true ? new ReadOnlyCollection<object>(parameters) : default;
            }

            /// <summary>
            /// Get the execution options.
            /// </summary>
            public DataOptions Options { get; }

            /// <summary>
            /// Gets the string command : can be stored procedure name, query or other command.
            /// </summary>
            public string CommandText { get; }

            /// <summary>
            /// Gets the parameters to be used with the <see cref="IDataExecutable{T}"/>.
            /// </summary>
            public ReadOnlyCollection<object>? Parameters { get; }
        }

        /// <summary>
        /// Contains component execution for <see cref="IDataExecutable{T}"/>.
        /// </summary>
#pragma warning disable CA1034 // Nested types should not be visible
        public sealed class DataComponent : ValueObject
#pragma warning restore CA1034 // Nested types should not be visible
        {
            /// <summary>
            /// Initializes a new instance of <see cref="DataComponent"/>.
            /// </summary>
            /// <param name="command">The database command instance.</param>
            /// <param name="adapter">The database adapter to act with.</param>
            /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
            /// <exception cref="ArgumentNullException">The <paramref name="adapter"/> is null.</exception>
            public DataComponent(DbCommand command, DbDataAdapter adapter)
            {
                Command = command ?? throw new ArgumentNullException(nameof(command));
                Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
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
}

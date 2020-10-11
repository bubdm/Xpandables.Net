﻿
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
using System.Collections.ObjectModel;
using System.Linq;

using Xpandables.Net.Data.Options;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Contains the argument execution for <see cref="DataExecutable{T}"/>.
    /// </summary>
    public sealed class DataExecutableArgument
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataExecutableArgument"/>.
        /// </summary>
        /// <param name="options">The execution options.</param>
        /// <param name="commandText">The string command to act with.</param>
        /// <param name="parameters">The parameters for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText"/> is null.</exception>
        public DataExecutableArgument(IDataExecutableOptions options, string commandText, object[]? parameters = default)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            CommandText = commandText ?? throw new ArgumentNullException(nameof(commandText));
            Parameters = parameters?.Any() == true ? new ReadOnlyCollection<object>(parameters) : default;
        }

        /// <summary>
        /// Get the execution options.
        /// </summary>
        public IDataExecutableOptions Options { get; }

        /// <summary>
        /// Gets the string command : can be stored procedure name, query or other command.
        /// </summary>
        public string CommandText { get; }

        /// <summary>
        /// Gets the parameters to be used with the <see cref="DataExecutable{T}"/>.
        /// </summary>
        public ReadOnlyCollection<object>? Parameters { get; }
    }
}


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
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Data.Executables;

namespace Xpandables.Net.Data
{
    /// <summary>
    /// Provides with a method to execute command to a database using an implementation of <see cref="IDataExecutable{TResult}"/> interface.
    /// </summary>
    public interface IDataBase
    {
        /// <summary>
        /// Executes a command/query with the specified executable <typeparamref name="TExecutable" /> type
        /// and returns a result of <typeparamref name="TResult" /> type.
        /// The <typeparamref name="TExecutable" /> type must implement <see cref="IDataExecutable{T}" /> interface.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TExecutable">The type of the executable. The class must implement <see cref="IDataExecutable{T}" /> interface.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="commandText">The query or store procedure name.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options" /> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandText" /> is null.</exception>
        /// <exception cref="InvalidOperationException">the execution failed. See inner exception.</exception>
        Task<TResult> ExecuteAsync<TResult, TExecutable>(
            DataOptions options,
            string commandText,
            CommandType commandType,
            CancellationToken cancellationToken,
            params object[] parameters)
            where TExecutable : class, IDataExecutable<TResult>;
    }
}

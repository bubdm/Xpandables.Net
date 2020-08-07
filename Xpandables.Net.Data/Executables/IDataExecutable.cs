
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
using System.Threading.Tasks;

using Xpandables.Net.Optionals;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Allows an application author to defines an executable process for <see cref="IDataBase"/>.
    /// </summary>
    public interface IDataExecutable
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        Task<Optional<object>> ExecuteAsync(DataExecutableContext context);
    }

    /// <summary>
    /// Allows an application author to defines an executable process for <see cref="IDataBase"/> for a specific-type result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IDataExecutable<TResult> : IDataExecutable
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        new Task<Optional<TResult>> ExecuteAsync(DataExecutableContext context);
    }

    /// <summary>
    /// Defines the abstract implementation of <see cref="IDataExecutable{TResult}"/> class.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class DataExecutable<TResult> : IDataExecutable<TResult>
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns a result of specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <returns>A task representing the asynchronous operation</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public abstract Task<Optional<TResult>> ExecuteAsync(DataExecutableContext context);

        async Task<Optional<object>> IDataExecutable.ExecuteAsync(DataExecutableContext context)
            => await ExecuteAsync(context).ConfigureAwait(false);
    }
}


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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Extensions;

namespace Xpandables.Net.Data.Executables
{
    /// <summary>
    /// Allows an application author to defines an executable process to map database result.
    /// </summary>
    public interface IDataExecutableMapper
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns the result of objects.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An asynchronous enumeration of objects.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public async IAsyncEnumerable<object> ExecuteMappedAsync(
            DataExecutableContext context, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            yield break;
        }
    }

    /// <summary>
    /// Allows an application author to defines an executable process to map database result to a specific-type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IDataExecutableMapper<TResult> : IDataExecutableMapper
    {
        /// <summary>
        /// Asynchronously executes an action to the database and returns the result mapped to the specific-type.
        /// </summary>
        /// <param name="context">The target executable context instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An asynchronous enumeration of <typeparamref name="TResult"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public new async IAsyncEnumerable<TResult> ExecuteMappedAsync(
            DataExecutableContext context, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            yield break;
        }

        async IAsyncEnumerable<object> IDataExecutableMapper.ExecuteMappedAsync(
            DataExecutableContext context, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var result in ExecuteMappedAsync(context, cancellationToken))
                yield return result!;
        }
    }
}

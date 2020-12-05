﻿/************************************************************************************************************
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Represents a wrapper interface that avoids use of C# dynamics with query pattern and allows type inference for <see cref="IAsyncQueryHandler{TQuery, TResult}"/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public interface IAsyncQueryHandlerWrapper<TResult> : ICanHandle
    {
        /// <summary>
        /// Asynchronously handles the specified query and returns the task result.
        /// </summary>
        /// <param name="query">The query to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IResultState{TValue}"/>.</returns>
        Task<IResultState<TResult>> HandleAsync(IAsyncQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}
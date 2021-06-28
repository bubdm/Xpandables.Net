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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Queries
{
    /// <summary>
    /// Represents a helper class that allows implementation of the <see cref="IQueryHandler{TQuery, TResult}"/> interface.
    /// </summary>
    /// <typeparam name="TQuery">Type of argument to act on.</typeparam>
    /// <typeparam name="TResult">Type of result.</typeparam>
    public abstract class QueryHandler<TQuery, TResult> : OperationResults, IQueryHandler<TQuery, TResult>
        where TQuery : class, IQuery<TResult>
    {
        ///<inheritdoc/>
        public abstract Task<IOperationResult<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
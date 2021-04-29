
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Entities;

namespace Xpandables.Net.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAggregateRoot"></typeparam>
    public interface IAggregateRootAccessor<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IOperationResult<TAggregateRoot>> ReadAsync(Guid aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IOperationResult> AppendAsync(TAggregateRoot aggregate, CancellationToken cancellationToken = default);
    }
}

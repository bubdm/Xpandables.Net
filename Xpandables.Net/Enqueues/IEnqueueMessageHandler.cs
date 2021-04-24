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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Enqueues
{
    /// <summary>
    /// Provides with a method to asynchronously enqueue a message of specific type that implements <see cref="IQueueMessage"/> interface.
    /// </summary>
    public interface IEnqueueMessageHandler
    {
        /// <summary>
        /// Asynchronously enqueues the specified message.
        /// </summary>
        /// <param name="queueMessage">The queue message instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="queueMessage"/> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        Task<IOperationResult> EnqueueAsync<TQueueMessage>(TQueueMessage queueMessage, CancellationToken cancellationToken = default)
            where TQueueMessage : class, IQueueMessage;
    }
}
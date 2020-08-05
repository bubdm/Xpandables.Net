
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
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Xpandables.Net.Extensions
{
    /// <summary>
    /// Provides with extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Asynchronously executes a <see langword="foreach"/> (For Each in Visual Basic) operation with thread-local data on an
        /// <see cref="IAsyncEnumerable{T}"/> in which iterations may run in parallel and the state of the loop can be monitored and manipulated.
        /// </summary>
        /// <typeparam name="TSource">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="actionBody">The action to invoke with each data element received.</param>
        /// <param name="maxDegreeOfParallelism">the maximum number of messages that may be processed by the block concurrently.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to use for scheduling tasks.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public static async Task ParallelForEachAsync<TSource>(
            this IAsyncEnumerable<TSource> source, Func<TSource, Task> actionBody, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler? scheduler = default, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = actionBody ?? throw new ArgumentNullException(nameof(actionBody));

            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = cancellationToken
            };

            if (scheduler != null)
                options.TaskScheduler = scheduler;

            var block = new ActionBlock<TSource>(actionBody, options);
            await foreach (var item in source)
                await block.SendAsync(item).ConfigureAwait(false);

            block.Complete();
            await block.Completion.ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a <see langword="foreach"/> (For Each in Visual Basic) operation with thread-local data on an
        /// <see cref="IAsyncEnumerable{T}"/> in which iterations may run in parallel and the state of the loop can be monitored and manipulated.
        /// </summary>
        /// <typeparam name="TSource">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="actionBody">The action to invoke with each data element received.</param>
        /// <param name="maxDegreeOfParallelism">the maximum number of messages that may be processed by the block concurrently.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to use for scheduling tasks.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public static async Task ParallelForEachAsync<TSource>(
            this IAsyncEnumerable<TSource> source, Action<TSource> actionBody, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler? scheduler = default, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = actionBody ?? throw new ArgumentNullException(nameof(actionBody));

            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = cancellationToken
            };

            if (scheduler != null)
                options.TaskScheduler = scheduler;

            var block = new ActionBlock<TSource>(actionBody, options);
            await foreach (var item in source)
                await block.SendAsync(item).ConfigureAwait(false);

            block.Complete();
            await block.Completion.ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously executes a <see langword="foreach"/> (For Each in Visual Basic) operation with thread-local data on an
        /// <see cref="IAsyncEnumerable{T}"/> in which iterations may run in parallel and the state of the loop can be monitored and manipulated.
        /// </summary>
        /// <typeparam name="TSource">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="actionBlock">The action to invoke with each data element received.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        public static async Task ParallelForEachAsync<TSource>(
            this IAsyncEnumerable<TSource> source, ActionBlock<TSource> actionBlock, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = actionBlock ?? throw new ArgumentNullException(nameof(actionBlock));

            await foreach (var item in source)
                await actionBlock.SendAsync(item, cancellationToken).ConfigureAwait(false);

            actionBlock.Complete();
            await actionBlock.Completion.ConfigureAwait(false);
        }
    }
}

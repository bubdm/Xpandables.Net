
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
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Optionals;

namespace Xpandables.Net.Asynchronous
{
    /// <summary>
    /// Provides with methods used to execute asynchronous operation synchronously.
    /// </summary>
    public static class AsyncExtensions
    {
        // Defines the static task factory.
        private static readonly TaskFactory _taskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        /// <summary>
        /// Executes the target asynchronous operation synchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="func">The asynchronous function to execute synchronously.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="func"/> is null.</exception>
        public static TResult RunSync<TResult>(this Func<Task<TResult>> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            return _taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the target asynchronous operation synchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="task">The asynchronous function to execute synchronously.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="task"/> is null.</exception>
        public static TResult RunSync<TResult>(this Task<TResult> task)
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            return _taskFactory.StartNew(() => task).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the target asynchronous operation synchronously.
        /// </summary>
        /// <param name="task">The operation to be synchronously executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="task"/> is null.</exception>
        public static void RunSync(this Task task)
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            _taskFactory.StartNew(() => task).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously enumerates the values in a safety mode. Any exception is handled by the <paramref name="onException"/> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncEnumerable">The asynchronous collection to act on.</param>
        /// <param name="onException">The delegate that get called when exception.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An object that contains an enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable in safety mode.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="asyncEnumerable"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="onException"/> is null.</exception>
        public static async IAsyncEnumerable<TResult> AsyncExecuteSafe<TResult>(this IAsyncEnumerable<TResult> asyncEnumerable, Action<ExceptionDispatchInfo> onException, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _ = asyncEnumerable ?? throw new ArgumentNullException(nameof(asyncEnumerable));
            _ = onException ?? throw new ArgumentNullException(nameof(onException));

            await using var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await asyncEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    resultExist = false;
                    onException(ExceptionDispatchInfo.Capture(exception));
                }

                if (resultExist)
                    yield return asyncEnumerator.Current;
                else
                    yield break;
            }
        }

        /// <summary>
        /// Asynchronously enumerates the values in a safety mode. Any exception is handled by the <paramref name="onException"/> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncEnumerable">The asynchronous collection to act on.</param>
        /// <param name="onException">The delegate that get called when exception.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>An object that contains an enumerator of <typeparamref name="TResult"/> that can be asynchronously enumerable in safety mode.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="asyncEnumerable"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="onException"/> is null.</exception>
        public static async IAsyncEnumerable<TResult> AsyncExecuteSafe<TResult>(this IAsyncEnumerable<TResult> asyncEnumerable, Func<ExceptionDispatchInfo, Task> onException, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _ = asyncEnumerable ?? throw new ArgumentNullException(nameof(asyncEnumerable));
            _ = onException ?? throw new ArgumentNullException(nameof(onException));

            await using var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
            for (var resultExist = true; resultExist;)
            {
                try
                {
                    resultExist = await asyncEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    resultExist = false;
                    await onException(ExceptionDispatchInfo.Capture(exception)).ConfigureAwait(false);
                }

                if (resultExist)
                    yield return asyncEnumerator.Current;
                else
                    yield break;
            }
        }

        /// <summary>
        /// Asynchronously executes the task is a safety mode. Any exception is handled by the <paramref name="onException"/> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskAsync">The tack to be invoked.</param>
        /// <param name="onException">The delegate that get called when exception.</param>
        /// <returns>A task that represents an optional object that may contains a value of <typeparamref name="TResult"/> or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="taskAsync"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="onException"/> is null.</exception>
        public static async Task<Optional<TResult>> AsyncExecuteSafe<TResult>(this Task<TResult> taskAsync, Action<ExceptionDispatchInfo> onException)
        {
            _ = taskAsync ?? throw new ArgumentNullException(nameof(taskAsync));
            _ = onException ?? throw new ArgumentNullException(nameof(onException));

            try
            {
                return await taskAsync.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                onException(ExceptionDispatchInfo.Capture(exception));
                return Optional<TResult>.Empty();
            }
        }

        /// <summary>
        /// Asynchronously executes the task is a safety mode. Any exception is handled by the <paramref name="onException"/> method.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskAsync">The tack to be invoked.</param>
        /// <param name="onException">The delegate that get called when exception.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="taskAsync"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="onException"/> is null.</exception>
        public static async Task AsyncExecuteSafe<TResult>(this Task taskAsync, Action<ExceptionDispatchInfo> onException)
        {
            _ = taskAsync ?? throw new ArgumentNullException(nameof(taskAsync));
            _ = onException ?? throw new ArgumentNullException(nameof(onException));

            try
            {
                await taskAsync.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                onException(ExceptionDispatchInfo.Capture(exception));
            }
        }

        /// <summary>
        /// Executes the action is a safety mode. Any exception is handled by the <paramref name="onException"/> method.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        /// <param name="onException">The delegate that get called when exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="onException"/> is null.</exception>
        public static void ExecuteSafe(this Action action, Action<ExceptionDispatchInfo> onException)
        {
            _ = action ?? throw new ArgumentNullException(nameof(action));
            _ = onException ?? throw new ArgumentNullException(nameof(onException));

            try
            {
                action();
            }
            catch (Exception exception)
            {
                onException(ExceptionDispatchInfo.Capture(exception));
            }
        }
    }
}

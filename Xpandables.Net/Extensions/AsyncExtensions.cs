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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Extensions
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
        /// Executes an <see cref="IAsyncEnumerable{T}"/> in a try catch scope.
        /// </summary>
        /// <typeparam name="TResult">The type of the result, may be <see cref="IAsyncEnumerable{T}"/>.</typeparam>
        /// <param name="func">The function to be executed in try catch.</param>
        /// <param name="exceptionDispatchInfo">The handled exception.</param>
        /// <returns>A result of <typeparamref name="TResult"/> type or null if exception..</returns>
        [return: MaybeNull]
        public static TResult TryCatchAsyncEnumerable<TResult>(this Func<TResult> func, out ExceptionDispatchInfo? exceptionDispatchInfo)
        {
            exceptionDispatchInfo = default;
            try
            {
                return func();
            }
            catch (Exception exception)
            {
                exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                return default;
            }
        }
    }
}

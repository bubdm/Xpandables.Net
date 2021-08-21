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

namespace Xpandables.Net;

/// <summary>
/// Provides with methods used to execute asynchronous operation synchronously.
/// </summary>
public static partial class AsyncEnumerableExtensions
{
    // Defines the static task factory.
    internal static readonly TaskFactory TaskFactory = new(
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
    /// <returns>An object of <typeparamref name="TResult"/> type.</returns>
    public static TResult RunSync<TResult>(this Func<Task<TResult>> func)
    {
        _ = func ?? throw new ArgumentNullException(nameof(func));
        return TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes the target asynchronous operation synchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of result.</typeparam>
    /// <param name="task">The asynchronous function to execute synchronously.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="task"/> is null.</exception>
    /// <returns>An object of <typeparamref name="TResult"/> type.</returns>
    public static TResult RunSync<TResult>(this Task<TResult> task)
    {
        _ = task ?? throw new ArgumentNullException(nameof(task));
        return TaskFactory.StartNew(() => task).Unwrap().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes the target asynchronous operation synchronously.
    /// </summary>
    /// <param name="task">The operation to be synchronously executed.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="task"/> is null.</exception>
    public static void RunSync(this Task task)
    {
        _ = task ?? throw new ArgumentNullException(nameof(task));
        TaskFactory.StartNew(() => task).Unwrap().GetAwaiter().GetResult();
    }
}

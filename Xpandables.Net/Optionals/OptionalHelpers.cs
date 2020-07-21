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
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Xpandables.Net.Optionals
{
    /// <summary>
    /// Functionalities for optional pattern methods.
    /// </summary>
    public static class OptionalHelpers
    {
        /// <summary>
        /// Converts the specified value to an optional instance.
        /// </summary>
        /// <typeparam name="T">The Type of the value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>An optional instance.</returns>
        public static Optional<T> AsOptional<T>([AllowNull] this T value)
            => value is { } ? Optional<T>.Some(value) : Optional<T>.Empty();

        /// <summary>
        /// Asynchronously creates a new optional that is the result of applying the given functions to the element.
        /// The some delegate get called only if the instance contains a value,
        /// otherwise returns the empty delegate.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="pattern">The pattern to be used.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="pattern"/> is null.</exception>
        public static async Task<Optional<TOutput>> MapAsync<T, TOutput>(
            this Task<Optional<T>> optional, (Func<Task<TOutput>> empty, Func<T, Task<TOutput>> some) pattern)
        {
            _ = optional ?? throw new ArgumentNullException(nameof(optional));

            return await (await optional.ConfigureAwait(false)).MapAsync(pattern).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously turns the current optional to a new optional one using the specified binding.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="binder">The binding function.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binder"/> is null.</exception>
        public static async Task<Optional<TOutput>> BindAsync<T, TOutput>(this Task<Optional<T>> optional, Func<T, Task<Optional<TOutput>>> binder)
        {
            _ = optional ?? throw new ArgumentNullException(nameof(optional));
            _ = binder ?? throw new ArgumentNullException(nameof(binder));

            return await (await optional.ConfigureAwait(false)).BindAsync(binder).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a new optional that is the result of calling the given function.
        /// The delegate get called only if the instance contains a value, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="some">The function to call.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public static async Task<Optional<T>> MapAsync<T>(this Task<Optional<T>> optional, Func<T, Task<T>> some)
        {
            _ = optional ?? throw new ArgumentNullException(nameof(optional));
            _ = some ?? throw new ArgumentNullException(nameof(some));

            return await (await optional.ConfigureAwait(false)).MapAsync(some).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <typeparam name="T">The type of the optional value.</typeparam>
        /// <param name="optional">The optional to act on.</param>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public static async Task<Optional<T>> WhenEmptyAsync<T>(this Task<Optional<T>> optional, Func<Task<T>> empty)
        {
            _ = optional ?? throw new ArgumentNullException(nameof(optional));
            _ = empty ?? throw new ArgumentNullException(nameof(empty));

            return await (await optional.ConfigureAwait(false)).WhenEmptyAsync(empty).ConfigureAwait(false);
        }
    }
}

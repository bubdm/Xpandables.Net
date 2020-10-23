
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Asynchronous;
using Xpandables.Net.Types;

namespace Xpandables.Net.Optionals
{
    /// <summary>
    /// Describes an object that can contain a value or not of a specific type.
    /// You can make unconditional calls to its content using <see cref="System.Linq"/> without testing whether the content is there or not.
    /// The enumerator will only return the available value.
    /// If <typeparamref name="T"/> is an enumerable, use the <see cref="GetEnumerable"/> function to access its content.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public readonly partial struct Optional<T> : IEnumerable<T>, IAsyncEnumerable<T>
    {
        private readonly Type[] _genericTypes;
        private readonly T[] _values;
#pragma warning disable CA1822 // Mark members as static
        private readonly bool IsEnumerbaleOrAsyncEnumerable => typeof(T).IsEnumerable() || typeof(T).IsAsyncEnumerable();
        private readonly bool IsEnumerbale => typeof(T).IsEnumerable();       
#pragma warning restore CA1822 // Mark members as static

        private static readonly MethodInfo _arrayEmpty = typeof(Array).GetMethod("Empty")!;
        private static readonly MethodInfo _asyncEmpty = typeof(AsyncEnumerableExtensions).GetMethod("Empty")!;

        internal bool IsValue() => _values?.Length > 0;

        private Optional(T[]? values = default)
        {
            _values = values ?? Array.Empty<T>();
            _genericTypes = typeof(T).IsEnumerable() || typeof(T).IsAsyncEnumerable() ? typeof(T).GetGenericArguments() : Type.EmptyTypes;
        }

        private T GetDefaultEnumerable()
        {
            var runtimeMethod = IsEnumerbale ? _arrayEmpty.MakeGenericMethod(_genericTypes[0]) : _asyncEmpty.MakeGenericMethod(_genericTypes[0]);
            return (T)runtimeMethod.Invoke(null, null)!;
        }

        /// <summary>
        /// Returns the available enumerable collection value when <typeparamref name="T"/> is an enumerable.
        /// If enumerable value is null, it'll return an empty enumerable.
        /// Otherwise, its will throw an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <typeparamref name="T"/> is not an enumerable.</exception>
        public T GetEnumerable()
        {
            if (!ValueIsEnumerable())
                throw new InvalidOperationException($"{typeof(T).Name} is not an enumerable nor asynchronous enumerable !");

            return IsValue() ? _values[0] : GetDefaultEnumerable();
        }

        /// <summary>
        /// Gets a state whether the internal value is an enumerable or asynchronous enumerable.
        /// </summary>
        /// <returns><see langword="true"/> if so, otherwise <see langword="false"/>.</returns>
        public bool ValueIsEnumerable() => IsEnumerbaleOrAsyncEnumerable;

        /// <summary>
        /// Returns an enumerator that iterates asynchronously through the collection.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken that may be used to cancel the asynchronous iteration.</param>
        /// <returns>An enumerator that can be used to iterate asynchronously through the collection.</returns>
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator<T>(GetEnumerator());

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (IsValue())
                return ((IEnumerable<T>)_values).GetEnumerator();

            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        /// <summary>
        /// Creates a new optional that is the result of applying the given functions to the element.
        /// The some delegate get called only if the instance contains a value,
        /// otherwise returns the empty delegate.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="empty">The empty action.</param>
        /// <param name="some">The some action.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<TOutput> Map<TOutput>(Func<TOutput> empty, Func<T, TOutput> some) => IsValue() ? some(_values[0]) : empty();

        /// <summary>
        /// Asynchronously creates a new optional that is the result of applying the given functions to the element.
        /// The some delegate get called only if the instance contains a value,
        /// otherwise returns the empty delegate.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="empty">The empty action.</param>
        /// <param name="some">The some action.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<TOutput>> MapAsync<TOutput>(Func<Task<TOutput>> empty, Func<T, Task<TOutput>> some)
            => IsValue() ? await some(_values[0]).ConfigureAwait(false) : await empty().ConfigureAwait(false);

        /// <summary>
        /// Turns the current optional to a new optional one using the specified binding.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="binder">The binding function.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binder"/> is null.</exception>
        public Optional<TOutput> Bind<TOutput>(Func<T, Optional<TOutput>> binder) => Map(() => Optional<TOutput>.Empty(), value => binder(value));

        /// <summary>
        /// Asynchronously turns the current optional to a new optional one using the specified binding.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="binder">The binding function.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binder"/> is null.</exception>
        public async Task<Optional<TOutput>> BindAsync<TOutput>(Func<T, Task<Optional<TOutput>>> binder)
            => await MapAsync(() => Task.FromResult(Optional<TOutput>.Empty()), value => binder(value)).ConfigureAwait(false);

        /// <summary>
        /// Creates a new optional that is the result of calling the given function.
        /// The delegate get called only if the instance contains a value, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to call.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public Optional<T> Map(Func<T, T> some)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));
            return IsValue() ? some(_values[0]) : this;
        }

        /// <summary>
        /// Asynchronously creates a new optional that is the result of calling the given function.
        /// The delegate get called only if the instance contains a value, otherwise returns the current instance.
        /// </summary>
        /// <param name="some">The function to call.</param>
        /// <returns>An optional of <typeparamref name="T"/> type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="some"/> is null.</exception>
        public async Task<Optional<T>> MapAsync(Func<T, Task<T>> some)
        {
            _ = some ?? throw new ArgumentNullException(nameof(some));
            return IsValue() ? await some(_values[0]).ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public Optional<T> WhenEmpty(Func<T> empty)
        {
            _ = empty ?? throw new ArgumentNullException(nameof(empty));
            return !IsValue() ? empty() : this;
        }

        /// <summary>
        /// Asynchronously creates a new value that is the result of applying the given function when the instance is empty.
        /// The delegate get called only if the instance is empty, otherwise returns the current instance.
        /// </summary>
        /// <param name="empty">The empty map.</param>
        /// <returns>The replacement value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="empty"/> is null.</exception>
        public async Task<Optional<T>> WhenEmptyAsync(Func<Task<T>> empty)
        {
            _ = empty ?? throw new ArgumentNullException(nameof(empty));
            return !IsValue() ? await empty().ConfigureAwait(false) : this;
        }

        /// <summary>
        /// Provides with an optional of the specific type that is empty.
        /// </summary>
        /// <returns>An optional with no value.</returns>
        public static Optional<T> Empty() => new Optional<T>();

        /// <summary>
        /// Provides with an optional that contains a value of specific type.
        /// </summary>
        /// <param name="result">The value to be used for optional.</param>
        /// <returns>An optional with a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="result"/> is null.</exception>
        public static Optional<T> Some(T result)
        {
            _ = result ?? throw new ArgumentNullException(nameof(result));
            return new Optional<T>(new T[] { result });
        }
    }
}


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
using System.Reflection;
using System.Threading.Tasks;

using Xpandables.Net.Extensions;

namespace Xpandables.Net.Optionals
{
    /// <summary>
    /// Describes an object that can contain a value or not of a specific type.
    /// You can make unconditional calls to its contents using <see cref="System.Linq"/> without testing whether the content is there or not.
    /// The enumerator will only return the available value.
    /// If <typeparamref name="T"/> is an enumerable, use the <see cref="GetEnumerable"/> function to access its contain.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#pragma warning disable CA1716 // Identifiers should not match keywords
    public readonly partial struct Optional<T> : IEnumerable<T>
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        private readonly Type[] _genericTypes;
        private readonly T[] _values;

        private static readonly MethodInfo _arrayEmpty = typeof(Array).GetMethod("Empty")!;

        internal bool IsValue() => _values?.Length > 0;

        private Optional(T[]? values = default)
        {
            _values = values ?? Array.Empty<T>();
            _genericTypes = typeof(T).IsEnumerable() ? typeof(T).GetGenericArguments() : Type.EmptyTypes;
        }

        private T GetDefaultEnumerable()
        {
            var runtimeMethod = _arrayEmpty.MakeGenericMethod(_genericTypes[0]);
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
            if (!typeof(T).IsEnumerable())
                throw new InvalidOperationException($"{typeof(T).Name} is not an enumerable !");

            return IsValue() ? _values[0] : GetDefaultEnumerable();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        /// <summary>
        /// Creates a new optional that is the result of applying the given functions to the element.
        /// The some delegate get called only if the instance contains a value,
        /// otherwise returns the empty delegate.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="pattern">The pattern to be used.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="pattern"/> is null.</exception>
        public Optional<TOutput> Map<TOutput>((Func<TOutput> empty, Func<T, TOutput> some) pattern) => IsValue() ? pattern.some(_values[0]) : pattern.empty();

        /// <summary>
        /// Asynchronously creates a new optional that is the result of applying the given functions to the element.
        /// The some delegate get called only if the instance contains a value,
        /// otherwise returns the empty delegate.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="pattern">The pattern to be used.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="pattern"/> is null.</exception>
        public async Task<Optional<TOutput>> MapAsync<TOutput>((Func<Task<TOutput>> empty, Func<T, Task<TOutput>> some) pattern)
            => IsValue() ? await pattern.some(_values[0]).ConfigureAwait(false) : await pattern.empty().ConfigureAwait(false);

        /// <summary>
        /// Turns the current optional to a new optional one using the specified binding.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="binder">The binding function.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binder"/> is null.</exception>
        public Optional<TOutput> Bind<TOutput>(Func<T, Optional<TOutput>> binder) => Map((() => Optional<TOutput>.Empty(), value => binder(value)));

        /// <summary>
        /// Asynchronously turns the current optional to a new optional one using the specified binding.
        /// </summary>
        /// <typeparam name="TOutput">The type of the result.</typeparam>
        /// <param name="binder">The binding function.</param>
        /// <returns>A new optional that could contain a value or not.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binder"/> is null.</exception>
        public async Task<Optional<TOutput>> BindAsync<TOutput>(Func<T, Task<Optional<TOutput>>> binder)
            => await MapAsync((() => Task.FromResult(Optional<TOutput>.Empty()), value => binder(value))).ConfigureAwait(false);

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
        /// <returns>An optional with no value nor exception.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static Optional<T> Empty() => new Optional<T>();
#pragma warning restore CA1000 // Do not declare static members on generic types

        /// <summary>
        /// Provides with an optional that contains a value of specific type.
        /// </summary>
        /// <param name="result">The value to be used for optional.</param>
        /// <returns>An optional with a value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="result"/> is null.</exception>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static Optional<T> Some(T result)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            _ = result ?? throw new ArgumentNullException(nameof(result));
            return new Optional<T>(new T[] { result });
        }
    }
}

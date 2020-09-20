
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Optionals;

namespace Xpandables.Net.Enumerables
{
    /// <summary>
    /// Provides with extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Enumerates the collection source and performs the specified action on each element.
        /// </summary>
        /// <typeparam name="TSource">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = action ?? throw new ArgumentNullException(nameof(action));

            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                action(enumerator.Current);
        }

        /// <summary>
        /// Asynchronously enumerates the collection source and performs the specified action on each element.
        /// </summary>
        /// <typeparam name="TSource">Type of the element in the sequence.</typeparam>
        /// <param name="source">The source of the sequence.</param>
        /// <param name="action">Action to invoke for each element.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task ForEachAsync<TSource>(this IAsyncEnumerable<TSource> source, Action<TSource> action, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = action ?? throw new ArgumentNullException(nameof(action));

            var enumeratorAsync = source.GetAsyncEnumerator(cancellationToken);
            try
            {
                while (await enumeratorAsync.MoveNextAsync().ConfigureAwait(false))
                    action(enumeratorAsync.Current);
            }
            finally
            {
                await enumeratorAsync.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> to a read only collection.
        /// </summary>
        /// <typeparam name="TSource">The type of the object.</typeparam>
        /// <param name="source">An instance of the collection to be converted.</param>
        /// <returns>A new <see cref="ReadOnlyCollection{T}"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static IReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => new ReadOnlyCollectionBuilder<TSource>(source).ToReadOnlyCollection()
            };

        /// <summary>
        /// Returns the first element of the specified sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <returns>The first element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<TSource> FirstOrEmpty<TSource>(this IEnumerable<TSource> source)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => source.FirstOrDefault() is { } value ? Optional<TSource>.Some(value) : Optional<TSource>.Empty()
            };

        /// <summary>
        /// Returns the first element of the specified sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The first element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async Task<Optional<TSource>> FirstOrEmptyAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return await source.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element to a condition.</param>
        /// <returns>The first element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static Optional<TSource> FirstOrEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
             => (source, predicate) switch
             {
                 (_, null) => throw new ArgumentNullException(nameof(predicate)),
                 (null, _) => throw new ArgumentNullException(nameof(source)),
                 ({ }, { }) => source.FirstOrDefault(predicate) is { } value ? Optional<TSource>.Some(value) : Optional<TSource>.Empty()
             };

        /// <summary>
        /// Returns the first element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element to a condition.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The first element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static async Task<Optional<TSource>> FirstOrEmptyAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return await source.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the last elements of a sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <returns>The last element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<TSource> LastOrEmpty<TSource>(this IEnumerable<TSource> source)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => source.LastOrDefault() is { } value ? Optional<TSource>.Some(value) : Optional<TSource>.Empty()
            };

        /// <summary>
        /// Returns the last elements of a sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The last element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async Task<Optional<TSource>> LastOrEmptyAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => await source.LastOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            };

        /// <summary>
        /// Returns the last element of the sequence that satisfies the predicate or an empty optional if no such element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element to a condition.</param>
        /// <returns>The last element that satisfies the predicate or an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static Optional<TSource> LastOrEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => (source, predicate) switch
            {
                (_, null) => throw new ArgumentNullException(nameof(predicate)),
                (null, _) => throw new ArgumentNullException(nameof(source)),
                ({ }, { }) => source.LastOrDefault(predicate) is { } value ? Optional<TSource>.Some(value) : Optional<TSource>.Empty()
            };

        /// <summary>
        /// Returns the last elements of a sequence or an empty optional if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="predicate">A function to test each element to a condition.</param>/// 
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>The last element from the sequence or an empty result if the sequence contains no elements.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static async Task<Optional<TSource>> LastOrEmptyAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => await source.LastOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false)
            };

        /// <summary>
        /// Returns the element at the specified index in a sequence or an empty optional if the index is out of range
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Optional<TSource> ElementAtOrEmpty<TSource>(this IEnumerable<TSource> source, int index)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => source.ElementAtOrDefault(index) is { } result ? Optional<TSource>.Some(result) : Optional<TSource>.Empty()
            };

        /// <summary>
        /// Returns the element at the specified index in a sequence or an empty optional if the index is out of range
        /// </summary>
        /// <typeparam name="TSource">The type of the element in the sequence.</typeparam>
        /// <param name="source">the source of the sequence.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static async Task<Optional<TSource>> ElementAtOrEmptyAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default)
            => source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                _ => await source.ElementAtOrDefaultAsync(index, cancellationToken).ConfigureAwait(false)
            };
    }
}

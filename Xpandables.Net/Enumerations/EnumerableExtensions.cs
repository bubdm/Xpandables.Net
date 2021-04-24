
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net
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

            using var enumerator = source.GetEnumerator();
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
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
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
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return new ReadOnlyCollectionBuilder<TSource>(source).ToReadOnlyCollection();
        }
    }
}

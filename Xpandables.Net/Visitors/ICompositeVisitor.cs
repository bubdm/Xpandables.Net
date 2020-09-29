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
using System.Linq;
using System.Threading.Tasks;

namespace Xpandables.Net.Visitors
{
    /// <summary>
    /// Allows an application author to apply the visitor pattern by composition using a decorator.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    public interface ICompositeVisitor<in TElement> : IVisitor<TElement>
        where TElement : class, IVisitable<TElement>
    {
        internal IEnumerable<IVisitor<TElement>> VisitorInstances { get; }

        /// <summary>
        /// Asynchronously applies all found visitors to the element according to the visitor order.
        /// </summary>
        /// <param name="element">The element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="element"/> does not implement <see cref="IVisitable{TVisitable}"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public new async Task VisitAsync(TElement element)
        {
            _ = element ?? throw new ArgumentNullException(nameof(element));

            var tasks = VisitorInstances.OrderBy(o => o.Order).Select(visitor => element.AcceptAsync(visitor));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}

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

using Xpandables.Net.Enumerables;

namespace Xpandables.Net.VisitorRules
{
    /// <summary>
    /// The composite visitor used to wrap all visitors for a specific visitable type.
    /// </summary>
    /// <typeparam name="TElement">Type of the element to be visited</typeparam>
    [Serializable]
    public sealed class CompositeVisitorRule<TElement> : ICompositeVisitorRule<TElement>
        where TElement : class, IVisitable
    {
        private readonly IEnumerable<IVisitorRule<TElement>> _visitors;

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeVisitorRule{TElement}"/> with a collection of visitors.
        /// </summary>
        /// <param name="visitors">The collection of visitors for a specific type.</param>
        public CompositeVisitorRule(IEnumerable<IVisitorRule<TElement>> visitors)
            => _visitors = visitors ?? Enumerable.Empty<IVisitorRule<TElement>>();

        /// <summary>
        /// Applies all found visitors to the element according to the visitor order.
        /// </summary>
        /// <param name="element">The element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="element" /> does not implement <see cref="IVisitable" />.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public async Task VisitAsync(TElement element)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            if (!typeof(IVisitable).IsAssignableFrom(element.GetType()))
            {
                throw new ArgumentException(
                    $"{element.GetType().Name} must implement {nameof(IVisitable)} in order to accept a visitor.",
                    nameof(element));
            }

            var tasks = _visitors.OrderBy(o => o.Order).Select(visitor => element.AcceptAsync(visitor));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies all found visitors to the element according to the visitor order.
        /// </summary>
        /// <param name="element">The element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="element" /> does not implement <see cref="IVisitable" />.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public void Visit(TElement element)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            if (!typeof(IVisitable).IsAssignableFrom(element.GetType()))
            {
                throw new ArgumentException(
                    $"{element.GetType().Name} must implement {nameof(IVisitable)} in order to accept a visitor.",
                    nameof(element));
            }

            _visitors.OrderBy(o => o.Order)
                .ForEach(visitor => element.Accept(visitor));
        }

        async Task ICompositeVisitorRule.VisitAsync(object target)
        {
            if (target is TElement element)
                await VisitAsync(element).ConfigureAwait(false);
            else
                throw new ArgumentException($"{nameof(target)} is not of {typeof(TElement).Name} type.");
        }

        void ICompositeVisitorRule.Visit(object target)
        {
            if (target is TElement element)
                Visit(element);
            else
                throw new ArgumentException($"{nameof(target)} is not of {typeof(TElement).Name} type.");
        }
    }
}
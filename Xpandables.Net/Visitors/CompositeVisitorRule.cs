
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

using Xpandables.Net.CQRS;

namespace Xpandables.Net.Visitors
{
    /// <summary>
    /// The composite visitor used to wrap all visitors for a specific visitable type.
    /// </summary>
    /// <typeparam name="TElement">Type of the element to be visited</typeparam>
    [Serializable]
    public sealed class CompositeVisitorRule<TElement> : ICompositeVisitor<TElement>
        where TElement : class, IVisitable<TElement>
    {
        private readonly IEnumerable<IVisitor<TElement>> _visitorInstances;

        IEnumerable<IVisitor<TElement>> ICompositeVisitor<TElement>.VisitorInstances => _visitorInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeVisitorRule{TElement}"/> class with a collection of visitors.
        /// </summary>
        /// <param name="visitors">The collection of visitors for a specific type.</param>
        public CompositeVisitorRule(IEnumerable<IVisitor<TElement>> visitors)
            => _visitorInstances = visitors ?? Enumerable.Empty<IVisitor<TElement>>();    
    }
}
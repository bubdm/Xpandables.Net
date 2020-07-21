
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
using System.Threading.Tasks;

namespace Xpandables.Net.VisitorRules
{
    /// <summary>
    /// Visitor when no explicit registration exist for a given type.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    public sealed class NullVisitorRule<TElement> : IVisitorRule<TElement>
        where TElement : class, IVisitable
    {
        /// <summary>
        /// Declares a Visit operation.
        /// When overridden in derived class, this method will do the actual job of visiting the specified element.
        /// The default behavior just call the non-generic <see cref="IVisitorRule.VisitAsync(object)" /> method from base interface
        /// for validating the element.
        /// The non-generic method <see cref="IVisitorRule.VisitAsync(object)" /> checks that the argument is not null and implements <see cref="IVisitable" />.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="element" /> does not implement <see cref="IVisitable" />.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public async Task VisitAsync(TElement element) { /* intentionally left empty. */ await Task.CompletedTask.ConfigureAwait(false); }
    }
}
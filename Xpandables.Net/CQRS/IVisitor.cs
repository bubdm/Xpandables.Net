
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Allows an application author to apply the visitor pattern : The generic Visitor definition.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    public interface IVisitor<in TElement>
        where TElement : class, IVisitable<TElement>
    {
        /// <summary>
        /// Gets the zero-base order in which the visitor will be applied.
        /// The default value is zero.
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        /// Declares a Visit operation.
        /// When overridden in derived class, this method will do the actual job of visiting the specified element.
        /// The default behavior checks that the argument is not null.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual async Task VisitAsync(TElement element)
        {
            _ = element ?? throw new ArgumentNullException(nameof(element));
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
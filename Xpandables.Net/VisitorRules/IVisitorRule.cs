
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
    /// Visitor allows you to add new behaviors to an existing object without changing the object structure.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IVisitorRule
    {
        /// <summary>
        /// Declares a Visit operation.
        /// When overridden in derived class, this method will do the actual job of visiting the specified element.
        /// The default behavior checks that the argument is not null and implements <see cref="IVisitable"/>.
        /// </summary>
        /// <param name="target">The target element to be visited, must implement <see cref="IVisitable"/> interface.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="target"/> does not implement <see cref="IVisitable"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual async Task VisitAsync(object target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (!typeof(IVisitable).IsAssignableFrom(target.GetType()))
            {
                throw new ArgumentException(
                    $"{target.GetType().Name} must implement {nameof(IVisitable)} in order to used an visitable.",
                    nameof(target));
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Declares a Visit operation.
        /// When overridden in derived class, this method will do the actual job of visiting the specified element.
        /// The default behavior checks that the argument is not null and implements <see cref="IVisitable"/>.
        /// </summary>
        /// <param name="target">The target element to be visited, must implement <see cref="IVisitable"/> interface.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="target"/> does not implement <see cref="IVisitable"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual void Visit(object target)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (!typeof(IVisitable).IsAssignableFrom(target.GetType()))
            {
                throw new ArgumentException(
                    $"{target.GetType().Name} must implement {nameof(IVisitable)} in order to used an visitable.",
                    nameof(target));
            }
        }

        /// <summary>
        /// Determines the zero-base order in which the visitor will be applied.
        /// The default value is zero.
        /// </summary>
        public virtual int Order => 0;
    }

    /// <summary>
    /// Allows an application author to apply the visitor pattern : The generic Visitor definition.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    /// <typeparam name="TElement">Type of element to be visited.</typeparam>
    public interface IVisitorRule<in TElement> : IVisitorRule
        where TElement : class, IVisitable
    {
        /// <summary>
        /// Declares a Visit operation.
        /// When overridden in derived class, this method will do the actual job of visiting the specified element.
        /// The default behavior just call the non-generic <see cref="IVisitorRule.VisitAsync(object)"/> method from base interface
        /// for validating the element.
        /// The non-generic method <see cref="IVisitorRule.VisitAsync(object)"/> checks that the argument is not null and implements <see cref="IVisitable"/>.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="element"/> does not implement <see cref="IVisitable"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual async Task VisitAsync(TElement element) => await VisitAsync(target: element).ConfigureAwait(false);

        /// <summary>
        /// Declares a Visit operation.
        /// When overridden in derived class, this method will do the actual job of visiting the specified element.
        /// The default behavior just call the non-generic <see cref="IVisitorRule.Visit(object)"/> method from base interface
        /// for validating the element.
        /// The non-generic method <see cref="IVisitorRule.Visit(object)"/> checks that the argument is not null and implements <see cref="IVisitable"/>.
        /// </summary>
        /// <param name="element">Element to be visited.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="element"/> does not implement <see cref="IVisitable"/>.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public virtual void Visit(TElement element) => Visit(target: element);
    }
}
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

namespace Xpandables.Net.Expressions.Specifications
{
    /// <summary>
    /// Defines one method <see cref="IsSatisfiedBy(TSource)"/> which returns boolean to assert that the specification is satisfied or not.
    /// Inherits from <see cref="IQueryExpression{TSource}"/> that provides with <see cref="IQueryExpression{TSource, TResult}.GetExpression"/>
    /// method used to check whether or not the specification is satisfied by the <typeparamref name="TSource"/> object.
    /// </summary>
    /// <typeparam name="TSource">The type of the object to check for.</typeparam>
    public interface ISpecification<TSource> : IQueryExpression<TSource>
        where TSource : notnull
    {
        /// <summary>
        /// Returns a value that determines whether or not the specification is satisfied by the source object.
        /// </summary>
        /// <param name="source">The target source to check specification on.</param>
        /// <returns><see langword="true"/> if the specification is satisfied, otherwise <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public virtual bool IsSatisfiedBy(TSource source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return GetExpression().Compile().Invoke(source);
        }
    }
}
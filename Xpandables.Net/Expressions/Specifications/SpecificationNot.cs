
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
using System.Linq.Expressions;

namespace Xpandables.Net.Expressions.Specifications
{
    /// <summary>
    /// Provides the <see cref="Specification{TSource}"/> "Not" profile.
    /// </summary>
    /// <typeparam name="TSource">The type of the object to check for.</typeparam>
    public sealed class SpecificationNot<TSource> : Specification<TSource>
        where TSource : notnull
    {
        private readonly ISpecification<TSource> _other;

        /// <summary>
        /// Returns a new instance of <see cref="SpecificationOr{TSource}"/> class with the specification for Not.
        /// </summary>
        /// <param name="other">The specification to convert to Not.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="other"/> is null.</exception>exception>
        public SpecificationNot(ISpecification<TSource> other) => _other = other ?? throw new ArgumentNullException(nameof(other));

        /// <summary>
        /// Returns a value that determines whether or not the specification is satisfied by the source object.
        /// </summary>
        /// <param name="source">The target source to check specification on.</param>
        /// <returns><see langword="true" /> if the specification is satisfied, otherwise <see langword="false" /></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> is null.</exception>
        public override bool IsSatisfiedBy(TSource source) => !_other.IsSatisfiedBy(source);

        /// <summary>
        /// Returns the expression to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        public override Expression<Func<TSource, bool>> GetExpression()
            => ExpressionFactory<bool>.Not(_other.GetExpression());
    }
}

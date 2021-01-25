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
using System.Linq.Expressions;

namespace Xpandables.Net.Expressions.Specifications
{
    /// <summary>
    /// Provides the <see cref="Specification{TSource}"/> "Or" profile.
    /// </summary>
    /// <typeparam name="TSource">The type of the object to check for.</typeparam>
    public sealed class SpecificationOr<TSource> : Specification<TSource>
        where TSource : notnull
    {
        private readonly ISpecification<TSource> _left;
        private readonly ISpecification<TSource> _right;

        /// <summary>
        /// Returns a new instance of <see cref="SpecificationOr{TSource}"/> class with the specifications for composition.
        /// </summary>
        /// <param name="left">The specification for the left side.</param>
        /// <param name="right">The specification for the right side.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public SpecificationOr(ISpecification<TSource> left, ISpecification<TSource> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        /// <summary>
        /// Returns a new instance of <see cref="SpecificationAnd{TSource}"/> class with the specification and expression for composition.
        /// </summary>
        /// <param name="left">The specification for the left side.</param>
        /// <param name="rightExpression">The expression for the right side.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rightExpression"/> is null.</exception>
        public SpecificationOr(ISpecification<TSource> left, Expression<Func<TSource, bool>> rightExpression)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = new SpecificationExpression<TSource>(rightExpression ?? throw new ArgumentNullException(nameof(rightExpression)));
        }

        /// <summary>
        /// Returns the expression to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        public override Expression<Func<TSource, bool>> GetExpression()
            => ExpressionFactory<bool>.Or(_left.GetExpression(), _right.GetExpression());
    }
}

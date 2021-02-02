
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
    /// Provides the generic class to build <see cref="Specification{TSource}"/> instance.
    /// </summary>
    /// <typeparam name="TSource">the target instance type.</typeparam>
    public sealed class SpecificationBuilder<TSource> : Specification<TSource>
        where TSource : notnull
    {
        private readonly Expression<Func<TSource, bool>> _expression;

        /// <summary>
        /// Returns  new instance of <see cref="SpecificationBuilder{TSource}"/> class with the specified expression.
        /// </summary>
        /// <param name="expression">The expression to be used by the instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> can not be null.</exception>
        public SpecificationBuilder(Expression<Func<TSource, bool>> expression)
            => _expression = expression ?? throw new ArgumentNullException(nameof(expression));

        /// <summary>
        /// Returns the expression to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        public override Expression<Func<TSource, bool>> GetExpression() => _expression;
    }
}

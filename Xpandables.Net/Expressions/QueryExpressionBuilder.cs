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

namespace Xpandables.Net.Expressions
{
    /// <summary>
    /// Provides the generic class to build <see cref="QueryExpression{TSource, TResult}"/> instance.
    /// </summary>
    /// <typeparam name="TSource">the target instance type.</typeparam>
    /// <typeparam name="TResult">The property type to be used for result.</typeparam>
    public sealed class QueryExpressionBuilder<TSource, TResult> : QueryExpression<TSource, TResult>
        where TSource : notnull
    {
        private readonly Expression<Func<TSource, TResult>> _expression;

        /// <summary>
        /// Returns  new instance of <see cref="QueryExpressionBuilder{TSource, TResult}"/> class with the specified expression.
        /// </summary>
        /// <param name="expression">The expression to be used by the instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> can not be null.</exception>
        public QueryExpressionBuilder(Expression<Func<TSource, TResult>> expression)
            => _expression = expression ?? throw new ArgumentNullException(nameof(expression));

        /// <summary>
        /// Returns the expression to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        public sealed override Expression<Func<TSource, TResult>> GetExpression() => _expression;
    }
}

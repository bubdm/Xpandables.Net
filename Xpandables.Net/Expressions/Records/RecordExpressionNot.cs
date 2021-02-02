
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

namespace Xpandables.Net.Expressions.Records
{
#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Provides the <see cref="QueryExpression{TSource, TResult}"/> "Not" profile with the query expression.
    /// </summary>
    /// <param name="Expression">The query expression to act on.</param>
    /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
    /// <typeparam name="TResult">The type of the result of expression.</typeparam>
    public sealed record RecordExpressionNot<TSource, TResult>(IQueryExpression<TSource, TResult> Expression)
        : RecordExpression<TSource, TResult>
        where TSource : notnull
    {
        private Expression<Func<TSource, TResult>>? _cache;

        /// <summary>
        /// Returns a new instance of <see cref="RecordExpressionNot{TSource, TResult}"/> record with the expression.
        /// </summary>
        /// <param name="leftExpression">The query expression to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="leftExpression"/> is null.</exception>
        public RecordExpressionNot(Expression<Func<TSource, TResult>> leftExpression)
            : this(Expression: new QueryExpressionBuilder<TSource, TResult>(leftExpression ?? throw new ArgumentNullException(nameof(leftExpression)))) { }

        /// <summary>
        /// Returns the expression to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        public override Expression<Func<TSource, TResult>> GetExpression() => _cache ??= ExpressionFactory<TResult>.Not(Expression.GetExpression());
    }
}

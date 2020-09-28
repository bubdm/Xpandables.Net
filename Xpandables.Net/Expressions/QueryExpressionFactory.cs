
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
using System.Linq;
using System.Linq.Expressions;

namespace Xpandables.Net.Expressions
{
    /// <summary>
    /// Provides the query expression factory that contains methods to create generic query expressions.
    /// </summary>   
    public static class QueryExpressionFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueryExpression{TSource, TResult}"/> with <see cref="bool"/> result that return <see langword="true"/>.
        /// </summary>
        /// <typeparam name="TSource">The data type source.</typeparam>
        /// <returns>an new instance of <see cref="QueryExpression{TSource, TResult}"/> with boolean result.</returns>
        public static QueryExpression<TSource, bool> Create<TSource>() where TSource : class => new QueryExpressionBuilder<TSource, bool>(_ => true);
    }

    /// <summary>
    /// Provides the query expression factory that contains methods to create generic query expressions.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    public static class QueryExpressionFactory<TResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueryExpression{TSource, TResult}"/> with the specified expression.
        /// </summary>
        /// <typeparam name="TSource">The data type source.</typeparam>
        /// <param name="expression">The expression to be used by the instance.</param>
        /// <returns>an new instance of <see cref="QueryExpression{TSource, TResult}"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
        public static QueryExpression<TSource, TResult> Create<TSource>(Expression<Func<TSource, TResult>> expression)
            where TSource : class => new QueryExpressionBuilder<TSource, TResult>(expression);

        /// <summary>
        /// Returns the <see cref="Expression{TDelegate}"/> that represents the And form of two expressions.
        /// </summary>
        /// <typeparam name="TSource">The type of the expression parameter.</typeparam>
        /// <param name="left">The expression value  for left side.</param>
        /// <param name="right">The expression value for right side.</param>
        /// <returns><see cref="Expression{TDelegate}"/> result</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public static Expression<Func<TSource, TResult>> And<TSource>(Expression<Func<TSource, TResult>> left, Expression<Func<TSource, TResult>> right)
        {
            var _left = left ?? throw new ArgumentNullException(nameof(left));
            var _right = right ?? throw new ArgumentNullException(nameof(right));

            var body = Expression.AndAlso(Expression.Invoke(_left, _left.Parameters.Single()), Expression.Invoke(_right, _left.Parameters.Single()));
            return Expression.Lambda<Func<TSource, TResult>>(body, _left.Parameters.Single());
        }

        /// <summary>
        /// Returns the <see cref="Expression{TDelegate}"/> that represents the Or form of two expressions.
        /// </summary>
        /// <typeparam name="TSource">The type of the expression parameter.</typeparam>
        /// <param name="left">The expression value  for left side.</param>
        /// <param name="right">The expression value for right side.</param>
        /// <returns><see cref="Expression{TDelegate}"/> result</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        public static Expression<Func<TSource, TResult>> Or<TSource>(Expression<Func<TSource, TResult>> left, Expression<Func<TSource, TResult>> right)
        {
            var _left = left ?? throw new ArgumentNullException(nameof(left));
            var _right = right ?? throw new ArgumentNullException(nameof(right));

            var body = Expression.OrElse(Expression.Invoke(_left, _left.Parameters.Single()), Expression.Invoke(_right, _left.Parameters.Single()));
            return Expression.Lambda<Func<TSource, TResult>>(body, _left.Parameters.Single());
        }

        /// <summary>
        /// Returns the <see cref="Expression{TDelegate}"/> that represents the Not form of an expression.
        /// </summary>
        /// <typeparam name="TSource">The type of the expression parameter.</typeparam>
        /// <param name="expression">The expression value.</param>
        /// <returns><see cref="Expression{TDelegate}"/> result</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/>is null.</exception>
        public static Expression<Func<TSource, TResult>> Not<TSource>(Expression<Func<TSource, TResult>> expression)
        {
            var _left = expression ?? throw new ArgumentNullException(nameof(expression));

            var body = Expression.Not(Expression.Invoke(_left, _left.Parameters.Single()));
            return Expression.Lambda<Func<TSource, TResult>>(body, _left.Parameters.Single());
        }
    }
}


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
using System.Diagnostics.CodeAnalysis;

namespace Xpandables.Net5.Expressions
{
    /// <summary>
    /// Provides extensions methods for <see cref="IQueryExpression{TSource, TResult}"/>.
    /// </summary>
    public static class QueryExpressionHelpers
    {
        /// <summary>
        /// Applies the AND operator to both query expressions and returns a new one.
        /// </summary>
        /// <param name="left">The expression left side.</param>
        /// <param name="right">The expression right side.</param>
        /// <returns><see cref="QueryExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [return: NotNull]
        public static QueryExpression<TSource, TResult> And<TSource, TResult>(
            [NotNull] this IQueryExpression<TSource, TResult> left,
            [NotNull] IQueryExpression<TSource, TResult> right)
            where TSource : class
            => new QueryExpressionAnd<TSource, TResult>(left, right);

        /// <summary>
        /// Applies the OR operator to both query expressions and returns a new one.
        /// </summary>
        /// <param name="left">The expression left side.</param>
        /// <param name="right">The expression right side.</param>
        /// <returns><see cref="QueryExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [return: NotNull]
        public static QueryExpression<TSource, TResult> Or<TSource, TResult>(
            [NotNull] this IQueryExpression<TSource, TResult> left,
            [NotNull] IQueryExpression<TSource, TResult> right)
            where TSource : class => new QueryExpressionOr<TSource, TResult>(left, right);

        /// <summary>
        /// Applies the NOT operator to the query expression and returns a new one.
        /// </summary>
        /// <param name="queryExpression">The expression left side.</param>
        /// <returns><see cref="QueryExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [return: NotNull]
        public static QueryExpression<TSource, TResult> Not<TSource, TResult>(
            [NotNull] this IQueryExpression<TSource, TResult> queryExpression)
            where TSource : class => new QueryExpressionNot<TSource, TResult>(queryExpression);
    }
}

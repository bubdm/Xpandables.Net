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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Xpandables.Net.Expressions.Records
{
    /// <summary>
    /// Provides extensions methods for <see cref="IQueryExpression{TSource, TResult}"/>.
    /// </summary>
    public static class RecordExpressionHelpers
    {
        /// <summary>
        /// Applies the AND operator to both record expressions and returns a new one.
        /// </summary>
        /// <param name="left">The expression left side.</param>
        /// <param name="right">The expression right side.</param>
        /// <returns><see cref="RecordExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [return: NotNull]
        public static RecordExpression<TSource, TResult> And<TSource, TResult>(
             this IQueryExpression<TSource, TResult> left,
             IQueryExpression<TSource, TResult> right)
            where TSource : class
            => new RecordExpressionAnd<TSource, TResult>(left, right);

        /// <summary>
        /// Applies the AND operator to both record expressions and returns a new one.
        /// </summary>
        /// <param name="left">The expression left side.</param>
        /// <param name="right">The expression right side.</param>
        /// <returns><see cref="RecordExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [return: NotNull]
        public static RecordExpression<TSource, TResult> And<TSource, TResult>(
             this IQueryExpression<TSource, TResult> left,
             Expression<Func<TSource, TResult>> right)
            where TSource : class
            => new RecordExpressionAnd<TSource, TResult>(left, right);

        /// <summary>
        /// Applies the OR operator to both record expressions and returns a new one.
        /// </summary>
        /// <param name="left">The expression left side.</param>
        /// <param name="right">The expression right side.</param>
        /// <returns><see cref="RecordExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [return: NotNull]
        public static RecordExpression<TSource, TResult> Or<TSource, TResult>(
             this IQueryExpression<TSource, TResult> left,
             IQueryExpression<TSource, TResult> right)
            where TSource : class => new RecordExpressionOr<TSource, TResult>(left, right);

        /// <summary>
        /// Applies the OR operator to both record expressions and returns a new one.
        /// </summary>
        /// <param name="left">The expression left side.</param>
        /// <param name="right">The expression right side.</param>
        /// <returns><see cref="RecordExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="left"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="right"/> is null.</exception>
        [return: NotNull]
        public static RecordExpression<TSource, TResult> Or<TSource, TResult>(
             this IQueryExpression<TSource, TResult> left,
             Expression<Func<TSource, TResult>> right)
            where TSource : class => new RecordExpressionOr<TSource, TResult>(left, right);

        /// <summary>
        /// Applies the NOT operator to the record expression and returns a new one.
        /// </summary>
        /// <param name="queryExpression">The expression left side.</param>
        /// <returns><see cref="RecordExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [return: NotNull]
        public static RecordExpression<TSource, TResult> Not<TSource, TResult>(
             this IQueryExpression<TSource, TResult> queryExpression)
            where TSource : class => new RecordExpressionNot<TSource, TResult>(queryExpression);

        /// <summary>
        /// Applies the NOT operator to the record expression and returns a new one.
        /// </summary>
        /// <param name="queryExpression">The expression left side.</param>
        /// <returns><see cref="RecordExpression{TSource, TResult}"/> object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [return: NotNull]
        public static RecordExpression<TSource, TResult> Not<TSource, TResult>(
             this Expression<Func<TSource, TResult>> queryExpression)
            where TSource : class => new RecordExpressionNot<TSource, TResult>(queryExpression);
    }
}
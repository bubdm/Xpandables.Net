
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
    /// See http://www.albahari.com/expressions for information and examples.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Returns a <see langword="true"/> predicate.
        /// </summary>
        /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
        /// <returns>A true lambda expression.</returns>
        public static Expression<Func<TSource, bool>> True<TSource>() => f => true;

        /// <summary>
        /// Returns a <see langword="false"/> predicate.
        /// </summary>
        /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
        /// <returns>A false lambda expression.</returns>
        public static Expression<Func<TSource, bool>> False<TSource>() => f => false;

        /// <summary>
        /// Returns "Or" combination of two expression.
        /// </summary>
        /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
        /// <param name="left">The first expression to act on.</param>
        /// <param name="right">The second expression to act on.</param>
        /// <returns>A lambda expression which "Or" combination of the two expressions.</returns>
        public static Expression<Func<TSource, bool>> Or<TSource>(this Expression<Func<TSource, bool>> left, Expression<Func<TSource, bool>> right)
        {
            var invokedExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>()!);
            return Expression.Lambda<Func<TSource, bool>>(Expression.OrElse(left.Body, invokedExpr), left.Parameters);
        }

        /// <summary>
        /// Returns "And" combination of two expression.
        /// </summary>
        /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
        /// <param name="left">The first expression to act on.</param>
        /// <param name="right">The second expression to act on.</param>
        /// <returns>A lambda expression which "And" combination of the two expressions.</returns>
        public static Expression<Func<TSource, bool>> And<TSource>(this Expression<Func<TSource, bool>> left, Expression<Func<TSource, bool>> right)
        {
            var invokedExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>()!);
            return Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(left.Body, invokedExpr), left.Parameters);
        }
    }
}

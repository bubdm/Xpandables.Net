
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

namespace Xpandables.Net.Expressions
{
    /// <summary>
    /// The base class to define a class expression.
    /// </summary>
    /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
    /// <typeparam name="TResult">The type of the result of expression.</typeparam>
    public abstract class QueryExpression<TSource, TResult> : IQueryExpression<TSource, TResult>
        where TSource : class
    {
        /// <summary>
        /// Gets the expression tree for the underlying instance.
        /// </summary>
        [return: NotNull]
        public abstract Expression<Func<TSource, TResult>> GetExpression();

        /// <summary>
        /// Returns the unique hash code for the current instance.
        /// </summary>
        /// <returns><see cref="int"/> value.</returns>
        public override int GetHashCode()
        {
            int hash = GetExpression().GetHashCode();
            hash = hash * 17 + GetExpression().Parameters.Count;
            foreach (var param in GetExpression().Parameters)
            {
                hash *= 17;
                if (param != null) hash += param.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Determines whether the current instance equals the specified one.
        /// </summary>
        /// <param name="obj">The object to be compared to.</param>
        /// <returns><see cref="bool"/> value.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not QueryExpression<TSource, TResult> objVal) return false;
            if (ReferenceEquals(this, objVal)) return true;

            return 
                ExpressionComparer.AreEqual(GetExpression(), objVal.GetExpression());
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        [return: NotNull]
        public static implicit operator Expression<Func<TSource, TResult>>(
             QueryExpression<TSource, TResult> queryExpression)
            => queryExpression.GetExpression();

        [return: NotNull]
        public static implicit operator Func<TSource, TResult>(
             QueryExpression<TSource, TResult> queryExpression)
            => queryExpression.GetExpression().Compile();

        [return: NotNull]
        public static implicit operator QueryExpression<TSource, TResult>(
             Expression<Func<TSource, TResult>> expression)
            => QueryExpressionFactory<TResult>.Create(expression);

        [return: NotNull]
        public static QueryExpression<TSource, TResult> operator &(
             QueryExpression<TSource, TResult> left,
             QueryExpression<TSource, TResult> right)
          => new QueryExpressionAnd<TSource, TResult>(left, right: right);

        [return: NotNull]
        public static QueryExpression<TSource, TResult> operator |(
             QueryExpression<TSource, TResult> left,
             QueryExpression<TSource, TResult> right)
            => new QueryExpressionOr<TSource, TResult>(left, right: right);

        public static QueryExpression<TSource, TResult> operator ==(
            bool value,
             QueryExpression<TSource, TResult> right)
            => value ? right : !right;

        [return: NotNull]
        public static QueryExpression<TSource, TResult> operator ==(
             QueryExpression<TSource, TResult> left,
            bool value)
            => value ? left : !left;

        [return: NotNull]
        public static QueryExpression<TSource, TResult> operator !=(
            bool value,
             QueryExpression<TSource, TResult> right)
            => value ? !right : right;

        [return: NotNull]
        public static QueryExpression<TSource, TResult> operator !=(
             QueryExpression<TSource, TResult> left,
            bool value)
            => value ? !left : left;

        [return: NotNull]
        public static QueryExpression<TSource, TResult> operator !(
             QueryExpression<TSource, TResult> left)
            => new QueryExpressionNot<TSource, TResult>(expression: left);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// This class is a helper that provides a default implementation for <see cref="IQueryExpression{TSource}"/> with <see cref="bool"/> as result.
    /// </summary>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public class QueryExpression<TSource> : QueryExpression<TSource, bool>, IQueryExpression<TSource>
        where TSource : class
    {
        /// <summary>
        /// When implemented in derived class, this method will return the expression
        /// to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        [return: NotNull]
        public override Expression<Func<TSource, bool>> GetExpression() => _ => true;
    }
}

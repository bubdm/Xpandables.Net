
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
    /// The base record to define a record expression.
    /// </summary>
    /// <typeparam name="TSource">The data type to apply expression to.</typeparam>
    /// <typeparam name="TResult">The type of the result of expression.</typeparam>
    public abstract record RecordExpression<TSource, TResult> : IQueryExpression<TSource, TResult>
        where TSource : class
    {
        /// <summary>
        /// Gets the expression tree for the underlying instance.
        /// </summary>
        [return: NotNull]
        public abstract Expression<Func<TSource, TResult>> GetExpression();

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        [return: NotNull]
        public static implicit operator Expression<Func<TSource, TResult>>(
             RecordExpression<TSource, TResult> queryExpression)
            => queryExpression.GetExpression();

        [return: NotNull]
        public static implicit operator Func<TSource, TResult>(
             RecordExpression<TSource, TResult> queryExpression)
            => queryExpression.GetExpression().Compile();

        [return: NotNull]
        public static implicit operator RecordExpression<TSource, TResult>(
             Expression<Func<TSource, TResult>> expression)
            => new RecordExpressionBuilder<TSource, TResult>(expression);

        [return: NotNull]
        public static RecordExpression<TSource, TResult> operator &(
             RecordExpression<TSource, TResult> left,
             RecordExpression<TSource, TResult> right)
          => new RecordExpressionAnd<TSource, TResult>(Left: left, right);

        [return: NotNull]
        public static RecordExpression<TSource, TResult> operator |(
             RecordExpression<TSource, TResult> left,
             RecordExpression<TSource, TResult> right)
            => new RecordExpressionOr<TSource, TResult>(Left: left, right);

        public static RecordExpression<TSource, TResult> operator ==(
            bool value,
             RecordExpression<TSource, TResult> right)
            => value ? right : !right;

        [return: NotNull]
        public static RecordExpression<TSource, TResult> operator ==(
             RecordExpression<TSource, TResult> left,
            bool value)
            => value ? left : !left;

        [return: NotNull]
        public static RecordExpression<TSource, TResult> operator !=(
            bool value,
             RecordExpression<TSource, TResult> right)
            => value ? !right : right;

        [return: NotNull]
        public static RecordExpression<TSource, TResult> operator !=(
             RecordExpression<TSource, TResult> left,
            bool value)
            => value ? !left : left;

        [return: NotNull]
        public static RecordExpression<TSource, TResult> operator !(
             RecordExpression<TSource, TResult> left)
            => new RecordExpressionNot<TSource, TResult>(Expression: left);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// This record is a helper that provides a default implementation for <see cref="IQueryExpression{TSource}"/> with <see cref="bool"/> as result.
    /// </summary>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public record RecordExpression<TSource> : RecordExpression<TSource, bool>, IQueryExpression<TSource>
        where TSource : class
    {
        /// <summary>
        /// When implemented in derived record, this method will return the expression
        /// to be used for the <see langword="Where"/> clause in a query.
        /// </summary>
        [return: NotNull]
        public override Expression<Func<TSource, bool>> GetExpression() => _ => true;
    }
}

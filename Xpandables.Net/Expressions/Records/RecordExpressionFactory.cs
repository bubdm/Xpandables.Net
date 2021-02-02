
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
    /// <summary>
    /// Provides with the record expression factory that contains methods to create generic record expressions.
    /// </summary>
    public static class RecordExpressionFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="RecordExpression{TSource, TResult}"/> with <see cref="bool"/> result that return <see langword="true"/>.
        /// </summary>
        /// <typeparam name="TSource">The data type source.</typeparam>
        /// <returns>a new instance of <see cref="RecordExpression{TSource, TResult}"/> with boolean result.</returns>
        public static RecordExpression<TSource, bool> Create<TSource>() where TSource : class => new RecordExpressionBuilder<TSource, bool>(_ => true);
    }

    /// <summary>
    /// Provides the record expression factory that contains methods to create generic record expressions.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    public static class RecordExpressionFactory<TResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="RecordExpression{TSource, TResult}"/> with the specified expression.
        /// </summary>
        /// <typeparam name="TSource">The data type source.</typeparam>
        /// <param name="expression">The expression to be used by the instance.</param>
        /// <returns>a new instance of <see cref="QueryExpression{TSource, TResult}"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
        public static RecordExpression<TSource, TResult> Create<TSource>(Expression<Func<TSource, TResult>> expression)
            where TSource : class => new RecordExpressionBuilder<TSource, TResult>(expression);
    }
}

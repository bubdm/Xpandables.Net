
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
    /// Provides the generic record to build <see cref="RecordExpression{TSource, TResult}"/> instance.
    /// </summary>
    /// <param name="Expression">The expression to be used by the instance.</param>
    /// <typeparam name="TSource">the target instance type.</typeparam>
    /// <typeparam name="TResult">The property type to be used for result.</typeparam>
    public sealed record RecordExpressionBuilder<TSource, TResult>(Expression<Func<TSource, TResult>> Expression) : RecordExpression<TSource, TResult>
        where TSource : notnull
    {
        /// <summary>
        /// Returns the expression to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        public sealed override Expression<Func<TSource, TResult>> GetExpression() => Expression;
    }
}

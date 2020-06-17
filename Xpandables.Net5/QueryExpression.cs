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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// This class is a helper that provides a default implementation for <see cref="IQueryExpression{TSource}"/>.
    /// You must override the <see cref="BuildExpression"/> method in order to provide a custom behavior.
    /// </summary>
    /// <typeparam name="TSource">The data source type.</typeparam>
    public class QueryExpression<TSource> : IQueryExpression<TSource>
        where TSource : class
    {
        /// <summary>
        /// Gets the expression tree for the underlying instance.
        /// </summary>
        public Expression<Func<TSource, bool>> GetExpression() => BuildExpression();

        /// <summary>
        /// When implemented in derived class, this method will return the expression
        /// to be used for the clause <see langword="Where"/> in a query.
        /// </summary>
        protected virtual Expression<Func<TSource, bool>> BuildExpression() => _ => true;

#pragma warning disable CS1591
        public static implicit operator Expression<Func<TSource, bool>>([NotNull] QueryExpression<TSource> criteria)
             => criteria.GetExpression();

        public static implicit operator Func<TSource, bool>(QueryExpression<TSource> criteria)
            => criteria?.GetExpression().Compile() ?? (_ => true);
    }
}

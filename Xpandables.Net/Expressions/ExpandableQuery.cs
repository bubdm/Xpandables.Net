
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xpandables.Net.Expressions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
    /// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
    /// </summary>
    public class ExpandableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable
    {
        readonly ExpandableQueryProvider<T> _provider;
        readonly IQueryable<T> _inner;

        internal IQueryable<T> InnerQuery => _inner;             // Original query, that we're wrapping

        internal ExpandableQuery(IQueryable<T> inner)
        {
            _inner = inner;
            _provider = new ExpandableQueryProvider<T>(this);
        }

        Expression IQueryable.Expression => _inner.Expression;
        Type IQueryable.ElementType => typeof(T);
        IQueryProvider IQueryable.Provider => _provider;
        public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
        public override string ToString() => _inner.ToString()!;
    }

    class ExpandableQueryProvider<T> : IQueryProvider
    {
        readonly ExpandableQuery<T> _query;
        internal ExpandableQueryProvider(ExpandableQuery<T> query) => _query = query;

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression) => new ExpandableQuery<TElement>(_query.InnerQuery.Provider.CreateQuery<TElement>(expression.Expand()));
        IQueryable IQueryProvider.CreateQuery(Expression expression) => _query.InnerQuery.Provider.CreateQuery(expression.Expand());
        TResult IQueryProvider.Execute<TResult>(Expression expression) => _query.InnerQuery.Provider.Execute<TResult>(expression.Expand());
        object? IQueryProvider.Execute(Expression expression) => _query.InnerQuery.Provider.Execute(expression.Expand());
    }
}

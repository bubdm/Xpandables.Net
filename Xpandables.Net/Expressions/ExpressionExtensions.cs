
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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>Refer to http://www.albahari.com/nutshell/linqkit.html and
    /// http://tomasp.net/blog/linq-expand.aspx for more information.</summary>
    public static class ExpressionExtensions
    {
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query) => query switch
        {
            ExpandableQuery<T> query1 => query1,
            _ => new ExpandableQuery<T>(query)
        };

        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr) => (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        public static Expression Expand(this Expression expr) => new ExpressionExpander().Visit(expr);
        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr) => expr.Compile().Invoke();
        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1) => expr.Compile().Invoke(arg1);
        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2) => expr.Compile().Invoke(arg1, arg2);
        public static TResult Invoke<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3) => expr.Compile().Invoke(arg1, arg2, arg3);
        public static TResult Invoke<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => expr.Compile().Invoke(arg1, arg2, arg3, arg4);
    }
}

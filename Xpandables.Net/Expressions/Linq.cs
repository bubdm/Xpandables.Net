
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

namespace Xpandables.Net.Expressions
{
    /// <summary>
    /// Another good idea by Tomas Petricek.
    /// See http://tomasp.net/blog/dynamic-linq-queries.aspx for information on how it's used.
    /// </summary>
    public static class Linq
    {
        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> Expr<T, TResult>(Expression<Func<T, TResult>> expr) => expr;

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Func<T, TResult> Func<T, TResult>(Func<T, TResult> expr) => expr;
    }
}

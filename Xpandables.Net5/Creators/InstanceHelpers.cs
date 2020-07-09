
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Xpandables.Net5.Creators
{
    /// <summary>
    /// Provides extensions methods for dynamic use.
    /// </summary>
    public static class InstanceHelpers
    {
        /// <summary>
        /// Dynamically creates a class named <see cref="InstanceClass"/> + counter from the <see cref="IEnumerable{InstanceProperty}"/> dynamic properties.
        /// </summary>
        /// <param name="properties">The properties list to be used</param>
        /// <returns><see cref="Type"/> with the specified properties</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        public static Type CreateClass(IEnumerable<InstanceProperty> properties) => InstanceClassFactory.Instance.GetInstanceClass(properties);

        /// <summary>
        /// Dynamically creates a class named <see cref="InstanceClass"/> + counter from the list of dynamic properties.
        /// </summary>
        /// <param name="properties">The arguments to be used</param>
        /// <returns><see cref="Type"/> with the specified properties</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        public static Type CreateClass(params InstanceProperty[] properties) => InstanceClassFactory.Instance.GetInstanceClass(properties);

        /// <summary>
        /// Parse a string expression to a <see langword="Expression{Func{TSource, TObject}}"/>.
        /// </summary>
        /// <typeparam name="TSource">The result type to be used</typeparam>
        /// <typeparam name="TObject">The parameter or variable type</typeparam>
        /// <param name="expression">The string expression to use</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see langword="Expression{Func{TSource, TObject}}"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static Expression<Func<TSource, TObject>> ParseLambda<TSource, TObject>(string expression, params object[] values)
            => (Expression<Func<TSource, TObject>>)ParseLambda(typeof(TSource), typeof(TObject), expression, values);

        /// <summary>
        /// Parses a string expression to <see cref="LambdaExpression"/> using a <see cref="ParameterExpression"/> list.
        /// </summary>
        /// <param name="parameters">The list of <see cref="ParameterExpression"/></param>
        /// <param name="resultType">The type to apply the result linq expression</param>
        /// <param name="expression">The string expression to use</param>
        /// <param name="values">The parameters values</param>
        /// <returns>a <see cref="LambdaExpression"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type? resultType, string expression, params object[] values)
        {
            var parser = new InstanceExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        /// <summary>
        /// Parses a string expression to <see cref="LambdaExpression"/> using the specified element type <paramref name="itType"/>
        /// and others parameters.
        /// </summary>
        /// <param name="itType">The type of the parameter or variable</param>
        /// <param name="resultType">The type to apply the result linq expression</param>
        /// <param name="expression">The string expression to use</param>
        /// <param name="values">The parameters values</param>
        /// <returns>a <see cref="LambdaExpression"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static LambdaExpression ParseLambda(Type itType, Type? resultType, string expression, params object[] values)
            => ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, values);

        /// <summary>
        /// Parses a string expression to <see cref="Expression"/> with the specified parameters applied to the resultType.
        /// </summary>
        /// <param name="resultType">The type to apply the result linq expression</param>
        /// <param name="expression">The string expression to use</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="Expression"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            var parser = new InstanceExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        /// <summary>
        /// Applies a <paramref name="predicate"/> string with parameters to an <see cref="IQueryable{T}"/> source and returns and <see cref="IQueryable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <param name="source">The query-able data source</param>
        /// <param name="predicate">The predicate string.</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="IQueryable{TSource}"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string predicate, params object[] values)
            => (IQueryable<TSource>)((IQueryable)source).Where(predicate, values);

        /// <summary>
        /// Applies a <paramref name="predicate"/> string with parameters to an <see cref="IQueryable"/> source and returns and <see cref="IQueryable"/>.
        /// </summary>
        /// <param name="source">The queryable data source</param>
        /// <param name="predicate">the predicate string</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="IQueryable"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = predicate ?? throw new ArgumentNullException(nameof(predicate));

            var lambda = ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Applies Select with string selector and parameters values.
        /// </summary>
        /// <param name="source">The queryable data source</param>
        /// <param name="selector">The string selector</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="IQueryable"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));

            var lambda = ParseLambda(source.ElementType, null, selector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { source.ElementType, lambda.Body.Type },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Applies OrderBy with string ordering and parameters values.
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <param name="source">The queryable data source</param>
        /// <param name="ordering">The ordering string</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="IQueryable{T}"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string ordering, params object[] values)
            => (IQueryable<TSource>)((IQueryable)source).OrderBy(ordering, values);

        /// <summary>
        /// Applies OrderBy with string ordering and parameters.
        /// </summary>
        /// <param name="source">The queryable data source</param>
        /// <param name="ordering">The ordering string</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="IQueryable{T}"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = ordering ?? throw new ArgumentNullException(nameof(ordering));

            ParameterExpression[] parameters = { Expression.Parameter(source.ElementType, "") };
            var parser = new InstanceExpressionParser(parameters, ordering, values);
            var orderings = parser.ParseOrdering();
            Expression queryExpr = source.Expression;
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            foreach (InstanceOrdering o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
                    new Type[] { source.ElementType, o.Selector!.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        /// <summary>
        /// Takes the number of <paramref name="count"/> contiguous elements specified from the start of the queryable data source.
        /// </summary>
        /// <param name="source">The queryable data source</param>
        /// <param name="count">The number of elements to be retrieved</param>
        /// <returns>a <see cref="IQueryable"/> of <paramref name="count"/> elements</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable Take(this IQueryable source, int count)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Take",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Bypasses a specified <paramref name="count"/> number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="source">The queryable data source</param>
        /// <param name="count">The number of element to skip</param>
        /// <returns>a <see cref="IQueryable"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable Skip(this IQueryable source, int count)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Skip",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Checks whether the queryable data source contains any elements.
        /// </summary>
        /// <param name="source">The queryable data source to check for emptiness</param>
        /// <returns><see langword="true"/> if the source sequence contains any elements; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool Any(this IQueryable source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Any",
                    new Type[] { source.ElementType }, source.Expression)) is { } result && (bool)result;
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>he number of elements in the input sequence.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static int Count(this IQueryable source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Count",
                    new Type[] { source.ElementType }, source.Expression)) is { } result
                    ? (int)result : 0;
        }


        /// <summary>
        /// Applies GroupBy with string key and element selector and parameters values.
        /// </summary>
        /// <param name="source">The queryable data source</param>
        /// <param name="keySelector">The string key selector</param>
        /// <param name="elementSelector">The string element selector</param>
        /// <param name="values">The parameters values</param>
        /// <returns>an <see cref="IQueryable"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, params object[] values)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            _ = elementSelector ?? throw new ArgumentNullException(nameof(elementSelector));

            var keyLambda = ParseLambda(source.ElementType, null, keySelector, values);
            var elementLambda = ParseLambda(source.ElementType, null, elementSelector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        }
    }
}

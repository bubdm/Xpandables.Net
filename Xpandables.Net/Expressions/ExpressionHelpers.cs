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

using System.Linq.Expressions;

namespace Xpandables.Net.Expressions;

/// <summary>
/// Provides with extension methods for <see cref="Expression"/>.
/// </summary>
public static class ExpressionHelpers
{
    /// <summary>
    /// Combines the first expression to be used as parameter for the second expression.
    /// </summary>
    /// <param name="source">The first expression for composition.</param>
    /// <param name="composeResult">The compose expression to apply the source expression to.</param>
    /// <typeparam name="TSource">The type of the source model.</typeparam>
    /// <typeparam name="TCompose">The type the compose model.</typeparam>
    /// <typeparam name="TResult">The type of the result model.</typeparam>
    /// <returns>A statement matching the composition of target functions.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="composeResult"/> is null.</exception>
    public static Expression<Func<TSource, TResult>> Compose<TSource, TCompose, TResult>(
        this Expression<Func<TSource, TCompose>> source,
        Expression<Func<TCompose, TResult>> composeResult)
        where TSource : notnull
        where TCompose : notnull
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));
        _ = composeResult ?? throw new ArgumentNullException(nameof(composeResult));

        var param = Expression.Parameter(typeof(TSource), null);
        var invoke = Expression.Invoke(source, param);
        var result = Expression.Invoke(composeResult, invoke);

        return Expression.Lambda<Func<TSource, TResult>>(result, param);
    }

    /// <summary>
    ///  Filters a sequence of values based on a predicate to be applied on properties of <typeparamref name="TParam"/> type.
    /// </summary>
    /// <param name="source"> An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="propertyExpression">The expression that contains the member name for composition.</param>
    /// <param name="whereClause">A function to test each element of the source for a condition.</param>
    /// <typeparam name="TSource">The type of the model source.</typeparam>
    /// <typeparam name="TParam">The type of the model parameter.</typeparam>
    /// <returns> An <see cref="IQueryable{T}"/> that contains elements of <typeparamref name="TParam"/> type
    /// from the source sequence that satisfy the condition specified by the <paramref name="whereClause"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="whereClause"/> is null.</exception>
    public static IQueryable<TSource> Where<TSource, TParam>(this IQueryable<TSource> source,
        Expression<Func<TSource, TParam>> propertyExpression,
        Expression<Func<TParam, bool>> whereClause)
        where TSource : notnull
        where TParam : notnull =>
        source.Where(propertyExpression.Compose(whereClause));

    /// <summary>
    /// Returns the member name from the expression if found, otherwise returns null.
    /// </summary>
    /// <typeparam name="TSource">The type of the model class.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The expression that contains the member name.</param>
    /// <returns>A string that represents the name of the member found in the expression.</returns>
    public static string? GetMemberName<TSource, TProperty>(
        this Expression<Func<TSource, TProperty>> propertyExpression)
        where TSource : class
    {
        _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));

        if (propertyExpression.NodeType == ExpressionType.Constant)
        {
            var expression = propertyExpression as Expression;
            var constantExpression = expression as ConstantExpression;
            return constantExpression?.Value!.ToString();
        }

        if (propertyExpression.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        if ((propertyExpression.Body as UnaryExpression)?.Operand is MemberExpression operandExpression)
            return operandExpression.Member.Name;

        return default;
    }

    /// <summary>
    /// Returns a property or field access-or expression for the specified name that matches a property or a field in the model.
    /// </summary>
    /// <typeparam name="TSource">The type of the model class.</typeparam>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyOrFieldName">The name of the property or field.</param>
    /// <returns>An expression tree.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyOrFieldName"/> is null.</exception>
    /// <exception cref="ArgumentException">No property or field named propertyOrFieldName is
    /// defined in expression.Type or its base types.</exception>
    public static Expression<Func<TSource, TProperty>> CreateAccessorFor<TSource, TProperty>(
        this string propertyOrFieldName)
        where TSource : class
    {
        _ = propertyOrFieldName ?? throw new ArgumentNullException(nameof(propertyOrFieldName));

        var paramExpr = Expression.Parameter(typeof(TSource));
        var bodyExpr = Expression.PropertyOrField(paramExpr, propertyOrFieldName);
        return Expression.Lambda<Func<TSource, TProperty>>(bodyExpr, paramExpr);
    }
}

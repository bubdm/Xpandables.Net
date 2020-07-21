
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
    /// Provides with extension methods for <see cref="Expression"/>.
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Returns the member name from the expression if found, otherwise returns null.
        /// </summary>
        /// <typeparam name="TSource">The type of the model class.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The expression that contains the member name.</param>
        /// <returns>A string that represents the name of the member found in the expression.</returns>
        public static string? GetMemberName<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyExpression)
            where TSource : class
        {
            _ = propertyExpression ?? throw new ArgumentNullException(nameof(propertyExpression));

            if (propertyExpression is ConstantExpression constantExpression)
                return constantExpression.Value!.ToString();

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
        public static Expression<Func<TSource, TProperty>> CreateAccessorFor<TSource, TProperty>(this string propertyOrFieldName)
            where TSource : class
        {
            _ = propertyOrFieldName ?? throw new ArgumentNullException(nameof(propertyOrFieldName));

            var paramExpr = Expression.Parameter(typeof(TSource));
            var bodyExpr = Expression.PropertyOrField(paramExpr, propertyOrFieldName);
            return Expression.Lambda<Func<TSource, TProperty>>(bodyExpr, new ParameterExpression[] { paramExpr });
        }
    }
}

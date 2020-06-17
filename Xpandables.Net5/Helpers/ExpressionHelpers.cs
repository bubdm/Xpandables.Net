
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

namespace System
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
    }
}

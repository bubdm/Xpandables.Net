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
using System;
using System.Linq.Expressions;
using System.Reflection;

using Xpandables.Net.Enumerables;

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with extension method similar to the VB.Net key word <see lanwgord="With"/>..<see lanwgord="EndWith"/>.
    /// </summary>
    public static class AssingExtensions
    {
        /// <summary>
        /// Sets properties via lambda expression scope.
        /// This is similar to the VB.Net key word <see lanwgord="With"/>..<see lanwgord="EndWith"/>.
        /// </summary>
        /// <typeparam name="TSource">Type source.</typeparam>
        /// <param name="source">The source item to act on.</param>
        /// <param name="action">The action to be applied.</param>
        /// <returns>The same object after applying the action on it.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is null.</exception>
        public static TSource Assign<TSource>(this TSource source, Action<TSource> action)
            where TSource : class
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = action ?? throw new ArgumentNullException(nameof(action));

            action.Invoke(source);
            return source;
        }

        /// <summary>
        /// Sets properties via lambda expressions scope.
        /// This is similar to the VB.Net key word <see lanwgord="With"/>..<see lanwgord="EndWith"/>.
        /// </summary>
        /// <typeparam name="TSource">Type source.</typeparam>
        /// <param name="source">The source item to act on.</param>
        /// <param name="actions">The actions to be applied.</param>
        /// <returns>The same object after applying the action on it.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="actions"/> is null.</exception>
        public static TSource Assign<TSource>(this TSource source, params Action<TSource>[] actions)
            where TSource : class
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = actions ?? throw new ArgumentNullException(nameof(actions));

            actions.ForEach(action => action.Invoke(source));
            return source;
        }

        /// <summary>
        /// Sets properties via lambda expression. This is useful when dealing with <see cref="Expression{TDelegate}"/>.
        /// </summary>
        /// <typeparam name="TSource">Type source.</typeparam>
        /// <param name="source">The source instance to act on.</param>
        /// <param name="nameOfExpression">The expression delegate for the property.
        /// Just use <see langword="nameof"/> as expression for the delegate.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The current instance with modified property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="nameOfExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="nameOfExpression"/> is not valid.</exception>
        /// <exception cref="TargetInvocationException">An error occurred while setting the property value. See inner exception.</exception>
        public static TSource Assign<TSource>(this TSource source, Expression<Func<TSource, string>> nameOfExpression, object value)
            where TSource : class
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = nameOfExpression ?? throw new ArgumentNullException(nameof(nameOfExpression));

            if (nameOfExpression.Body is not ConstantExpression constantExpression)
                throw new ArgumentNullException($"Constant Expression expected. {nameof(nameOfExpression)}");

            if (constantExpression.Value is null)
                throw new ArgumentException($"Constant Expression Value is null. {nameof(nameOfExpression)}");

            if (source.GetType().GetProperty(constantExpression.Value.ToString()!) is not PropertyInfo propertyInfo)
                throw new ArgumentException($"Property {constantExpression.Value} does not exist in the {source.GetType().Name}.");

            if (!(propertyInfo.GetSetMethod() is MethodInfo))
                throw new ArgumentException($"Property {propertyInfo.Name} is not settable.");

            if (value != null && !propertyInfo.PropertyType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException($"Property type of {propertyInfo.Name} and type of the value does not match.");

            if (value is null && !(Nullable.GetUnderlyingType(propertyInfo.PropertyType) is Type))
                throw new ArgumentException($"Property type of {propertyInfo.Name} is not nullable.");

            propertyInfo.SetValue(source, value);
            return source;
        }
    }
}
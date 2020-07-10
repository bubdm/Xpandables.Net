
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

namespace Xpandables.Net5.Assertion
{
    /// <summary>
    ///  Contains static methods for representing program contracts that check for argument null or empty,
    ///  customs conditions and throws customs exceptions. If you don'TValue want to use message,
    ///  set the value to <see langword="null"/> or <see langword="default"/>.
    ///  You can defines your own extension contract method to match your requirement.
    /// </summary>
    public static class ContractExpressions
    {
        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the target value is null.
        /// </summary>
        /// <param name="this">actual value.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNull<TValue>(this TValue @this, string thisExpression)
            => new Contract<TValue>(@this, value => value is not null, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the target value is not null.
        /// </summary>
        /// <param name="this">actual value.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotNull<TValue>(this TValue @this, string thisExpression)
            => new Contract<TValue>(@this, value => value is null, thisExpression);

        /// <summary>
        /// Checks whether the target string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="this">The actual string.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <returns>An instance of <see cref="Contract{TValue}"/> where TValue is <see cref="string"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<string> WhenNull(this string @this, string thisExpression)
            => new Contract<string>(@this, value => !string.IsNullOrWhiteSpace(value), thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the condition is <see langword="true"/>.
        /// </summary>
        /// <param name="this">The actual string.</param>
        /// <param name="condition">The predicate to be applied.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        /// <exception cref="ContractException">The <paramref name="condition"/> is null.</exception>
        public static Contract<TValue> WhenConditionFailed<TValue>(this TValue @this, Func<TValue, bool> condition, string thisExpression)
            => new Contract<TValue>(@this, condition, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the <paramref name="this"/> is not greater than the <paramref name="compare"/>.
        /// </summary>
        /// <param name="this">The current value.</param>
        /// <param name="compare">The value to compare with.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotGreaterThan<TValue>(
            this TValue @this, TValue compare, string thisExpression)
            where TValue : unmanaged, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
            => new Contract<TValue>(@this, value => value.CompareTo(compare) > 0, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the <paramref name="this"/> is not greater than or equal to the <paramref name="compare"/>.
        /// </summary>
        /// <param name="this">The current value.</param>
        /// <param name="compare">The value to compare with.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotGreaterThanOrEqualTo<TValue>(
            this TValue @this, TValue compare, string thisExpression)
            where TValue : unmanaged, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
            => new Contract<TValue>(@this, value => value.CompareTo(compare) >= 0, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the <paramref name="this"/> is not lower than the <paramref name="compare"/>.
        /// </summary>
        /// <param name="this">The current value.</param>
        /// <param name="compare">The value to compare with.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotLowerThan<TValue>(
            this TValue @this, TValue compare, string thisExpression)
            where TValue : unmanaged, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
            => new Contract<TValue>(@this, value => value.CompareTo(compare) < 0, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the <paramref name="this"/> is
        /// not lower than or equal to the <paramref name="compare"/>.
        /// </summary>
        /// <param name="this">The current value.</param>
        /// <param name="compare">The value to compare with.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotLowerThanOrEqualTo<TValue>(
            this TValue @this, TValue compare, string thisExpression)
            where TValue : unmanaged, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
            => new Contract<TValue>(@this, value => value.CompareTo(compare) <= 0, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the <paramref name="this"/> is not between the range of values provided.
        /// </summary>
        /// <param name="this">The current value.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotInRange<TValue>(
            this TValue @this, TValue min, TValue max, string thisExpression)
            where TValue : unmanaged, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
            => new Contract<TValue>(@this, value => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0, thisExpression);

        /// <summary>
        /// Returns a <see cref="Contract{TValue}"/> that will check whether the <paramref name="this"/> is not in the collection of values provided.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The current value.</param>
        /// <param name="collection">The collection of values to act on.</param>
        /// <param name="thisExpression">The parameter name.</param>
        /// <returns>An instance of <see cref="Contract{TValue}"/>.</returns>
        /// <exception cref="ContractException">The <paramref name="collection"/> is null.</exception>
        /// <exception cref="ContractException">The <paramref name="thisExpression"/> is null.</exception>
        public static Contract<TValue> WhenNotInCollection<TValue>(
            this TValue @this, ICollection<TValue> collection, string thisExpression)
            => new Contract<TValue>(
                @this,
                value => !collection?.Contains(value) ?? throw new ContractException(new ArgumentNullException(nameof(collection))),
                thisExpression);
    }
}
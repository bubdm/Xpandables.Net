
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
using System.Collections.Generic;

namespace System
{
    /// <summary>
    ///  Contains static methods for representing program contracts that check for argument null or empty,
    ///  customs conditions and throws customs exceptions. If you don't want to use message,
    ///  set the value to <see langword="null"/> or <see langword="default"/>.
    ///  You can defines your own extension contract method to match your requirement.
    /// </summary>
    public static class ContractExpressions
    {
        /// <summary>
        /// Checks whether the target value is null.
        /// </summary>
        /// <param name="source">actual value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        public static Contract<T> WhenNull<T>(this T source, string parameterName)
            => new Contract<T>(
                source,
                value => value is not null,
                parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the target value is not null.
        /// </summary>
        /// <param name="source">actual value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="paramaterName"/> is null.</exception>
        public static Contract<T> WhenNotNull<T>(this T source, string paramaterName)
            => new Contract<T>(
                source,
                value => value is null,
                paramaterName ?? throw new ArgumentNullException(nameof(paramaterName)));

        /// <summary>
        /// Checks whether the target string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="source">The actual string.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>An instance of <see cref="Contract{T}"/> where T is <see cref="string"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        public static Contract<string> WhenNull(this string source, string parameterName)
            => new Contract<string>(
                source,
                value => !string.IsNullOrWhiteSpace(value),
                parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the predicate is <see langword="true"/>.
        /// </summary>
        /// <param name="source">The actual string.</param>
        /// <param name="predicate">The predicate to be applied.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is null.</exception>
        public static Contract<T> WhenConditionFailed<T>(
             this T source,
            Predicate<T> predicate,
            string parameterName)
            => new Contract<T>(
                source,
                predicate ?? throw new ArgumentNullException(nameof(predicate)),
                parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the <paramref name="source"/> is not greater than the <paramref name="comp"/>.
        /// </summary>
        /// <param name="source">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        public static Contract<T> WhenNotGreaterThan<T>(
            this T source, T comp, string parameterName)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(
                source,
                value => value.CompareTo(comp) > 0,
               parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the <paramref name="source"/> is not greater than or equal to the <paramref name="comp"/>.
        /// </summary>
        /// <param name="source">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        public static Contract<T> WhenNotGreaterThanOrEqualTo<T>(
            this T source, T comp, string parameterName)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(
                source,
                value => value.CompareTo(comp) >= 0,
               parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the <paramref name="source"/> is not lower than the <paramref name="comp"/>.
        /// </summary>
        /// <param name="source">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        public static Contract<T> WhenNotLowerThan<T>(
            this T source, T comp, string parameterName)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(
                source,
                value => value.CompareTo(comp) < 0,
               parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the <paramref name="source"/> is not lower than or equal to the <paramref name="comp"/>.
        /// </summary>
        /// <param name="source">The current value.</param>
        /// <param name="comp">The value to compare with.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> is null.</exception>
        public static Contract<T> WhenNotLowerThanOrEqualTo<T>(
            this T source, T comp, string parameterName)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(
                source,
                value => value.CompareTo(comp) <= 0,
               parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not between the range of values provided.
        /// </summary>
        /// <param name="actual">The current value.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotInRange<T>(
            this T actual, T min, T max, string parameterName)
            where T : struct, IComparable, IFormattable, IComparable<T>, IEquatable<T>
            => new Contract<T>(
                actual,
                value => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0,
                parameterName ?? throw new ArgumentNullException(nameof(parameterName)));

        /// <summary>
        /// Checks whether the <paramref name="actual"/> is not in the collection of values provided.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="actual">The current value.</param>
        /// <param name="collection">The collection of values to act on.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>An instance of <see cref="Contract{T}"/>.</returns>
        public static Contract<T> WhenNotInCollection<T>(
            this T actual, ICollection<T> collection, string parameterName)
            => new Contract<T>(
                actual,
                value => !collection?.Contains(value) ?? throw new ArgumentNullException(nameof(collection)),
                parameterName ?? throw new ArgumentNullException(nameof(parameterName)));
    }
}

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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

using Newtonsoft.Json;

namespace Xpandables.Net.Strings
{
    /// <summary>
    /// Provides with methods to extend use of <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the argument object into the current text equivalent <see cref="string"/>
        /// using the default <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <param name="value">The format string.</param>
        /// <param name="args">The object to be formatted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or <paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="args"/> is null or empty.</exception>
        /// <exception cref="FormatException">The format is invalid.</exception>
        /// <returns>value <see cref="string"/> filled with <paramref name="args"/>.</returns>
        public static string StringFormat(this string value, params object[] args)
            => value.StringFormat(CultureInfo.InvariantCulture, args);

        /// <summary>
        /// Replaces the argument object into the current text equivalent <see cref="string"/> using the specified culture.
        /// </summary>
        /// <param name="value">The format string.</param>
        /// <param name="cultureInfo">CultureInfo to be used.</param>
        /// <param name="args">The object to be formatted.</param>
        /// <returns>value <see cref="string"/> filled with <paramref name="args"/></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="args"/> is null or empty.</exception>
        /// <exception cref="FormatException">The format is invalid.</exception>
        public static string StringFormat(this string value, CultureInfo cultureInfo, params object[] args)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            _ = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));

            return string.Format(cultureInfo, value, args);
        }

        /// <summary>
        /// Serializes the current instance to JSON string.
        /// </summary>
        /// <param name="source">The object to act on.</param>
        /// <param name="options">The serializer options to be applied.</param>
        /// <returns>A JSOn string representation of the object.</returns>
        public static string ToJsonString<T>(this T source, JsonSerializerOptions? options = default) => System.Text.Json.JsonSerializer.Serialize(source, options);

        /// <summary>
        /// Concatenates all the elements of an <see cref="IEnumerable{T}"/>,
        /// using the specified string separator between each element.
        /// </summary>
        /// <typeparam name="TSource">The generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="separator">The string to use as a separator.
        /// Separator is included in the returned string only if value has more than one element.</param>
        /// <returns>A string that consists of the elements in value delimited by the separator string.
        /// If value is an empty array, the method returns String.Empty.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public static string StringJoin<TSource>(this IEnumerable<TSource> collection, string separator)
            => string.Join(separator, collection);

        /// <summary>
        /// Concatenates all the elements of an <see cref="IEnumerable{T}"/>,
        /// using the specified char separator between each element.
        /// </summary>
        /// <typeparam name="TSource">The generic type parameter.</typeparam>
        /// <param name="collection">The collection to act on.</param>
        /// <param name="separator">The string to use as a separator.
        /// Separator is included in the returned string only if value has more than one element.</param>
        /// <returns>A string that consists of the elements in value delimited by the separator string.
        /// If value is an empty array, the method returns String.Empty.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is null.</exception>
        public static string StringJoin<TSource>(this IEnumerable<TSource> collection, char separator)
            => string.Join(separator.ToString(CultureInfo.InvariantCulture), collection);

        /// <summary>
        /// Tries to convert a string to the specified value type.
        /// </summary>
        /// <typeparam name="TResult">Type source.</typeparam>
        /// <param name="value">The string value.</param>
        /// <param name="result">The string value converted to the specified value type.</param>
        /// <param name="valueTypeException">The handled exception during conversion.</param>
        /// <returns>Returns <see langword="true"/> if conversion OK and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null or empty.</exception>
        public static bool TryToValueType<TResult>(
            this string value,
            [MaybeNullWhen(returnValue: false)] out TResult result,
            [MaybeNullWhen(returnValue: true)] out Exception valueTypeException)
            where TResult : struct, IComparable, IFormattable, IConvertible, IComparable<TResult>, IEquatable<TResult>
        {
            try
            {
                valueTypeException = default;
                result = (TResult)Convert.ChangeType(value, typeof(TResult), CultureInfo.CurrentCulture);
                return true;
            }
            catch (Exception exception) when (exception is InvalidCastException
                                            || exception is FormatException
                                            || exception is OverflowException)
            {
                valueTypeException = exception;
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Converts string date to <see cref="DateTime"/> type.
        /// If error, returns an exception.
        /// </summary>
        /// <param name="source">A string containing a date and time to convert.</param>
        /// <param name="provider">An object that supplies culture-specific format information about string.</param>
        /// <param name="styles"> A bitwise combination of enumeration values that indicates the permitted format
        /// of string. A typical value to specify is System.Globalization.DateTimeStyles.None.</param>
        /// <param name="result">An object that is equivalent to the date and time contained in <paramref name="source"/> as specified
        /// by formats, provider, and style.</param>
        /// <param name="dateTimeException">The handled exception during conversion.</param>
        /// <param name="formats">An array of allowable formats of strings.</param>
        /// <returns>Returns <see langword="true"/> if conversion OK and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        public static bool TryToDateTime(
            this string source,
            IFormatProvider provider,
            DateTimeStyles styles,
            [MaybeNullWhen(returnValue: false)] out DateTime result,
            [MaybeNullWhen(returnValue: true)] out Exception dateTimeException,
            params string[] formats)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = provider ?? throw new ArgumentNullException(nameof(provider));

            try
            {
                dateTimeException = default;
                result = DateTime.ParseExact(source, formats, provider, styles);
                return true;
            }
            catch (Exception exception) when (exception is ArgumentException || exception is FormatException)
            {
                dateTimeException = exception;
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to deserialize the JSON string to the specified type.
        /// The default implementation used the <see cref="System.Text.Json"/> API.
        /// </summary>
        /// <typeparam name="TResult">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="result">The deserialized object from the JSON string.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns><see langword="true"/> if OK, otherwise <see langword="false"/>.</returns>
        public static bool TryDeserialize<TResult>(this
            string value,
            [MaybeNullWhen(false)] out TResult result,
            [MaybeNullWhen(true)] out Exception exception,
            JsonSerializerSettings? options = default)
            where TResult : class
        {
            result = default;
            exception = default;

            try
            {
                result = JsonConvert.DeserializeObject<TResult>(value, options);
                if (result is null)
                {
                    exception = new ArgumentNullException(nameof(value), "No result from deserialization.");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}

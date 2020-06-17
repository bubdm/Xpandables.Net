
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// Provides with methods to extend use of <see cref="string"/>.
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Tries to deserialize the json string to the specified type.
        /// </summary>
        /// <typeparam name="TSource"> The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="obj">The deserialized object from the JSON string.</param>
        /// <param name="exception">The handled exception.</param>
        /// <param name="options">The json converter options.</param>
        public static bool TryDeserializeObject<TSource>(
            this string value,
            [MaybeNull] out TSource? obj,
            [NotNullWhen(false)] out Exception? exception,
            JsonSerializerOptions? options = default)
            where TSource : class
        {
            obj = default;
            exception = default;

            try
            {
                obj = JsonSerializer.Deserialize<TSource>(value, options);
                return true;
            }
            catch (Exception ex) when (ex is JsonException || ex is ArgumentNullException)
            {
                exception = ex;
                return false;
            }
        }

        /// <summary>
        /// Determines whether the string is a well formatted email address.
        /// </summary>
        /// <param name="source">The email address to be checked.</param>
        /// <param name="mailException">The source is not in a recognized format.</param>
        /// <returns><see langword="true"/> if well formatted, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static bool IsValidEmailAddress(this string source, [NotNullWhen(false)] out Exception? mailException)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            try
            {
                mailException = default;
                _ = new MailAddress(source);
                return true;
            }
            catch (Exception exception) when (exception is FormatException || exception is ArgumentException)
            {
                mailException = exception;
                return false;
            }
        }

        /// <summary>
        /// Determines whether the string is a well formatted email address.
        /// </summary>
        /// <param name="source">The email address to be checked.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="phoneException">The phone does not match the pattern.</param>
        /// <returns><see langword="true"/> if well formatted, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pattern"/> is null.</exception>
        public static bool IsValidPhone(this string source, string pattern, [NotNullWhen(false)] out Exception? phoneException)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = pattern ?? throw new ArgumentNullException(nameof(pattern));

            try
            {
                var result = Regex.IsMatch(source, pattern);
                if (result)
                {
                    phoneException = default;
                    return true;
                }

                phoneException = new ArgumentException("Phone does not match the pattern.");
                return result;
            }
            catch (Exception exception) when (exception is ArgumentException || exception is RegexMatchTimeoutException)
            {
                phoneException = exception;
                return false;
            }
        }

        /// <summary>
        /// Escapes special characters from the target string.
        /// </summary>
        /// <param name="value">The string to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static string StringEscape(this string value)
            => new string(
                Array.FindAll(
                    value?.ToCharArray() ?? throw new ArgumentNullException(nameof(value)),
                    c => char.IsLetterOrDigit(c)
                        || char.IsWhiteSpace(c)
                        || c == '-'));

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
            => StringFormat(value, CultureInfo.InvariantCulture, args);

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
        /// <returns>Returns<see langword="true"/> if conversion succeeded and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null or empty.</exception>
        public static bool TryToValueType<TResult>(
            this string value,
            [NotNullWhen(true)] out TResult result,
            [NotNullWhen(false)] out Exception? valueTypeException)
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
        /// <returns>Returns<see langword="true"/> if conversion succeeded and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="provider"/> is null.</exception>
        public static bool TryToDateTime(
            this string source,
            IFormatProvider provider,
            DateTimeStyles styles,
            [NotNullWhen(true)] out DateTime result,
            [NotNullWhen(false)] out Exception? dateTimeException,
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
        /// Tries to load assembly from its name.
        /// </summary>
        /// <param name="assemblyName">The full assembly name.</param>
        /// <param name="loadedAssembly">The loaded assembly if succeeded.</param>
        /// <param name="assemblyException">The handled exception during assembly loading.</param>
        /// <returns>Returns<see langword="true"/> if loading succeeded and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblyName"/> is null.</exception>
        public static bool TryLoadAssembly(
            this string assemblyName,
            [NotNullWhen(true)] out Assembly? loadedAssembly,
            [NotNullWhen(false)] out Exception? assemblyException)
        {
            _ = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));

            try
            {
                assemblyException = default;
                loadedAssembly = Assembly.LoadFrom(assemblyName);
                return true;
            }
            catch (Exception exception) when (exception is ArgumentException
                                            || exception is FileNotFoundException
                                            || exception is FileLoadException
                                            || exception is BadImageFormatException
                                            || exception is PathTooLongException
                                            || exception is Security.SecurityException)
            {
                loadedAssembly = default;
                assemblyException = exception;
                return false;
            }
        }

        /// <summary>
        /// Tries to get type from its string name.
        /// </summary>
        /// <param name="typeName">The name of the type to find.</param>
        /// <param name="foundType">The found type.</param>
        /// <param name="typeException">The handled type exception.</param>
        /// <returns>Returns<see langword="true"/> if loading succeeded and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeName"/> is null.</exception>
        public static bool TryGetType(
            this string typeName,
            [NotNullWhen(true)] out Type? foundType,
            [NotNullWhen(false)] out Exception? typeException)
        {
            _ = typeName ?? throw new ArgumentNullException(nameof(typeName));

            try
            {
                typeException = default;
                foundType = Type.GetType(typeName, true, true);
                if (foundType is null)
                {
                    typeException = new ArgumentException("Expected type not found.");
                    return false;
                }

                return true;
            }
            catch (Exception exception) when (exception is TargetInvocationException
                                            || exception is TypeLoadException
                                            || exception is ArgumentException
                                            || exception is FileNotFoundException
                                            || exception is FileLoadException
                                            || exception is BadImageFormatException)
            {
                typeException = exception;
                foundType = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the type from string, if not found, try to load from the assembly.
        /// </summary>
        /// <param name="typeName">The name of the type to find.</param>
        /// <param name="assemblyName">The assembly to act on.</param>
        /// <param name="foundType">The found type.</param>
        /// <param name="typeException">The handled type exception.</param>
        /// <returns>Returns<see langword="true"/> if loading succeeded and <see langword="false"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="typeName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="assemblyName"/> is null.</exception>
        public static bool TryGetType(
            this string typeName,
            string assemblyName,
            [NotNullWhen(true)] out Type? foundType,
            [NotNullWhen(false)] out Exception? typeException)
        {
            _ = typeName ?? throw new ArgumentNullException(nameof(typeName));
            _ = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));

            if (typeName.TryGetType(out foundType, out typeException))
                return true;

            if (!assemblyName.TryLoadAssembly(out var assembly, out typeException))
            {
                foundType = default;
                return false;
            }

            try
            {
                typeException = default;
                foundType = Array
                    .Find(assembly!
                        .GetExportedTypes(), type => type.FullName!.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));

                if (foundType is null)
                {
                    typeException = new ArgumentException("Expected type not found.");
                    return false;
                }

                return true;
            }
            catch (Exception exception) when (exception is NotSupportedException || exception is FileNotFoundException)
            {
                typeException = exception;
                return false;
            }
        }

        /// <summary>
        /// Append the given query keys and values to the uri.
        /// </summary>
        /// <param name="uri">The base uri.</param>
        /// <param name="queryString">A collection of name value query pairs to append.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="queryString"/> is null.</exception>
        public static string AddQueryString(this string uri, IDictionary<string, string> queryString)
        {
            _ = uri ?? throw new ArgumentNullException(nameof(uri));
            _ = queryString ?? throw new ArgumentNullException(nameof(queryString));

            var anchorIndex = uri.IndexOf('#');
            var uriToBeAppended = uri;
            var anchorText = "";

            // If there is an anchor, then the query string must be inserted before its first occurence.
            if (anchorIndex != -1)
            {
                anchorText = uri.Substring(anchorIndex);
                uriToBeAppended = uri.Substring(0, anchorIndex);
            }

            var queryIndex = uriToBeAppended.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);
            foreach (var parameter in queryString)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            sb.Append(anchorText);
            return sb.ToString();
        }
    }
}

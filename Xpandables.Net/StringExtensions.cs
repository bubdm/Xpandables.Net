
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
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net;

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
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null or
    /// <paramref name="args"/> is null.</exception>
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
    /// Parses the text representing a single JSON value into an instance of the 
    /// anonymous type specified by a generic type parameter.
    /// </summary>
    /// <typeparam name="T">The anonymous type.</typeparam>
    /// <param name="json">The JSON data to parse.</param>
    /// <param name="_">The anonymous instance.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns> A T representation of the JSON value.</returns>
    /// <exception cref="JsonException">The JSON is invalid or T is not compatible with the JSON or 
    /// There is remaining data in the string beyond a single JSON value.</exception>
    /// <exception cref="NotSupportedException">There is no compatible <see cref="JsonConverter"/> 
    /// for T or its serializable members.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="json"/> is null.</exception>
    public static T? DeserializeAnonymousType<T>(this string json, T _, JsonSerializerOptions? options = default)
         => JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of 
    /// an anonymous type specified by a generic type parameter. The stream will be read to completion.
    /// </summary>
    /// <typeparam name="T">The anonymous type.</typeparam>
    /// <param name="stream">The JSON data to parse.</param>
    /// <param name="_">The anonymous instance.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns> A T representation of the JSON value.</returns>
    /// <exception cref="JsonException">The JSON is invalid or T is not compatible with the JSON or 
    /// There is remaining data in the stream.</exception>
    /// <exception cref="NotSupportedException">There is no compatible <see cref="JsonConverter"/> 
    /// for T or its serializable members.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
    public static ValueTask<T?> DeserializeAnonymousTypeAsync<T>(
        this Stream stream,
        T _,
        JsonSerializerOptions? options = default,
        CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);

    /// <summary>
    /// Serializes the current instance to JSON string using <see cref="System.Text.Json"/>.
    /// </summary>
    /// <param name="source">The object to act on.</param>
    /// <param name="options">The serializer options to be applied.</param>
    /// <returns>A JSOn string representation of the object.</returns>
    /// <exception cref="NotSupportedException">There is no compatible 
    /// System.Text.Json.Serialization.JsonConverter for TValue or its serializable members.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException">inputType is not compatible with value.</exception>
    /// <exception cref="NotSupportedException"> There is no compatible System.Text.Json.Serialization.JsonConverter 
    /// for inputType or its serializable members.</exception>
    public static string ToJsonString<T>(this T source, JsonSerializerOptions? options = default)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));
        return JsonSerializer.Serialize(source, source.GetType(), options);
    }

    /// <summary>
    /// Deserializes the current JSON element to an object of specified <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">The target type of the UTF-8 encoded text.</typeparam>
    /// <param name="element">The JSON text to parse.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A <typeparamref name="T"/> representation of the JSON value.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
    /// There is remaining data in the span beyond a single JSON value.</exception>
    /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
    /// for returnType or its serializable members.</exception>
    public static T? ToObject<T>(this JsonElement element, JsonSerializerOptions? options = default)
        => ToObject(element, typeof(T), options) is { } result ? (T)result : default;

    /// <summary>
    /// Asynchronously deserializes the current JSON element to an object of specified <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">The target type of the UTF-8 encoded text.</typeparam>
    /// <param name="element">The JSON text to parse.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A <typeparamref name="T"/> representation of the JSON value.</returns>
    /// <exception cref="InvalidOperationException">See inner exception.</exception>
    public static async ValueTask<T?> ToObjectAsync<T>(
        this JsonElement element,
        JsonSerializerOptions? options = default,
        CancellationToken cancellationToken = default)
        => await ToObjectAsync(element, typeof(T), options, cancellationToken) is { } result ? (T)result : default;

    /// <summary>
    /// Deserializes the current JSON element to an object of specified type.
    /// </summary>
    /// <param name="element">The JSON text to parse.</param>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A returnType representation of the JSON value.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="returnType"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
    /// There is remaining data in the span beyond a single JSON value.</exception>
    /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
    /// for returnType or its serializable members.</exception>
    public static object? ToObject(this JsonElement element, Type returnType, JsonSerializerOptions? options = default)
    {
        _ = returnType ?? throw new ArgumentNullException(nameof(returnType));

        var bufferWriter = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(bufferWriter);
        element.WriteTo(writer);
        writer.Flush();

        return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, returnType, options);
    }

    /// <summary>
    /// Deserializes the current JSON document to an object of specified <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">The target type of the UTF-8 encoded text.</typeparam>
    /// <param name="document">The JSON document to parse.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A <typeparamref name="T"/> representation of the JSON value.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="document"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
    /// There is remaining data in the span beyond a single JSON value.</exception>
    /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
    /// for returnType or its serializable members.</exception>
    public static T? ToObject<T>(this JsonDocument document, JsonSerializerOptions? options = default)
    {
        _ = document ?? throw new ArgumentNullException(nameof(document));
        return document.RootElement.ToObject<T>(options);
    }

    /// <summary>
    /// Asynchronously deserializes the current JSON document to an object of specified <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">The target type of the UTF-8 encoded text.</typeparam>
    /// <param name="document">The JSON document to parse.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A <typeparamref name="T"/> representation of the JSON value.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="document"/> is null.</exception>
    /// <exception cref="InvalidOperationException">See inner exception.</exception>
    public static ValueTask<T?> ToObjectAsync<T>(
        this JsonDocument document,
        JsonSerializerOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        _ = document ?? throw new ArgumentNullException(nameof(document));
        return document.RootElement.ToObjectAsync<T>(options, cancellationToken);
    }

    /// <summary>
    /// Deserializes the current JSON document to an object of specified type.
    /// </summary>
    /// <param name="document">The JSON document to parse.</param>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A returnType representation of the JSON value.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="returnType"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="document"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
    /// There is remaining data in the span beyond a single JSON value.</exception>
    /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
    /// for returnType or its serializable members.</exception>
    public static object? ToObject(this JsonDocument document, Type returnType, JsonSerializerOptions? options = default)
    {
        _ = document ?? throw new ArgumentNullException(nameof(document));
        return document.RootElement.ToObject(returnType, options);
    }

    /// <summary>
    /// Asynchronously deserializes the current JSON element to an object of specified <paramref name="returnType"/> type.
    /// </summary>
    /// <param name="element">The JSON text to parse.</param>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A returnType representation of the JSON value.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="returnType"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="element"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The JsonElement.ValueKind of this value is System.Text.Json.JsonValueKind.Undefined.</exception>
    /// <exception cref="ObjectDisposedException">The parent System.Text.Json.JsonDocument has been disposed.</exception>
    /// <exception cref="JsonException">The JSON is invalid. -or- returnType is not compatible with the JSON. -or- 
    /// There is remaining data in the span beyond a single JSON value.</exception>
    /// <exception cref="NotSupportedException">There is no compatible System.Text.Json.Serialization.JsonConverter 
    /// for returnType or its serializable members.</exception>
    public static ValueTask<object?> ToObjectAsync(
        this JsonElement element,
        Type returnType,
        JsonSerializerOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);
        element.WriteTo(writer);
        writer.Flush();

        return JsonSerializer.DeserializeAsync(memoryStream, returnType, options, cancellationToken);
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
    /// <exception cref="OutOfMemoryException">The length of the resulting string overflows 
    /// the maximum allowed length (<see cref="int.MaxValue"/>).</exception>
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
    /// <exception cref="OutOfMemoryException">The length of the resulting string overflows the 
    /// maximum allowed length (<see cref="int.MaxValue"/>).</exception>
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
}

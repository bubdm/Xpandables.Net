
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net
{
    /// <summary>
    /// Converts an <see cref="OperationResult"/> to JSON, just does nothing because there is no value to be converted.
    /// </summary>
    public sealed class OperationResultConverter : JsonConverter<IOperationResult>
    {
        /// <summary>
        /// Reads and converts the JSON to type T.
        /// Not concerned, use with caution.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options"> An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override IOperationResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = JsonSerializer.Deserialize(ref reader, typeToConvert, options);
            return result is null
                ? new FailureOperationResult()
                : new SuccessOperationResult();
        }
        /// <summary>
        /// Writes a <see cref="OperationResult"/> value as JSON : does nothing.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, IOperationResult value, JsonSerializerOptions options) { }
    }

    /// <summary>
    /// Converts an <see cref="OperationResult{TValue}"/> to JSON using only the <see cref="OperationResult{TValue}.Value"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class OperationResultConverter<TValue> : JsonConverter<IOperationResult<TValue>>
    {
        /// <summary>
        /// determines whether to handler null value.
        /// </summary>
        public override bool HandleNull => false;

        /// <summary>
        /// Reads and converts the JSON to type T.
        /// Not concerned, use with caution.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options"> An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override IOperationResult<TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = JsonSerializer.Deserialize(ref reader, typeToConvert, options);
            return result is null
                ? new FailureOperationResult<TValue>()
                : new SuccessOperationResult<TValue>((TValue)result);
        }

        /// <summary>
        /// Writes a <see cref="OperationResult{TValue}"/> value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, IOperationResult<TValue> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, typeof(TValue), options);
        }
    }
}

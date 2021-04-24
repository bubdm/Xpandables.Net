
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
    /// Supports converting <see cref="EnumerationType"/> using the <see cref="EnumerationTypeJsonConverter{TEnumerationType}"/>.
    /// </summary>
    public sealed class EnumerationTypeJsonConverterFactory : JsonConverterFactory
    {
        private readonly IInstanceCreator _instanceCreator = new InstanceCreator();

        /// <summary>
        /// Determines whether the converter instance can convert the specified object type.
        /// </summary>
        /// <param name="typeToConvert">The type of the object to check whether it can be converted by this converter instance.</param>
        /// <returns>true if the instance can convert the specified object type; otherwise, false.</returns>
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsSubclassOf(typeof(EnumerationType));

        /// <summary>
        /// Creates a converter for a specified type.
        /// </summary>
        /// <param name="typeToConvert">The type handled by the converter.</param>
        /// <param name="options">The serialization options to use.</param>
        /// <returns> A converter for which <see cref="EnumerationType"/> is compatible with typeToConvert.</returns>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var type = typeof(EnumerationTypeJsonConverter<>).MakeGenericType(typeToConvert);
            return _instanceCreator.Create(type) as JsonConverter;
        }
    }

    /// <summary>
    /// Converts an <see cref="EnumerationType"/> to/or from JSON using only the <see cref="EnumerationType.Name"/>.
    /// </summary>
    /// <typeparam name="TEnumerationType">The type of the enumeration type.</typeparam>
    public sealed class EnumerationTypeJsonConverter<TEnumerationType> : JsonConverter<TEnumerationType>
        where TEnumerationType : EnumerationType
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
        public override TEnumerationType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var name = JsonSerializer.Deserialize(ref reader, typeof(string), options) as string;
            if (name is not null)
            {
                var enumerationType = EnumerationType.FromName(typeToConvert, name);
                if (enumerationType is not null)
                    return (TEnumerationType)enumerationType;
            }

            return default;
        }

        /// <summary>
        /// Writes a <typeparamref name="TEnumerationType"/> value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, TEnumerationType value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Name, typeof(string), options);
        }
    }
}

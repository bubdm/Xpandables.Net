
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net;

/// <summary>
/// Allows interface conversion using <see cref="System.Text.Json"/>.
/// </summary>
/// <typeparam name="TInterface">The type of interface.</typeparam>
public sealed class JsonInterfaceConverter<TInterface> : JsonConverter<TInterface>
    where TInterface : class
{
    ///<inheritdoc/>
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize(ref reader, typeToConvert, options) as TInterface;
    }

    ///<inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, default(TInterface), options);
                break;
            default:
                var type = value.GetType();
                JsonSerializer.Serialize(writer, value, type, options);
                break;
        }
    }
}

/// <summary>
/// When placed on an interface or type, specifies the converter type to use : 
/// <see langword="typeof(JsonInterfaceConverter{TInterface})"/> where TInterface is the decorated interface.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
public class JsonInterfaceConverterAttribute : JsonConverterAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="JsonInterfaceConverterAttribute"/>
    /// with the specified converter type.
    /// </summary>
    /// <param name="converterType">The type of the converter.</param>
    public JsonInterfaceConverterAttribute(Type converterType)
        : base(converterType) { }
}

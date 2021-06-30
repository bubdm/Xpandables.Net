
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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xpandables.Net
{
    /// <summary>
    /// Supports converting <see cref="OperationResult"/> and <see cref="OperationResult{TValue}"/> using the appropriate converter.
    /// </summary>
    public sealed class OperationResultConverterFactory : JsonConverterFactory
    {
        private readonly IInstanceCreator _instanceCreator = new InstanceCreator();

        /// <summary>
        /// Determines whether the converter instance can convert the specified object type.
        /// </summary>
        /// <param name="typeToConvert">The type of the object to check whether it can be converted by this converter instance.</param>
        /// <returns>true if the instance can convert the specified object type; otherwise, false.</returns>
        public override bool CanConvert(Type typeToConvert) => typeof(IOperationResult).IsAssignableFrom(typeToConvert);

        /// <summary>
        /// Creates a converter for a specified type.
        /// </summary>
        /// <param name="typeToConvert">The type handled by the converter.</param>
        /// <param name="options">The serialization options to use.</param>
        /// <returns> A converter for which <see cref="OperationResult"/> or <see cref="OperationResult{TValue}"/> 
        /// is compatible with typeToConvert.</returns>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!typeToConvert.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IOperationResult<>)))
            {
                return _instanceCreator.Create(typeof(OperationResultConverter)) as OperationResultConverter;
            }
            else
            {
                Type elementType = typeToConvert.GetGenericArguments()[0];
                return _instanceCreator.Create(typeof(OperationResultConverter<>).MakeGenericType(elementType)) as JsonConverter;
            }
        }
    }
}

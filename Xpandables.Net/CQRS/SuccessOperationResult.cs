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

using System.Text.Json.Serialization;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// An <see cref="OperationResult"/> that will produces a <see cref="OperationStatus.Success"/> status.
    /// It is decorated with a <see cref="JsonConverterAttribute"/> with the <see cref="OperationResultJsonConverterFactory"/> type in order to automatically convert instance to nothing as it does not contain a value.
    /// </summary>
    [JsonConverter(typeof(OperationResultJsonConverterFactory))]
    public sealed class SuccessOperationResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult"/> class with <see cref="OperationStatus.Success"/> status.
        /// </summary>
        public SuccessOperationResult() : base(OperationStatus.Success) { }
    }

    /// <summary>
    /// An <see cref="OperationResult{TValue}"/> that will produces a <see cref="OperationStatus.Success"/> status with a value of generic type.
    /// It is decorated with a <see cref="JsonConverterAttribute"/> with the <see cref="OperationResultJsonConverterFactory"/> type in order to automatically convert internal <see langword="Value"/> to JSON.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [JsonConverter(typeof(OperationResultJsonConverterFactory))]
    public sealed class SuccessOperationResult<TValue> : OperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult{TValue}"/> class with <see cref="OperationStatus.Success"/> status and the content value.
        /// </summary>
        /// <param name="value">The operation value.</param>
        public SuccessOperationResult(TValue value) : base(OperationStatus.Success, value) { }
    }
}

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

using System.Net;
using System.Text.Json.Serialization;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// An <see cref="OperationResult"/> that will produces a <see cref="OperationStatus.Success"/> status.
    /// It is decorated with a <see cref="JsonConverterAttribute"/> with the <see cref="OperationResultJsonConverterFactory"/> type in order to automatically convert instance to nothing as it does not contain a value.
    /// </summary>
    /// <remarks>In derived classes, be aware of the fact that <see cref="System.Text.Json"/> does not manage attribute inheritance.
    /// So you have to apply the attribute to all your derived classes.</remarks>
    [JsonConverter(typeof(OperationResultJsonConverterFactory))]
    public class SuccessOperationResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult"/> class with <see cref="OperationStatus.Success"/> status and <see cref="HttpStatusCode.OK"/> status code.
        /// </summary>
        public SuccessOperationResult() : base(OperationStatus.Success, HttpStatusCode.OK) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult"/> class with <see cref="OperationStatus.Success"/> status.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        public SuccessOperationResult(HttpStatusCode statusCode) : base(OperationStatus.Success, statusCode) { }
    }

    /// <summary>
    /// An <see cref="OperationResult{TValue}"/> that will produces a <see cref="OperationStatus.Success"/> status with a value of generic type.
    /// It is decorated with a <see cref="JsonConverterAttribute"/> with the <see cref="OperationResultJsonConverterFactory"/> type in order to automatically convert internal <see langword="Value"/> to JSON.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <remarks>In derived classes, be aware of the fact that <see cref="System.Text.Json"/> does not manage attribute inheritance.
    /// So you have to apply the attribute to all your derived classes.</remarks>
    [JsonConverter(typeof(OperationResultJsonConverterFactory))]
    public class SuccessOperationResult<TValue> : OperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult{TValue}"/> class with <see cref="OperationStatus.Success"/> status, <see cref="HttpStatusCode.OK"/> status code and the content value.
        /// </summary>
        /// <param name="value">The operation value.</param>
        public SuccessOperationResult(TValue value) : base(OperationStatus.Success, HttpStatusCode.OK, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOperationResult{TValue}"/> class with <see cref="OperationStatus.Success"/> status and the content value.
        /// </summary>
        /// <param name="statusCode">The HTTP operation status code.</param>
        /// <param name="value">The operation value.</param>
        public SuccessOperationResult(HttpStatusCode statusCode, TValue value) : base(OperationStatus.Success, statusCode, value) { }    
    }
}

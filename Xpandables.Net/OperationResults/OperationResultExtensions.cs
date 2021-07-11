
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
using System.Linq;

namespace Xpandables.Net
{
    /// <summary>
    /// <see cref="IOperationResult"/> extensions.
    /// </summary>
    public static class OperationResultExtensions
    {
        /// <summary>
        /// Converts the error collection as a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> that contains values of <see cref="OperationError"/> selected from the collection.</returns>
        public static IDictionary<string, string[]> ToDictionary(this IReadOnlyCollection<OperationError> @this) => @this.ToDictionary(d => d.Key, d => d.ErrorMessages);

        /// <summary>
        /// Converts the enumerable collection of errors to <see cref="OperationErrorCollection"/>.
        /// </summary>
        /// <param name="this">The source collection.</param>
        /// <returns>A <see cref="OperationErrorCollection"/> that contains values of <see cref="OperationError"/> selected from the collection.</returns>
        public static OperationErrorCollection ToOperationCollection(this IEnumerable<OperationError> @this)
            => new OperationErrorCollection(@this.ToArray());

        /// <summary>
        /// Converts the value to a success operation result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The target value to act on.</param>
        /// <returns>A new instance of <see cref="SuccessOperationResult{TValue}"/> that contains the value.</returns>
        public static SuccessOperationResult<TValue> ToSuccessOperationResult<TValue>(this TValue value)
            where TValue : notnull
            => new(value);
    }
}

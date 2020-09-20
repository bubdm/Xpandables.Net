
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
using System.ComponentModel.DataAnnotations;

namespace Xpandables.Net.Types
{
    /// <summary>
    /// Contains a token value.
    /// </summary>
    public sealed class Token : ValueObject
    {
        /// <summary>
        /// Gets the token value.
        /// </summary>
        [Required, DataType(DataType.Text)]
        public string Value { get; }

        /// <summary>
        /// Gets the type of the token : Bearer, Basic....
        /// </summary>
        [Required, DataType(DataType.Text)]
        public string Type { get; }

        /// <summary>
        /// Gets the token expiry.
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public DateTime Expiry { get; }

        /// <summary>
        /// Creates an instance of <see cref="Token"/> with the specified value.
        /// </summary>
        /// <param name="value">The token value.</param>
        /// <param name="type">The token type.</param>
        /// <param name="expiry">The token expiry.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static Token Create(string value, string type, DateTime expiry) => new Token(value, type, expiry);

        /// <summary>
        /// Provides with deconstruction for <see cref="Token"/>.
        /// </summary>
        /// <param name="value">The output token value.</param>
        /// <param name="type">The output token type.</param>
        /// <param name="expiry">The output token expiry date.</param>
        public void Deconstruct(out string value, out string type, out DateTime expiry) => (value, type, expiry) = (Value, Type, Expiry);

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Type;
            yield return Expiry;
        }

        private Token(string value, string type, DateTime expiry)
            => (Value, Type, Expiry) = (value ?? throw new ArgumentNullException(nameof(value)), type ?? throw new ArgumentNullException(nameof(type)), expiry);
    }
}

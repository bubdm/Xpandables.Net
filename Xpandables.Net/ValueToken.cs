
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

namespace Xpandables.Net
{
    /// <summary>
    /// Contains properties for a token.
    /// </summary>
    public readonly struct ValueToken
    {
        /// <summary>
        /// Gets the token value.
        /// </summary>
        public readonly string Value { get; }

        /// <summary>
        /// Gets the type of the token : Bearer, Basic....
        /// </summary>
        [Required, DataType(DataType.Text)]
        public readonly string Type { get; }

        /// <summary>
        /// Gets the token expiry.
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public readonly DateTime Expiry { get; }

        /// <summary>
        /// Gets a collection of custom properties.
        /// </summary>
        [Required]
        public readonly IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Creates an instance of <see cref="ValueToken"/> with the specified value.
        /// </summary>
        /// <param name="value">The token value.</param>
        /// <param name="type">The token type.</param>
        /// <param name="expiry">The token expiry.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public static ValueToken Create(string value, string type, DateTime expiry) => new ValueToken(value, type, expiry, new Dictionary<string, object>());

        /// <summary>
        /// Creates an instance of <see cref="ValueToken"/> with the specified value.
        /// </summary>
        /// <param name="value">The token value.</param>
        /// <param name="type">The token type.</param>
        /// <param name="expiry">The token expiry.</param>
        /// <param name="properties">The collection of custom properties.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public static ValueToken Create(string value, string type, DateTime expiry, IDictionary<string, object> properties) => new ValueToken(value, type, expiry, properties);

        /// <summary>
        /// Provides with deconstruction for <see cref="ValueToken"/>.
        /// </summary>
        /// <param name="value">The output token value.</param>
        /// <param name="type">The output token type.</param>
        /// <param name="expiry">The output token expiry date.</param>
        public void Deconstruct(out string value, out string type, out DateTime expiry) => (value, type, expiry) = (Value, Type, Expiry);

        private ValueToken(string value, string type, DateTime expiry, IDictionary<string, object> properties)
            => (Value, Type, Expiry, Properties)
            = (value ?? throw new ArgumentNullException(nameof(value)), type ?? throw new ArgumentNullException(nameof(type)), expiry, properties ?? throw new ArgumentNullException(nameof(properties)));
    }
}


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

namespace Xpandables.Net.Entities
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
        public string Value { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="Token"/> with the specified value.
        /// </summary>
        /// <param name="value">The token value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static Token Create(string value) => new Token(value);

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private Token(string value)
            => Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}


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
using System.Text;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Contains information for refreshing a token.
    /// </summary>
    public class RefreshToken : ValueObject
    {
        /// <summary>
        /// Gets the refresh token value.
        /// </summary>
        [Required, DataType(DataType.Text)]
        public string Value { get; private set; }

        /// <summary>
        /// Gets the expiry date.
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public DateTime ExpiresOn { get; private set; }

        /// <summary>
        /// Determines whether the current instance is expired in comparison (greater than) to the specified one.
        /// </summary>
        /// <param name="compare">The date time to compare with.</param>
        public bool IsExpired(DateTime compare) => ExpiresOn > compare;

        /// <summary>
        /// Determines whether the current instance matches the specified one.
        /// </summary>
        /// <param name="compare">The value to compare with.</param>
        public bool IsMatch(string compare) => Value == compare;

        /// <summary>
        /// Creates a new instance of <see cref="RefreshToken"/> with specified values.
        /// </summary>
        /// <param name="value">The refresh token value.</param>
        /// <param name="expiresOn">The expiry date.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static RefreshToken Create(string value, DateTime expiresOn) => new RefreshToken(value, expiresOn);

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return ExpiresOn;
        }

        private RefreshToken(string value, DateTime expiresOn)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ExpiresOn = expiresOn;
        }
    }
}

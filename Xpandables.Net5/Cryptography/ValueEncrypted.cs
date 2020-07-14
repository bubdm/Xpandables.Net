
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
using System.ComponentModel;
using System.Diagnostics;

namespace Xpandables.Net5.Cryptography
{
    /// <summary>
    /// Defines a representation of an encrypted value, its key and its salt.
    /// This class uses the <see cref="ValueEncryptedConverter"/> type converter.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Key = {Key}, Value = {Value}, Salt = {Salt}")]
    [TypeConverter(typeof(ValueEncryptedConverter))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed record ValueEncrypted(string Key, string Value, string Salt)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Compares the encrypted value with the specified one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public bool IsEqualTo(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            IStringCryptography cryptography = new StringCryptography(new StringGenerator());
            return cryptography.AreEqual(this, value);
        }

        /// <summary>
        /// Creates a string representation of the <see cref="ValueEncrypted"/>.
        /// </summary>
        public override string ToString() => $"{Key}:{Value}:{Salt}";

        /// <summary>
        /// Creates a string representation of the <see cref="ValueEncrypted"/> using the specified format and provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        /// <exception cref="FormatException">The <paramref name="format"/> is invalid or
        /// the index of a format item is not zero or one.</exception>
        public string ToString(string format, IFormatProvider formatProvider) => string.Format(formatProvider, format, Key, Value, Salt);
    }
}

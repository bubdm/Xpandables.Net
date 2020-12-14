
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Xpandables.Net
{
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1734 // XML comment has a paramref tag, but there is no parameter by that name
#pragma warning disable CS1572 // XML comment has a param tag, but there is no parameter by that name
    /// <summary>
    /// Defines a representation of an encrypted value, its key and its salt used with <see cref="IStringCryptography"/>.
    /// This class uses the <see cref="ValueEncryptedTypeConverter"/> type converter.
    /// <para>Returns a new instance of <see cref="ValueEncrypted"/> with the key and value.</para>
    /// </summary>
    /// <param name="Key">Contains the encryption key.</param>
    /// <param name="Value">Contains the base64 encrypted value.</param>
    /// <param name="Salt">Contains the base64 salt value.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="Key"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Value"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Salt"/> is null.</exception>
    [Serializable]
    [DebuggerDisplay("Key = {Key}, Value = {Value}, Salt = {Salt}")]
    [TypeConverter(typeof(ValueEncryptedTypeConverter))]
    public sealed record ValueEncrypted([Required] string Key, [Required] string Value, [Required] string Salt)
    {
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

        /// <summary>
        /// Compares the encrypted object with the plain text one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="stringCryptography">Contains methods to encrypt and decrypt string.</param>
        /// <param name="compare">The value to compare with.</param>
        /// <returns><see langword="true"/> if equality otherwise <see langword="false"/>.</returns>
        public bool AreEqual(IStringCryptography stringCryptography, string compare) => stringCryptography.AreEqual(this, compare);

        /// <summary>
        /// Implicit converter from <see cref="ValueEncrypted"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="valueEncrypted">The target value to act on.</param>
        public static implicit operator string(ValueEncrypted valueEncrypted) => valueEncrypted.ToString();
    }
}

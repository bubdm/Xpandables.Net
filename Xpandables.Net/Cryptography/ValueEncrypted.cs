
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
using System.ComponentModel;
using System.Diagnostics;

namespace Xpandables.Net.Cryptography
{
    /// <summary>
    /// Defines a representation of an encrypted value, its key and its salt.
    /// This class uses the <see cref="ValueEncryptedTypeConverter"/> type converter.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Key = {Key}, Value = {Value}, Salt = {Salt}")]
    [TypeConverter(typeof(ValueEncryptedTypeConverter))]
    public sealed class ValueEncrypted : ValueObject
    {
        /// <summary>
        /// Returns a new instance of <see cref="ValueEncrypted"/> with the key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The encrypted value.</param>
        /// <param name="salt">The salt value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="salt"/> is null.</exception>
        public ValueEncrypted(string key, string value, string salt)
            => (Key, Value, Salt) =
            (key ?? throw new ArgumentNullException(nameof(key)),
            value ?? throw new ArgumentNullException(nameof(value)),
            salt ?? throw new ArgumentNullException(nameof(salt)));

        /// <summary>
        /// Makes a copy of the value encrypted source.
        /// </summary>
        /// <param name="source">The encrypted value to be copied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public ValueEncrypted(ValueEncrypted source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            (Key, Value, Salt) = (source.Key, source.Value, source.Salt);
        }

        /// <summary>
        /// Provides with deconstruction for <see cref="ValueEncrypted"/>.
        /// </summary>
        /// <param name="key">The output key.</param>
        /// <param name="value">The output value.</param>
        /// <param name="salt">the output salt value.</param>
        public void Deconstruct(out string key, out string value, out string salt) => (key, value, salt) = (Key, Value, Salt);

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Key;
            yield return Value;
            yield return Salt;
        }

        /// <summary>
        /// Compares the encrypted value with the specified one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <param name="stringCryptography">The cryptography instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stringCryptography"/> is null.</exception>
        public bool IsEqualTo(string value, IStringCryptography stringCryptography)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            _ = stringCryptography ?? throw new ArgumentNullException(nameof(stringCryptography));

            return stringCryptography.AreEqual(this, value);
        }

        /// <summary>
        /// Contains the encryption key.
        /// </summary>
        [field: NonSerialized]
        public string Key { get; }

        /// <summary>
        /// Contains the base64 encrypted value.
        /// </summary>
        [field: NonSerialized]
        public string Value { get; }

        /// <summary>
        /// Contains the base64 salt value.
        /// </summary>
        [field: NonSerialized]
        public string Salt { get; }

        /// <summary>
        /// Compares the <see cref="ValueEncrypted"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object? obj) => obj is ValueEncrypted encryptedValue && this == encryptedValue;

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(ValueEncrypted left, ValueEncrypted right) =>
            left is null && right is null || !(left is null) && !(right is null) && left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(ValueEncrypted left, ValueEncrypted right) => !(left == right);

        /// <summary>
        /// Compares <see cref="ValueEncrypted"/> with the value.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(ValueEncrypted other) => other is ValueEncrypted && (Key, Value, Salt) == (other.Key, other.Value, other.Salt);

        /// <summary>
        /// Computes the hash-code for the <see cref="ValueEncrypted"/> instance.
        /// </summary>
        public override int GetHashCode()
            => Key.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ^ Value.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ^ Salt.GetHashCode(StringComparison.InvariantCultureIgnoreCase);

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
        /// Implicit converter from <see cref="ValueEncrypted"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="valueEncrypted">The target value to act on.</param>
        public static implicit operator string(ValueEncrypted valueEncrypted) => valueEncrypted.ToString();
    }
}

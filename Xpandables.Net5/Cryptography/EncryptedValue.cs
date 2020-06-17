
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
using System.ComponentModel;
using System.Diagnostics;

namespace System.Design.Cryptography
{
    /// <summary>
    /// Defines a representation of an encrypted value, its key and its salt.
    /// This class uses the <see cref="EncryptedValueConverter"/> type converter.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Key = {Key}, Value = {Value}, Salt = {Salt}")]
    [TypeConverter(typeof(EncryptedValueConverter))]
    public readonly struct EncryptedValue : IEquatable<EncryptedValue>
    {
        /// <summary>
        /// Returns a new instance of <see cref="EncryptedValue"/> with the key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The encrypted value.</param>
        /// <param name="salt">The salt value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="salt"/> is null.</exception>
        public EncryptedValue(string key, string value, string salt)
            => (Key, Value, Salt) =
            (key ?? throw new ArgumentNullException(nameof(key)),
            value ?? throw new ArgumentNullException(nameof(value)),
            salt ?? throw new ArgumentNullException(nameof(salt)));

        /// <summary>
        /// Provides with deconstruction for <see cref="EncryptedValue"/>.
        /// </summary>
        /// <param name="key">The output key.</param>
        /// <param name="value">The output value.</param>
        /// <param name="salt">the ouput salt value.</param>
        public void Deconstruct(out string key, out string value, out string salt) => (key, value, salt) = (Key, Value, Salt);

        /// <summary>
        /// Contains the encryption key.
        /// </summary>
        [field: NonSerialized]
        public readonly string Key { get; }

        /// <summary>
        /// Contains the base64 encrypted value.
        /// </summary>
        [field: NonSerialized]
        public readonly string Value { get; }

        /// <summary>
        /// Contains the base64 salt value.
        /// </summary>
        [field: NonSerialized]
        public readonly string Salt { get; }

        /// <summary>
        /// Compares the <see cref="EncryptedValue"/> with other object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        public override bool Equals(object? obj) => (obj is EncryptedValue encryptedValue) && this == encryptedValue;

        /// <summary>
        /// Computes the hash-code for the <see cref="EncryptedValue"/> instance.
        /// </summary>
        public override int GetHashCode()
            => Key.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ^ Value.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ^ Salt.GetHashCode(StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Applies equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(EncryptedValue left, EncryptedValue right) => left.Equals(right);

        /// <summary>
        /// Applies non equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(EncryptedValue left, EncryptedValue right) => !(left == right);

        /// <summary>
        /// Compares <see cref="EncryptedValue"/> with the value.
        /// </summary>
        /// <param name="other">Option to compare with.</param>
        public bool Equals(EncryptedValue other) => (Key, Value, Salt) == (other.Key, other.Value, other.Salt);

        /// <summary>
        /// Creates a string representation of the <see cref="EncryptedValue"/>.
        /// </summary>
        public readonly override string ToString() => $"{Key}:{Value}:{Salt}";

        /// <summary>
        /// Creates a string representation of the <see cref="EncryptedValue"/> using the specified format and provider.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="format"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="formatProvider"/> is null.</exception>
        /// <exception cref="FormatException">The <paramref name="format"/> is invalid or
        /// the index of a format item is not zero or one.</exception>
        public readonly string ToString(string format, IFormatProvider formatProvider)
            => string.Format(formatProvider, format, Key, Value, Salt);
    }
}

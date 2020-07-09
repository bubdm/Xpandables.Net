
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
using System.Security.Cryptography;
using System.Text;

namespace Xpandables.Net5.Cryptography
{
    /// <summary>
    /// Provides with helper methods for <see cref="ValueEncrypted"/>.
    /// </summary>
    public static class ValueEncryptedHelpers
    {
        /// <summary>
        /// Returns an encrypted string from the value using the specified key.
        /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
        /// </summary>
        /// <param name="source">The value to be encrypted.</param>
        /// <param name="key">The key value to be used for encryption.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="EncoderFallbackException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        public static string Encrypt(this string source, string key)
        {
            if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            IStringCryptography cryptography = new StringCryptography(new StringGenerator());
            return cryptography.Encrypt(source, key).Value;
        }

        /// <summary>
        /// Generates a string of the specified length that contains random characters from the lookup characters.
        /// <para>The implementation uses the <see cref="RNGCryptoServiceProvider"/>.</para>
        /// </summary>
        /// <param name="lookupChar">The string to be used to pick characters from.</param>
        /// <param name="length">The length of the expected string value.</param>
        /// <returns>A new string of the specified length with random characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length"/> is lower or equal to zero.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="lookupChar"/> is null.</exception>
        public static string GenerateString(this string lookupChar, int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (string.IsNullOrWhiteSpace(lookupChar)) throw new ArgumentNullException(nameof(lookupChar));

            IStringGenerator stringGenerator = new StringGenerator();
            return stringGenerator.Generate(length, lookupChar);
        }

        /// <summary>
        /// Returns an encrypted string from the value using a randomize key.
        /// <para>The implementation uses the <see cref="SHA512Managed"/>.</para>
        /// </summary>
        /// <param name="source">The value to be encrypted.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static ValueEncrypted Encrypt(this string source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            IStringCryptography cryptography = new StringCryptography(new StringGenerator());
            return cryptography.Encrypt(source, default, default);
        }

        /// <summary>
        /// Compares the encrypted value with the specified one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="encrypted">The encrypted value.</param>
        /// <param name="value">The value to compare with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        public static bool IsEqualTo(this ValueEncrypted encrypted, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            IStringCryptography cryptography = new StringCryptography(new StringGenerator());
            return cryptography.AreEqual(encrypted, value);
        }
    }
}

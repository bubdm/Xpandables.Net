
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

namespace Xpandables.Net.Cryptography
{
    /// <summary>
    /// Provides with methods to generate strings, encrypt and decrypt string values.
    /// Contains a default implementation.
    /// </summary>
    public interface IStringGenerator
    {
        /// <summary>
        /// The lookup characters used to generate random string.
        /// </summary>
        public const string LookupCharacters = "abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,;!(-è_çàà)=@%µ£¨//?§/.?";

        /// <summary>
        /// Generates a string of the specified length that contains random characters from the lookup characters.
        /// <para>The implementation uses the <see cref="RNGCryptoServiceProvider"/>.</para>
        /// </summary>
        /// <param name="length">The length of the expected string value.</param>
        /// <param name="lookupCharacters">The string to be used to pick characters from or default one.</param>
        /// <returns>A new string of the specified length with random characters.</returns>
        /// <exception cref="ArgumentException">The <paramref name="length"/> must be greater than zero
        /// and lower or equal to <see cref="ushort.MaxValue"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="lookupCharacters"/> is null.</exception>
        public string Generate(ushort length, string lookupCharacters = LookupCharacters)
        {
            if (length == 0) throw new ArgumentException($"{nameof(length)} must be greater than zero and lower or equal to {ushort.MaxValue}");
            if (string.IsNullOrWhiteSpace(lookupCharacters)) throw new ArgumentNullException(nameof(lookupCharacters));

            var stringResult = new StringBuilder(length);
            using (var random = new RNGCryptoServiceProvider())
            {
                var count = (int)Math.Ceiling(Math.Log(lookupCharacters.Length, 2) / 8.0);
                System.Diagnostics.Debug.Assert(count <= sizeof(uint));

                var offset = BitConverter.IsLittleEndian ? 0 : sizeof(uint) - count;
                var max = (int)(Math.Pow(2, count * 8) / lookupCharacters.Length) * lookupCharacters.Length;

                var uintBuffer = new byte[sizeof(uint)];
                while (stringResult.Length < length)
                {
                    random.GetBytes(uintBuffer, offset, count);
                    var number = BitConverter.ToUInt32(uintBuffer, 0);
                    if (number < max)
                        stringResult.Append(lookupCharacters[(int)(number % lookupCharacters.Length)]);
                }
            }

            return stringResult.ToString();
        }

        /// <summary>
        /// Generates a salt base64 string of the specified byte length.
        /// <para>The implementation uses the <see cref="RNGCryptoServiceProvider"/>.</para>
        /// </summary>
        /// <param name="length">The length of the expected string value.</param>
        /// <returns>A new base64 string from the salt bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="length"/> must be greater than zero
        /// and lower or equal to <see cref="ushort.MaxValue"/>.</exception>
        /// <exception cref="InvalidOperationException">Generating the salt failed. See inner exception.</exception>
        public string GenerateSalt(ushort length = 32)
        {
            if (length == 0) throw new ArgumentException($"{nameof(length)} must be greater than zero and lower or equal to {ushort.MaxValue}");

            try
            {
                var salt = new byte[length];
                using var random = new RNGCryptoServiceProvider();
                random.GetNonZeroBytes(salt);

                return Convert.ToBase64String(salt);
            }
            catch (Exception exception) when (exception is CryptographicException)
            {
                throw new InvalidOperationException($"{nameof(GenerateSalt)} : Generating salt failed. See inner exception.", exception);
            }
        }

        /// <summary>
        /// Generates a string of the specified length that contains random characters.
        /// <para>The implementation uses the <see cref="RNGCryptoServiceProvider"/>.</para>
        /// </summary>
        /// <param name="length">The length of the expected string value.</param>
        /// <returns>A new string of the specified length with random characters.</returns>
        /// <exception cref="ArgumentException">The <paramref name="length"/> must be greater than zero
        /// and lower or equal to <see cref="ushort.MaxValue"/>.</exception>
        public string Generate(ushort length) => Generate(length, LookupCharacters);
    }

    /// <summary>
    /// Provides with <see cref="IStringGenerator"/> implementation.
    /// </summary>
    public class StringGenerator : IStringGenerator { }
}
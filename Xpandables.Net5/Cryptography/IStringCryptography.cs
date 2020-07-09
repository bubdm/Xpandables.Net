
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
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Xpandables.Net5.Cryptography
{
    /// <summary>
    /// Provides with methods to generate strings, encrypt and decrypt string values.
    /// Contains a default implementation.
    /// </summary>
    public interface IStringCryptography
    {
        /// <summary>
        /// Returns an encrypted string from the value using a randomize key.
        /// The process uses the <see cref="RijndaelManaged"/> algorithm with the <see cref="SHA256"/>.
        /// </summary>
        /// <param name="value">The value to be encrypted.</param>
        /// <param name="keySize">The size of the string key to be used to encrypt the string value.</param>
        /// <returns>An encrypted object that contains the encrypted value and its key.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="keySize"/> is lower than zero and greater than <see cref="byte.MaxValue"/>.</exception>
        /// <exception cref="InvalidOperationException">The encryption failed. See inner exception.</exception>
        ValueEncrypted EncryptShort(string value, int keySize = 12);

        /// <summary>
        /// Returns an encrypted string from the value string using the specified key and the salt value.
        /// If <paramref name="key"/> or <paramref name="salt"/> is not provided, a default value will be used.
        /// The process uses the <see cref="RijndaelManaged"/> algorithm with the <see cref="SHA256"/>.
        /// </summary>
        /// <param name="value">The value to be encrypted.</param>
        /// <param name="key">The optional key value to be used for encryption.</param>
        /// <param name="salt">The optional salt base64 string value to be used for encryption.</param>
        /// <returns>An encrypted object that contains the encrypted value, its key and its salt.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The encryption failed. See inner exception.</exception>
        ValueEncrypted Encrypt(string value, string? key = default, string? salt = default);

        /// <summary>
        /// Returns an decrypted string from the encrypted value.
        /// The process uses the <see cref="RijndaelManaged"/> algorithm with the <see cref="SHA256"/>.
        /// </summary>
        /// <param name="key">The key value to be used for decryption.</param>
        /// <param name="value">The base64 encrypted value to be decrypted.</param>
        /// <param name="salt">The salt base64 string value to be used for decryption.</param>
        /// <returns>A decrypted string from the encrypted object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="salt"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The decryption failed. See inner exception.</exception>
        string Decrypt(string key, string value, string salt) => Decrypt(new ValueEncrypted(key, value, salt));

        /// <summary>
        /// Returns an decrypted string from the encrypted object.
        /// The process uses the <see cref="RijndaelManaged"/> algorithm with the <see cref="SHA256"/>.
        /// </summary>
        /// <param name="encrypted">The object that contains encrypted information.</param>
        /// <returns>A decrypted string from the encrypted object.</returns>
        /// <exception cref="InvalidOperationException">The decryption failed. See inner exception.</exception>
        string Decrypt(ValueEncrypted encrypted)
        {
            try
            {
                var toBeDecrypted = Convert.FromBase64String(encrypted.Value);
                var keyBytes = Encoding.UTF8.GetBytes(encrypted.Key);
                var saltBytes = Convert.FromBase64String(encrypted.Salt);

                using var sha256 = SHA256.Create();
                keyBytes = sha256.ComputeHash(keyBytes);
                string decryptedString;

                using (var memoryStream = new MemoryStream())
                {
                    using var rijndaelManaged = new RijndaelManaged();
                    using var rfcKey = new Rfc2898DeriveBytes(keyBytes, saltBytes, 1000, HashAlgorithmName.SHA256);

                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    rijndaelManaged.Key = rfcKey.GetBytes(rijndaelManaged.KeySize / 8);
                    rijndaelManaged.IV = rfcKey.GetBytes(rijndaelManaged.BlockSize / 8);
                    rijndaelManaged.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(toBeDecrypted, 0, toBeDecrypted.Length);
                    }

                    var decrypted = memoryStream.ToArray();
                    decryptedString = Encoding.UTF8.GetString(decrypted);
                }

                return decryptedString;
            }
            catch (Exception exception) when (exception is EncoderFallbackException
                                                || exception is DecoderFallbackException
                                                || exception is FormatException
                                                || exception is ObjectDisposedException
                                                || exception is ArgumentException
                                                || exception is ArgumentOutOfRangeException
                                                || exception is NotSupportedException
                                                || exception is TargetInvocationException)
            {
                throw new InvalidOperationException($"{nameof(Decrypt)} : decryption failed. See inner exception.", exception);
            }
        }

        /// <summary>
        /// Compares the encrypted object with the plain text one.
        /// Returns <see langword="true"/> if equality otherwise <see langword="false"/>.
        /// </summary>
        /// <param name="encrypted">The encrypted object.</param>
        /// <param name="value">The value to compare with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The comparison failed. See inner exception.</exception>
        bool AreEqual(ValueEncrypted encrypted, string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            var comp = Encrypt(value, encrypted.Key, encrypted.Salt);
            return comp == encrypted;
        }
    }
}
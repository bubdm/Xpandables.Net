
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
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace System.Design.Cryptography
{
    public sealed class StringCryptography : IStringCryptography
    {
        private readonly IStringGenerator _stringGenerator;

        /// <summary>
        /// Initializes a new instance of <see cref="StringCryptography"/> class.
        /// </summary>
        /// <param name="stringGenerator">The string generator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stringGenerator"/> is null.</exception>
        public StringCryptography(IStringGenerator stringGenerator)
            => _stringGenerator = stringGenerator ?? throw new ArgumentNullException(nameof(stringGenerator));

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
        public EncryptedValue EncryptShort(string value, int keySize = 12)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (keySize < 1 || keySize > byte.MaxValue)
                throw new ArgumentOutOfRangeException($"{keySize} must be between 1 and {byte.MaxValue}.");

            var key = _stringGenerator.Generate(keySize).ToLower();
            var salt = _stringGenerator.GenerateSalt();
            return Encrypt(value, key, salt);
        }

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
        public EncryptedValue Encrypt(string value, string? key = default, string? salt = default)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            key ??= _stringGenerator.Generate(12).ToLower();
            salt ??= _stringGenerator.GenerateSalt().ToLower();

            try
            {
                var toBeEncrypted = Encoding.UTF8.GetBytes(value);
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var saltBytes = Convert.FromBase64String(salt);

                keyBytes = SHA256.Create().ComputeHash(keyBytes);
                string encryptedString;

                using (var memoryStream = new MemoryStream())
                {
                    using var rijndaelManaged = new RijndaelManaged();
                    var rfcKey = new Rfc2898DeriveBytes(keyBytes, saltBytes, 1000);

                    rijndaelManaged.Padding = PaddingMode.PKCS7;
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    rijndaelManaged.Key = rfcKey.GetBytes(rijndaelManaged.KeySize / 8);
                    rijndaelManaged.IV = rfcKey.GetBytes(rijndaelManaged.BlockSize / 8);
                    rijndaelManaged.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(toBeEncrypted, 0, toBeEncrypted.Length);
                    }

                    var encrypted = memoryStream.ToArray();
                    encryptedString = Convert.ToBase64String(encrypted);
                }

                return new EncryptedValue(key, encryptedString, salt);
            }
            catch (Exception exception) when (exception is EncoderFallbackException
                                                  || exception is ObjectDisposedException
                                                  || exception is ArgumentException
                                                  || exception is ArgumentOutOfRangeException
                                                  || exception is NotSupportedException
                                                  || exception is TargetInvocationException)
            {
                throw new InvalidOperationException($"{nameof(Encrypt)} : encryption failed. See inner exception.", exception);
            }
        }
    }
}
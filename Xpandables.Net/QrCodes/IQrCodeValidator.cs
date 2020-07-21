
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
using System.Text;

namespace Xpandables.Net.QrCodes
{
    /// <summary>
    /// Provides a method for validating qr-code.
    /// </summary>
    public interface IQrCodeValidator
    {
        /// <summary>
        /// Validates that the specified qr-code matches the expected requirement.
        /// </summary>
        /// <param name="textCode">Th qr-code to be validated.</param>
        /// <exception cref="ArgumentException">The <paramref name="textCode"/> is not valid.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="textCode"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public bool IsValid(string textCode)
        {
            _ = textCode ?? throw new ArgumentNullException(nameof(textCode));

            try
            {
                var part = textCode.Substring(0, 8);
                var crcSource = textCode.Substring(8);
                var crc = new SecurityChecker().ComputeChecksum(Encoding.ASCII.GetBytes(part));

                return IQrCodeTextGenerator.Normalize(crc) == crcSource;
            }
            catch (Exception exception) when (!(exception is ArgumentException))
            {
                throw new InvalidOperationException("Qr-Code validation failed. See inner exception.", exception);
            }
        }
    }
}
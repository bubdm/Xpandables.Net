
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
using System.Globalization;
using System.Text;

namespace Xpandables.Net5.QrCodes
{
    /// <summary>
    /// Provides a method to generate qr-codes.
    /// </summary>
    public interface IQrCodeTextGenerator
    {
        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="count">The number of qr-codes to be generates. Must be greater or equal to 1.</param>
        /// <param name="previous">The previous qr-code to be used.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="ArgumentException">The <paramref name="previous"/> is not valid previous index.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        IEnumerable<string> Generate(uint count, string? previous);

        /// <summary>
        /// Defines the generator delegate to be used to generate qr-code text.
        /// </summary>
        /// <param name="generatorDelegate">The generator delegate to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="generatorDelegate"/> is null.</exception>
        /// <returns>The current instance of the service with the new text generator.</returns>
        IQrCodeTextGenerator UseQrTextGenerator(QrTextGeneratorDelegate generatorDelegate);

        /// <summary>
        /// Generates a new code from the specified index.
        /// </summary>
        /// <param name="index">The index to be used.</param>
        /// <returns>A new string code.</returns>
        public static string GenerateCode(string? index)
        {
            var part = Normalize(DateTime.Now.Day, 2) + Normalize(DateTime.Now.Month, 2);
            var codeBase = part + index?.Normalize();
            var crc = new SecurityChecker().ComputeChecksum(Encoding.ASCII.GetBytes(codeBase));
            return codeBase + Normalize(crc);
        }

        /// <summary>
        /// Converts the value to string adding the zero to the left side for specified position.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="length">The number of zero to be added.</param>
        /// <returns>A new string.</returns>
        public static string Normalize(int value, int length = 4) => value.ToString(CultureInfo.InvariantCulture).PadLeft(length, '0');
    }

    /// <summary>
    /// Defines the delegate used to generate a text for qr-code.
    /// </summary>
    /// <param name="previous"></param>
#pragma warning disable ET001 // Type name does not match file name
    public delegate string QrTextGeneratorDelegate(string? previous);
#pragma warning restore ET001 // Type name does not match file name
}
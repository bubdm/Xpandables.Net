
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
using System.Collections.Generic;
using System.Linq;

namespace System.Design.QrCode
{
    /// <inheritdoc />
    /// <summary>
    /// Generates qr-codes.
    /// </summary>
    public sealed class QrCodeTextGenerator : IQrCodeTextGenerator
    {
        private QrTextGeneratorDelegate qrCodeTextGenerator = IQrCodeTextGenerator.GenerateCode;

        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="count">The number of qr-codes to be generates. Must be greater or equal to 1.</param>
        /// <param name="previous">The previous qr-code to be used.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="ArgumentException">The <paramref name="previous" /> is not valid previous index.</exception>
        /// <exception cref="InvalidOperationException">The operation failed. See inner exception.</exception>
        public IEnumerable<string> Generate(uint count, string? previous)
        {
            var qrCode = previous ?? "0";
            var number = (int)count;
            foreach (var unused in Enumerable.Range(1, number))
            {
                qrCode = qrCodeTextGenerator(qrCode);
                yield return qrCode;
            }
        }

        /// <summary>
        /// Defines the generator delegate to be used to generate qr-code text.
        /// </summary>
        /// <param name="generatorDelegate">The generator delegate to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="generatorDelegate" /> is null.</exception>
        /// <returns>The current instance of the service with the new text generator.</returns>
        public IQrCodeTextGenerator UseQrTextGenerator(QrTextGeneratorDelegate generatorDelegate)
        {
            qrCodeTextGenerator = generatorDelegate;
            return this;
        }
    }
}
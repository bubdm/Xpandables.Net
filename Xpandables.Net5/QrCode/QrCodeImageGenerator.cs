
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

namespace System.Design.QrCode
{
    /// <inheritdoc />
    /// <summary>
    /// Generates qr-code image.
    /// You must derive from this class to implement a custom generator that match your requirement.
    /// </summary>
    public class QrCodeImageGenerator : IQrCodeImageGenerator
    {
        private readonly QRCodeGenerator _qRCodeGenerator = new QRCodeGenerator();

        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="textCodeList">The list of qr-codes text to generate image.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="InvalidOperationException">Generating images failed. See inner exception.</exception>
        public virtual IEnumerable<byte[]> Generate(IEnumerable<string> textCodeList)
        {
            foreach (var source in textCodeList)
            {
                using var qrCodeData = _qRCodeGenerator.CreateQrCode(source, QRCodeGenerator.ECCLevel.Q, true);
                using var qrCode = new QrCode(qrCodeData);
                using var image = qrCode.GetGraphic(20);
                yield return image.ToByte();
            }
        }
    }
}
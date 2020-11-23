
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
using System.Drawing;

using Xpandables.Net.QrCodes.Presenter;

namespace Xpandables.Net.QrCodes
{
    /// <inheritdoc />
    /// <summary>
    /// Generates qr-code image.
    /// </summary>
    public sealed class QrCodeImageGenerator : IDisposable, IQrCodeImageGenerator
    {
        private readonly QRCodeGenerator _qRCodeGenerator = new();

        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="textCodeList">The list of qr-codes text to generate image.</param>
        /// <param name="pixelsPerModule">The pixels per module.</param>
        /// <param name="darkColor">The dark color of the qr-code.</param>
        /// <param name="lightColor">The light color of the qr-code.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="InvalidOperationException">Generating images failed. See inner exception.</exception>
        public IEnumerable<byte[]> Generate(IEnumerable<string> textCodeList, Color darkColor, Color lightColor, int pixelsPerModule = 20)
        {
            foreach (var source in textCodeList)
            {
                using var qrCodeData = _qRCodeGenerator.CreateQrCode(source, QRCodeGenerator.ECCLevel.Q, true);
                using var qrCode = new QrCode(qrCodeData);
                using var image = qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, true);
                yield return image.ToByte();
            }
        }

        /// <summary>
        /// Implementation of Dispose pattern.
        /// </summary>

        public void Dispose() => _qRCodeGenerator?.Dispose();
    }
}
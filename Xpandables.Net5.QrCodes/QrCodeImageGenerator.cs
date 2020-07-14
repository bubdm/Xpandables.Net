
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

using Xpandables.Net5.QrCodes;
using Xpandables.Net5.QrCodes.Presenter;

namespace Xpandables.Net5.QrCodes
{
    /// <inheritdoc />
    /// <summary>
    /// Generates qr-code image.
    /// You must derive from this class to implement a custom generator that match your requirement.
    /// </summary>
    public class QrCodeImageGenerator : Disposable, IQrCodeImageGenerator
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
#pragma warning disable CA2000 // Dispose objects before losing scope
                using var qrCode = new QrCode(qrCodeData);
                using var image = qrCode.GetGraphic(20);
                yield return image.ToByte();
            }
        }

        private bool _isDosposed;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected override void Dispose(bool disposing)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (_isDosposed) return;
            if (disposing)
                _qRCodeGenerator?.Dispose();

            _isDosposed = true;
            base.Dispose(disposing);
        }
    }
}
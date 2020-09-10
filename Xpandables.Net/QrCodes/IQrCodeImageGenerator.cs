
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
using System.Linq;

using Xpandables.Net.Enumerables;

namespace Xpandables.Net.QrCodes
{
    /// <summary>
    /// Provides methods to generate qr-code images.
    /// </summary>
    public interface IQrCodeImageGenerator : IDisposable
    {
        /// <summary>
        /// Generates a list of qr-codes.
        /// </summary>
        /// <param name="textCodeList">The list of qr-codes text to generate image.</param>
        /// <returns>A new list of qr-codes</returns>
        /// <exception cref="InvalidOperationException">Generating images failed. See inner exception.</exception>
        public IEnumerable<byte[]> Generate(IEnumerable<string> textCodeList) => Generate(textCodeList, Color.Black, Color.White, 20);

        /// <summary>
        /// Generates a qr-code.
        /// </summary>
        /// <param name="textCode">The qr-code text to generate image.</param>
        /// <returns>A new qr-code</returns>
        /// <exception cref="InvalidOperationException">Generating image failed. See inner exception.</exception>
        public byte[] Generate(string textCode) => Generate(textCode, Color.Black, Color.White, 20);

        /// <summary>
        /// Generates a qr-code.
        /// </summary>
        /// <param name="textCode">The qr-code text to generate image.</param>
        /// <param name="pixelsPerModule">The pixels per module.</param>
        /// <param name="darkColor">The dark color of the qr-code.</param>
        /// <param name="lightColor">The light color of the qr-code.</param>
        /// <returns>A new qr-code</returns>
        /// <exception cref="InvalidOperationException">Generating image failed. See inner exception.</exception>
        public byte[] Generate(string textCode, Color darkColor, Color lightColor, int pixelsPerModule = 20)
            => Generate(textCode.SingleToEnumerable(), darkColor, lightColor).First();

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
            => Enumerable.Empty<byte[]>();
    }
}
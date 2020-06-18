
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
using System.Linq;

namespace System.Design.QrCode
{
    /// <summary>
    /// The BitmapByte QrCode definition.
    /// </summary>
    public class BitmapByteQrCode : AbstractQrCode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BitmapByteQrCode" /> class with the QrCode data content.
        /// </summary>
        /// <param name="data">The QrCode data content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="data" /> is null.</exception>
        public BitmapByteQrCode(QrCodeData data) : base(data) { }

        /// <summary>
        /// Returns an array of bytes that contains the resulting QR code as bytes.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        public byte[] GetGraphic(int pixelsPerModule) => GetGraphic(pixelsPerModule, new byte[] { 0x00, 0x00, 0x00 }, new byte[] { 0xFF, 0xFF, 0xFF });

        /// <summary>
        /// Returns an array of bytes that contains the resulting QR code as bytes.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHtmlHex"></param>
        /// <param name="lightColorHtmlHex"></param>
        public byte[] GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex)
            => GetGraphic(pixelsPerModule, HexColorToByteArray(darkColorHtmlHex), HexColorToByteArray(lightColorHtmlHex));

        /// <summary>
        /// Returns an array of bytes that contains the resulting QR code as bytes.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorRgb"></param>
        /// <param name="lightColorRgb"></param>
        public byte[] GetGraphic(int pixelsPerModule, byte[] darkColorRgb, byte[] lightColorRgb)
        {
            var sideLength = this.QrCodeData.ModuleMatrix.Count * pixelsPerModule;

            var moduleDark = darkColorRgb.Reverse();
            var moduleLight = lightColorRgb.Reverse();

            List<byte> bmp = new List<byte>();

            //header
            bmp.AddRange(new byte[] { 0x42, 0x4D, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00 });

            //width
            bmp.AddRange(IntTo4Byte(sideLength));
            //height
            bmp.AddRange(IntTo4Byte(sideLength));

            //header end
            bmp.AddRange(new byte[] { 0x01, 0x00, 0x18, 0x00 });

            //draw qr code
            for (var x = sideLength - 1; x >= 0; x -= pixelsPerModule)
            {
                for (int pm = 0; pm < pixelsPerModule; pm++)
                {
                    for (var y = 0; y < sideLength; y += pixelsPerModule)
                    {
                        var module =
                            this.QrCodeData.ModuleMatrix[((x + pixelsPerModule) / pixelsPerModule) - 1][((y + pixelsPerModule) / pixelsPerModule) - 1];
                        for (int i = 0; i < pixelsPerModule; i++)
                        {
                            bmp.AddRange(module ? moduleDark : moduleLight);
                        }
                    }
                    if (sideLength % 4 != 0)
                    {
                        for (int i = 0; i < sideLength % 4; i++)
                        {
                            bmp.Add(0x00);
                        }
                    }
                }
            }

            //finalize with terminator
            bmp.AddRange(new byte[] { 0x00, 0x00 });

            return bmp.ToArray();
        }

        private byte[] HexColorToByteArray(string colorString)
        {
            if (colorString.StartsWith("#"))
                colorString = colorString.Substring(1);
            byte[] byteColor = new byte[colorString.Length / 2];
            for (int i = 0; i < byteColor.Length; i++)
                byteColor[i] = byte.Parse(colorString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            return byteColor;
        }

        private byte[] IntTo4Byte(int inp)
        {
            byte[] bytes = new byte[2];
            unchecked
            {
                bytes[1] = (byte)(inp >> 8);
                bytes[0] = (byte)(inp);
            }
            return bytes;
        }
    }
}

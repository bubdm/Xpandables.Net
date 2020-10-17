﻿
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Xpandables.Net.QrCodes.Presenter
{
    /// <summary>
    /// The Base64 QrCode definition.
    /// </summary>
    public class Base64QrCode : AbstractQrCode
    {
        private readonly QrCode qr;

        /// <summary>
        /// Initializes a new instance of <see cref="Base64QrCode" /> class with the QrCode data content.
        /// </summary>
        /// <param name="data">The QrCode data content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="data" /> is null.</exception>
        public Base64QrCode(QrCodeData data) : base(data) => qr = new QrCode(data);

        /// <summary>
        /// Set a QRCodeData object that will be used to generate QR code. Used in COM Objects connections
        /// </summary>
        /// <param name="data">Need a QRCodeData object generated by QRCodeGenerator.CreateQrCode()</param>
        public override void SetQRCodeData(QrCodeData data) => qr.SetQRCodeData(data);

        /// <summary>
        /// Returns a strings that contains the resulting QR code as base64 chars.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        public string GetGraphic(int pixelsPerModule) => GetGraphic(pixelsPerModule, Color.Black, Color.White, true);

        /// <summary>
        /// Returns a strings that contains the resulting QR code as base64 chars.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHtmlHex"></param>
        /// <param name="lightColorHtmlHex"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="imgType"></param>
        /// <returns></returns>
        public string GetGraphic(
            int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
            => GetGraphic(pixelsPerModule, ColorTranslator.FromHtml(darkColorHtmlHex), ColorTranslator.FromHtml(lightColorHtmlHex), drawQuietZones, imgType);

        /// <summary>
        /// Returns a strings that contains the resulting QR code as base64 chars.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="imgType"></param>
        /// <returns></returns>
        public string GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, ImageType imgType = ImageType.Png)
        {
            var base64 = string.Empty;
            using (Bitmap bmp = qr.GetGraphic(pixelsPerModule, darkColor, lightColor, drawQuietZones))
                base64 = BitmapToBase64(bmp, imgType);

            return base64;
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as base64 chars.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="icon"></param>
        /// <param name="iconSizePercent"></param>
        /// <param name="iconBorderWidth"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="imgType"></param>
        public string GetGraphic(
            int pixelsPerModule,
            Color darkColor,
            Color lightColor,
            Bitmap icon,
            int iconSizePercent = 15,
            int iconBorderWidth = 6,
            bool drawQuietZones = true,
            ImageType imgType = ImageType.Png)
        {
            var base64 = string.Empty;
            using (Bitmap bmp = qr.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones))
                base64 = BitmapToBase64(bmp, imgType);
            return base64;
        }

#pragma warning disable CA1822 // Mark members as static
        private string BitmapToBase64(Bitmap bmp, ImageType imgType)
#pragma warning restore CA1822 // Mark members as static
        {
            var base64 = string.Empty;
            ImageFormat iFormat = imgType switch
            {
                ImageType.Png => ImageFormat.Png,
                ImageType.Jpeg => ImageFormat.Jpeg,
                ImageType.Gif => ImageFormat.Gif,
                _ => ImageFormat.Png,
            };
            using (var memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, iFormat);
                base64 = Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None);
            }

            return base64;
        }

        /// <summary>
        /// Defines the image type.
        /// </summary>
        public enum ImageType
        {
            /// <summary>
            /// Gif
            /// </summary>
            Gif,

            /// <summary>
            /// JPjeg
            /// </summary>
            Jpeg,

            /// <summary>
            /// Png
            /// </summary>
            Png
        }
    }
}
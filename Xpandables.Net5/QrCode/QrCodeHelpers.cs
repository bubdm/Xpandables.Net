
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static System.Design.QrCode.Base64QrCode;
using static System.Design.QrCode.QRCodeGenerator;
using static System.Design.QrCode.SvgQRCode;

namespace System.Design.QrCode
{
    /// <summary>
    /// Provides with methods to extend use <see cref="QrCode"/>.
    /// </summary>
    public static class QrCodeHelpers
    {
        /// <summary>
        /// Converts the bitmap image to the byte format using the default Jpeg format.
        /// </summary>
        /// <remarks>If error, return default value. See trace listener output for exception.</remarks>
        /// <param name="image">The source image to be converted.</param>
        /// <returns>An array of byte that represents the image.</returns>
        public static byte[] ToByte(this Bitmap image) => ToByte(image, ImageFormat.Jpeg);

        /// <summary>
        /// Converts the bitmap image to the byte format using the specified format.
        /// </summary>
        /// <remarks>If error, return default value. See trace listener output for exception.</remarks>
        /// <param name="image">The source image to be converted.</param>
        /// <param name="imageFormat">The target image format.</param>
        /// <returns>An array of byte that represents the image.</returns>
        public static byte[] ToByte(this Bitmap image, ImageFormat imageFormat)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, imageFormat);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Copies the input stream to the output one.
        /// </summary>
        /// <param name="input">The stream source.</param>
        /// <param name="output">The stream destination</param>
        /// <exception cref="ArgumentNullException">The <paramref name="input"/></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="output"/></exception>
        public static void CoptyTo(Stream input, Stream output)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));
            _ = output ?? throw new ArgumentNullException(nameof(output));

            var buffer = new byte[16 * 1024];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, bytesRead);
        }

        /// <summary>
        /// Returns a <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        /// <param name="icon"></param>
        /// <param name="iconSizePercent"></param>
        /// <param name="iconBorderWidth"></param>
        /// <param name="drawQuietZones"></param>
        public static Bitmap GetQrCode(
            string plainText,
            int pixelsPerModule,
            Color darkColor,
            Color lightColor,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1,
            Bitmap? icon = null,
            int iconSizePercent = 15,
            int iconBorderWidth = 6,
            bool drawQuietZones = true)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new QrCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, icon, iconSizePercent, iconBorderWidth, drawQuietZones);
        }

        /// <summary>
        /// Returns an ASCII <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorString"></param>
        /// <param name="whiteSpaceString"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        /// <param name="endOfLine"></param>
        public static string GetAsciiQrCode(
            string plainText,
            int pixelsPerModule,
            string darkColorString,
            string whiteSpaceString,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1,
            string endOfLine = "\n")
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new AsciiQrCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColorString, whiteSpaceString, endOfLine);
        }

        /// <summary>
        /// Returns an Bas64 <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHtmlHex"></param>
        /// <param name="lightColorHtmlHex"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="imgType"></param>
        public static string GetBase64QrCode(
            string plainText,
            int pixelsPerModule,
            string darkColorHtmlHex,
            string lightColorHtmlHex,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1,
            bool drawQuietZones = true,
            ImageType imgType = ImageType.Png)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new Base64QrCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColorHtmlHex, lightColorHtmlHex, drawQuietZones, imgType);
        }

        /// <summary>
        /// Returns an Svg <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="sizingMode"></param>
        public static string GetSvgQrRCode(
            string plainText,
            int pixelsPerModule,
            string darkColorHex,
            string lightColorHex,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1,
            bool drawQuietZones = true,
            SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new SvgQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
        }

        /// <summary>
        /// Returns a Postscript <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pointsPerModule"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="epsFormat"></param>
        public static string GetPostScriptQrCode(
            string plainText,
            int pointsPerModule,
            string darkColorHex,
            string lightColorHex,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1,
            bool drawQuietZones = true,
            bool epsFormat = false)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new PostScriptQrCode(qrCodeData);
            return qrCode.GetGraphic(pointsPerModule, darkColorHex, lightColorHex, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Returns a Png Byte <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorRgba"></param>
        /// <param name="lightColorRgba"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        public static byte[] GetPngByteQrCode(
            string plainText,
            int pixelsPerModule,
            byte[] darkColorRgba,
            byte[] lightColorRgba,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion);
            using var qrCode = new PngByteQrCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColorRgba, lightColorRgba);
        }

        /// <summary>
        /// /// Returns a Png Byte <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="eccLevel"></param>
        /// <param name="size"></param>
        public static byte[] GetPngByteQrCode(string txt, QRCodeGenerator.ECCLevel eccLevel, int size)
        {
            using var qrGen = new QRCodeGenerator();
            using var qrCode = qrGen.CreateQrCode(txt, eccLevel);
            using var qrPng = new PngByteQrCode(qrCode);
            return qrPng.GetGraphic(size);
        }

        /// <summary>
        /// Returns an Bitmap Byte <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHtmlHex"></param>
        /// <param name="lightColorHtmlHex"></param>
        /// <param name="eccLevel"></param>
        /// <param name="forceUtf8"></param>
        /// <param name="utf8BOM"></param>
        /// <param name="eciMode"></param>
        /// <param name="requestedVersion"></param>
        public static byte[] GetBitmapByteQrCode(
            string plainText,
            int pixelsPerModule,
            string darkColorHtmlHex,
            string lightColorHtmlHex,
            ECCLevel eccLevel,
            bool forceUtf8 = false,
            bool utf8BOM = false,
            EciMode eciMode = EciMode.Default,
            int requestedVersion = -1)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode,
                    requestedVersion);
            using var qrCode = new BitmapByteQrCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule, darkColorHtmlHex, lightColorHtmlHex);
        }

        /// <summary>
        /// Returns a Bitmap Byte <see cref="QrCode"/> using the definition properties.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="eccLevel"></param>
        /// <param name="size"></param>
        public static byte[] GetBitmapByteQrCode(string txt, QRCodeGenerator.ECCLevel eccLevel, int size)
        {
            using var qrGen = new QRCodeGenerator();
            using var qrCode = qrGen.CreateQrCode(txt, eccLevel);
            using var qrBmp = new BitmapByteQrCode(qrCode);
            return qrBmp.GetGraphic(size);
        }
    }
}

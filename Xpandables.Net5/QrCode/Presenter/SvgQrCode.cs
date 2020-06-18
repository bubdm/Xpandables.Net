
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
using System.Text;

namespace System.Design.QrCode
{
    /// <summary>
    /// The SVG QrCode class.
    /// </summary>
    public class SvgQRCode : AbstractQrCode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SvgQRCode" /> class with the QrCode data content.
        /// </summary>
        /// <param name="data">The QrCode data content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="data" /> is null.</exception>
        public SvgQRCode(QrCodeData data) : base(data) { }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as SVG.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        public string GetGraphic(int pixelsPerModule)
        {
            var viewBox = new Size(pixelsPerModule * QrCodeData.ModuleMatrix.Count, pixelsPerModule * QrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, Color.Black, Color.White);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as SVG.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="sizingMode"></param>
        public string GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            var viewBox = new Size(pixelsPerModule * QrCodeData.ModuleMatrix.Count, pixelsPerModule * QrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, sizingMode);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as SVG.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="sizingMode"></param>
        public string GetGraphic(int pixelsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            var viewBox = new Size(pixelsPerModule * QrCodeData.ModuleMatrix.Count, pixelsPerModule * QrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, sizingMode);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as SVG.
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="sizingMode"></param>
        public string GetGraphic(Size viewBox, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            return GetGraphic(viewBox, Color.Black, Color.White, drawQuietZones, sizingMode);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as SVG.
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="sizingMode"></param>
        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            return GetGraphic(viewBox, ColorTranslator.ToHtml(Color.FromArgb(darkColor.ToArgb())), ColorTranslator.ToHtml(Color.FromArgb(lightColor.ToArgb())), drawQuietZones, sizingMode);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as SVG.
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="sizingMode"></param>
        public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true, SizingMode sizingMode = SizingMode.WidthHeightAttribute)
        {
            var offset = drawQuietZones ? 0 : 4;
            var drawableModulesCount = QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
            var pixelsPerModule = (double)Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;
            var qrSize = drawableModulesCount * pixelsPerModule;
            var svgSizeAttributes = (sizingMode == SizingMode.WidthHeightAttribute) ? $@"width=""{viewBox.Width}"" height=""{viewBox.Height}""" : $@"viewBox=""0 0 {viewBox.Width} {viewBox.Height}""";
            var svgFile = new StringBuilder($@"<svg version=""1.1"" baseProfile=""full"" shape-rendering=""crispEdges"" {svgSizeAttributes} xmlns=""http://www.w3.org/2000/svg"">");
            svgFile.Append(@"<rect x=""0"" y=""0"" width=""").Append(CleanSvgVal(qrSize)).Append(@""" height=""").Append(CleanSvgVal(qrSize)).Append(@""" fill=""").Append(lightColorHex).AppendLine(@""" />");
            for (int xi = offset; xi < offset + drawableModulesCount; xi++)
            {
                for (int yi = offset; yi < offset + drawableModulesCount; yi++)
                {
                    if (QrCodeData.ModuleMatrix[yi][xi])
                    {
                        var x = (xi - offset) * pixelsPerModule;
                        var y = (yi - offset) * pixelsPerModule;
                        svgFile.Append(@"<rect x=""").Append(CleanSvgVal(x)).Append(@""" y=""").Append(CleanSvgVal(y)).Append(@""" width=""").Append(CleanSvgVal(pixelsPerModule)).Append(@""" height=""").Append(CleanSvgVal(pixelsPerModule)).Append(@""" fill=""").Append(darkColorHex).AppendLine(@""" />");
                    }
                }
            }
            svgFile.Append("</svg>");
            return svgFile.ToString();
        }

        private string CleanSvgVal(double input)
        {
            //Clean double values for international use/formats
            return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sizing Mode.
        /// </summary>
        public enum SizingMode
        {
            /// <summary>
            /// Width Height
            /// </summary>
            WidthHeightAttribute,

            /// <summary>
            /// ViewBox
            /// </summary>
            ViewBoxAttribute
        }
    }
}

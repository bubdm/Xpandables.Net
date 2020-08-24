
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

namespace Xpandables.Net.QrCodes.Presenter
{
    /// <summary>
    /// The Postscript QrCode
    /// </summary>
    public class PostScriptQrCode : AbstractQrCode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PostScriptQrCode" /> class with the QrCode data content.
        /// </summary>
        /// <param name="data">The QrCode data content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="data" /> is null.</exception>
        public PostScriptQrCode(QrCodeData data) : base(data) { }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as postscript chars.
        /// </summary>
        /// <param name="pointsPerModule"></param>
        /// <param name="epsFormat"></param>
        public string GetGraphic(int pointsPerModule, bool epsFormat = false)
        {
            var viewBox = new Size(pointsPerModule * QrCodeData.ModuleMatrix.Count, pointsPerModule * QrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, Color.Black, Color.White, true, epsFormat);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as postscript chars.
        /// </summary>
        /// <param name="pointsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="epsFormat"></param>
        public string GetGraphic(int pointsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true, bool epsFormat = false)
        {
            var viewBox = new Size(pointsPerModule * QrCodeData.ModuleMatrix.Count, pointsPerModule * QrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, darkColor, lightColor, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as postscript chars.
        /// </summary>
        /// <param name="pointsPerModule"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="epsFormat"></param>
        public string GetGraphic(int pointsPerModule, string darkColorHex, string lightColorHex, bool drawQuietZones = true, bool epsFormat = false)
        {
            var viewBox = new Size(pointsPerModule * QrCodeData.ModuleMatrix.Count, pointsPerModule * QrCodeData.ModuleMatrix.Count);
            return GetGraphic(viewBox, darkColorHex, lightColorHex, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as postscript chars.
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="epsFormat"></param>
        public string GetGraphic(Size viewBox, bool drawQuietZones = true, bool epsFormat = false)
        {
            return GetGraphic(viewBox, Color.Black, Color.White, drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as postscript chars.
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="darkColorHex"></param>
        /// <param name="lightColorHex"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="epsFormat"></param>
        public string GetGraphic(Size viewBox, string darkColorHex, string lightColorHex, bool drawQuietZones = true, bool epsFormat = false)
        {
            return GetGraphic(viewBox, ColorTranslator.FromHtml(darkColorHex), ColorTranslator.FromHtml(lightColorHex), drawQuietZones, epsFormat);
        }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as postscript chars.
        /// </summary>
        /// <param name="viewBox"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="drawQuietZones"></param>
        /// <param name="epsFormat"></param>
        public string GetGraphic(Size viewBox, Color darkColor, Color lightColor, bool drawQuietZones = true, bool epsFormat = false)
        {
            var offset = drawQuietZones ? 0 : 4;
            var drawableModulesCount = QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : offset * 2);
            var pointsPerModule = Math.Min(viewBox.Width, viewBox.Height) / (double)drawableModulesCount;
            string psFile = string.Format(psHeader, new object[] {
                DateTime.Now.ToString("s"), CleanSvgVal(viewBox.Width), CleanSvgVal(pointsPerModule),
                epsFormat ? "EPSF-3.0" : string.Empty
            });
            psFile += string.Format(psFunctions, new object[] {
                CleanSvgVal(darkColor.R /255.0), CleanSvgVal(darkColor.G /255.0), CleanSvgVal(darkColor.B /255.0),
                CleanSvgVal(lightColor.R /255.0), CleanSvgVal(lightColor.G /255.0), CleanSvgVal(lightColor.B /255.0),
                drawableModulesCount
            });
            for (int xi = offset; xi < offset + drawableModulesCount; xi++)
            {
                if (xi > offset)
                    psFile += "nl\n";
                for (int yi = offset; yi < offset + drawableModulesCount; yi++)
                {
                    psFile += QrCodeData.ModuleMatrix[xi][yi] ? "f " : "b ";
                }
                psFile += "\n";
            }
            return psFile + psFooter;
        }

#pragma warning disable CA1822 // Mark members as static
        private string CleanSvgVal(double input)
#pragma warning restore CA1822 // Mark members as static
        {
            //Clean double values for international use/formats
            return input.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private const string psHeader = @"%!PS-Adobe-3.0 {3}
%%Creator: Xpandables.Net
%%Title: QRCode
%%CreationDate: {0}
%%DocumentData: Clean7Bit
%%Origin: 0
%%DocumentMedia: Default {1} {1} 0 () ()
%%BoundingBox: 0 0 {1} {1}
%%LanguageLevel: 2 
%%Pages: 1
%%Page: 1 1
%%EndComments
%%BeginConstants
/sz {1} def
/sc {2} def
%%EndConstants
%%BeginFeature: *PageSize Default
<< /PageSize [ sz sz ] /ImagingBBox null >> setpagedevice
%%EndFeature
";

        private const string psFunctions = @"%%BeginFunctions 
/csquare {{
    newpath
    0 0 moveto
    0 1 rlineto
    1 0 rlineto
    0 -1 rlineto
    closepath
    setrgbcolor
    fill
}} def
/f {{ 
    {0} {1} {2} csquare
    1 0 translate
}} def
/b {{ 
    1 0 translate
}} def 
/background {{ 
    {3} {4} {5} csquare 
}} def
/nl {{
    -{6} -1 translate
}} def
%%EndFunctions
%%BeginBody
0 0 moveto
gsave
sz sz scale
background
grestore
gsave
sc sc scale
0 {6} 1 sub translate
";

        private const string psFooter = @"%%EndBody
grestore
showpage   
%%EOF
";
    }
}

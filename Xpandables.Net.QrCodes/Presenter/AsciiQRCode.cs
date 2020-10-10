
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
using System.Text;

namespace Xpandables.Net.QrCodes.Presenter
{
    /// <summary>
    /// The ASCII QrCode definition.
    /// </summary>
    public class AsciiQrCode : AbstractQrCode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsciiQrCode" /> class with the QrCode data content.
        /// </summary>
        /// <param name="data">The QrCode data content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="data" /> is null.</exception>
        public AsciiQrCode(QrCodeData data) : base(data) { }

        /// <summary>
        /// Returns a strings that contains the resulting QR code as ASCII chars.
        /// </summary>
        /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
        public string GetGraphic(int repeatPerModule) => string.Join("\n", GetLineByLineGraphic(repeatPerModule));

        /// <summary>
        /// Returns a strings that contains the resulting QR code as ASCII chars.
        /// </summary>
        /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
        /// <param name="darkColorString">String for use as dark color modules. In case of string make sure whiteSpaceString has the same length.</param>
        /// <param name="whiteSpaceString">String for use as white modules (whitespace).
        /// In case of string make sure darkColorString has the same length.</param>
        /// <param name="endOfLine">End of line separator. (Default: \n)</param>
        public string GetGraphic(int repeatPerModule, string darkColorString, string whiteSpaceString, string endOfLine = "\n")
            => string.Join(endOfLine, GetLineByLineGraphic(repeatPerModule, darkColorString, whiteSpaceString));

        /// <summary>
        /// Returns an array of strings that contains each line of the resulting QR code as ASCII chars.
        /// </summary>
        /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
        /// <returns></returns>
        public string[] GetLineByLineGraphic(int repeatPerModule) => GetLineByLineGraphic(repeatPerModule, "██", "  ");

        /// <summary>
        /// Returns an array of strings that contains each line of the resulting QR code as ASCII chars.
        /// </summary>
        /// <param name="repeatPerModule">Number of repeated darkColorString/whiteSpaceString per module.</param>
        /// <param name="darkColorString">String for use as dark color modules. In case of string make sure whiteSpaceString has the same length.</param>
        /// <param name="whiteSpaceString">String for use as white modules (whitespace).
        /// In case of string make sure darkColorString has the same length.</param>
        public string[] GetLineByLineGraphic(int repeatPerModule, string darkColorString, string whiteSpaceString)
        {
            var qrCode = new List<string>();

            //We need to adjust the repeatPerModule based on number of characters in darkColorString
            //(we assume whiteSpaceString has the same number of characters)
            //to keep the QR code as square as possible.

            var adjustmentValueForNumberOfCharacters = darkColorString.Length / 2 != 1 ? darkColorString.Length / 2 : 0;
            var verticalNumberOfRepeats = repeatPerModule + adjustmentValueForNumberOfCharacters;
            var sideLength = QrCodeData.ModuleMatrix.Count * verticalNumberOfRepeats;
            for (var y = 0; y < sideLength; y++)
            {
                bool emptyLine = true;
                var lineBuilder = new StringBuilder();

                for (var x = 0; x < QrCodeData.ModuleMatrix.Count; x++)
                {
                    var module = QrCodeData.ModuleMatrix[x][(y + verticalNumberOfRepeats) / verticalNumberOfRepeats - 1];

                    for (var i = 0; i < repeatPerModule; i++)
                    {
                        lineBuilder.Append(module ? darkColorString : whiteSpaceString);
                    }
                    if (module)
                    {
                        emptyLine = false;
                    }
                }

                if (!emptyLine)
                    qrCode.Add(lineBuilder.ToString());
            }

            return qrCode.ToArray();
        }
    }
}

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

namespace System.Design.QrCode
{
    /// <summary>
    /// The <see cref="QrCode"/> exception
    /// </summary>
    public class QrCodeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QrCodeException" /> class with a specified error message.</summary>
        /// <param name="eccLevel">The level of the message.</param>
        /// <param name="encodingMode"></param>
        /// <param name="maxSizeByte"></param>
        public QrCodeException(string eccLevel, string encodingMode, int maxSizeByte) : base(
            $"The given payload exceeds the maximum size of the QR code standard. The maximum size allowed for the choosen paramters (ECC level={eccLevel}, EncodingMode={encodingMode}) is {maxSizeByte} byte."
        )
        { }
    }
}

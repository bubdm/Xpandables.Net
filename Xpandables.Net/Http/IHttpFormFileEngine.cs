
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
using System.Collections.Generic;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with methods to validate an instance of IFormFile.
    /// </summary>
    public interface IHttpFormFileEngine
    {
        /// <summary>
        /// Checks that the HTTP request file name is a valid one.
        /// </summary>
        /// <param name="formFile">The sent file.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        bool IsValidFileName(object formFile);

        /// <summary>
        /// Checks that the HTTP request file extension matches one of the specified extensions.
        /// </summary>
        /// <param name="formFile">The sent file.</param>
        /// <param name="extensions">The extensions to compare to.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        bool IsValidFileExtension(object formFile, string[] extensions);

        /// <summary>
        /// Checks that the HTTP request file size is lower or equal to the specified size.
        /// </summary>
        /// <param name="formFile">The sent file.</param>
        /// <param name="size">The file size to compare to.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        bool IsValidFileSize(object formFile, int size);

        /// <summary>
        /// Checks that the HTTP request file content matches its extension signature.
        /// </summary>
        /// <param name="formFile">the sent file.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        bool IsValidFileContent(object formFile);

        /// <summary>
        /// Checks that the HTTP request file content matches its extension signature.
        /// </summary>
        /// <param name="formFile">the sent file.</param>
        /// <param name="fileSignatures">The file signatures dictionary.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        bool IsValidFileContent(object formFile, IDictionary<string, List<byte[]>> fileSignatures);
    }
}

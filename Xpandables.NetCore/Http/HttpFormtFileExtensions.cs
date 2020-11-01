
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
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with helper methods for validating HTTP request file content.
    /// </summary>
    public static class HttpFormtFileExtensions
    {
        /// <summary>
        /// Checks that the HTTP request file name is a valid one.
        /// </summary>
        /// <param name="formFile">The sent file.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        public static bool IsValidFileName(this IFormFile formFile)
            => !string.IsNullOrWhiteSpace(formFile?.FileName)
                && formFile?.FileName.Length != 0
                && Path.GetFileName(formFile?.FileName) == formFile?.FileName;

        /// <summary>
        /// Checks that the HTTP request file extension matches one of the specified extensions.
        /// </summary>
        /// <param name="formFile">The sent file.</param>
        /// <param name="extensions">The extensions to compare to.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        public static bool IsValidFileExtension(this IFormFile formFile, string[] extensions)
            => !string.IsNullOrWhiteSpace(Path.GetExtension(formFile?.FileName))
                && extensions?.Contains(Path.GetExtension(formFile?.FileName)) == true;

        /// <summary>
        /// Checks that the HTTP request file size is lower or equal to the specified size.
        /// </summary>
        /// <param name="formFile">The sent file.</param>
        /// <param name="size">The file size to compare to.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        public static bool IsValidFileSize(this IFormFile formFile, int size)
            => formFile?.Length <= size;

        /// <summary>
        /// Checks that the HTTP request file content matches its extension signature.
        /// </summary>
        /// <param name="formFile">the sent file.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        public static bool IsValidFileContent(this IFormFile formFile) => formFile.IsValidFileContent(FileContentSignatures);

        /// <summary>
        /// Checks that the HTTP request file content matches extension signatures provided.
        /// </summary>
        /// <param name="formFile">the sent file.</param>
        /// <param name="fileSignatures">The file signatures dictionary.</param>
        /// <returns>Returns <see langword="true"/> is valid, otherwise <see langword="false"/>.</returns>
        public static bool IsValidFileContent(this IFormFile formFile, IDictionary<string, List<byte[]>> fileSignatures)
        {
            _ = formFile ?? throw new ArgumentNullException(nameof(formFile));
            _ = fileSignatures ?? throw new ArgumentNullException(nameof(fileSignatures));

            using var reader = new BinaryReader(formFile.OpenReadStream());
            var signatures = fileSignatures[Path.GetExtension(formFile.FileName)];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

            return !signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }

        /// <summary>
        /// Content a collection of file signatures and extension as key.
        /// </summary>
        public static Dictionary<string, List<byte[]>> FileContentSignatures
            => new Dictionary<string, List<byte[]>>
            {
                {
                    ".jpeg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                    }
                },
                {
                    ".jpg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xF0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xF0 }
                    }
                },
                {
                    ".png",new List<byte[]>
                    {
                        new byte[] { 0x89, 0x50, 0xE, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                    }
                },
                {
                    ".bmp",new List<byte[]>
                    {
                        new byte[] { 0x42, 0x4D }
                    }
                },
                {
                    ".tiff", new List<byte[]>
                    {
                        new byte[] { 0x49, 0x20, 0x49 },
                        new byte[] { 0x49, 0x49, 0x2A, 0x00 },
                        new byte[] { 0x4D, 0x4D, 0x00, 0x2A },
                        new byte[] { 0x4D, 0x4D, 0x00, 0x2B }
                    }
                },
                {
                    ".pdf",new List<byte[]>
                    {
                        new byte[] { 0x25, 0x50, 0x44, 0x46 }
                    }
                }
           };
    }
}

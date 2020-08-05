
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Xpandables.Net.QrCodes.Presenter
{
    /// <summary>
    /// The QrCode Content.
    /// </summary>
    public sealed class QrCodeData : Disposable
    {
        /// <summary>
        /// The module content list.
        /// </summary>
        public List<BitArray> ModuleMatrix { get; } = new List<BitArray>();

        /// <summary>
        /// Gets the data version.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="QrCodeData"/> class with the version.
        /// </summary>
        /// <param name="version"></param>
        public QrCodeData(int version)
        {
            Version = version;
            var size = ModulesPerSideFromVersion(version);
            ModuleMatrix = new List<BitArray>();
            for (var i = 0; i < size; i++)
                ModuleMatrix.Add(new BitArray(size));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="QrCodeData"/> class with content and compression mode.
        /// </summary>
        /// <param name="pathToRawData"></param>
        /// <param name="compressMode"></param>
        public QrCodeData(string pathToRawData, CompressionMode compressMode)
            : this(File.ReadAllBytes(pathToRawData), compressMode) { }

        /// <summary>
        /// Initializes a new instance of <see cref="QrCodeData"/> class with content and compression mode.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="compressionModel"></param>
        public QrCodeData(byte[] rawData, CompressionMode compressionModel)
        {
            var bytes = new List<byte>(rawData);

            if (compressionModel == CompressionMode.Deflate || compressionModel == CompressionMode.GZip)
            {
                using var input = new MemoryStream(bytes.ToArray());
                using var output = new MemoryStream();
                using (var stream = compressionModel == CompressionMode.Deflate
                    ? new DeflateStream(input, System.IO.Compression.CompressionMode.Decompress)
                    : (Stream)new GZipStream(input, System.IO.Compression.CompressionMode.Decompress))
                {
                    QrCodeHelpers.CoptyTo(stream, output);
                }

                bytes = new List<byte>(output.ToArray());
            }

            if (bytes[0] != 0x51 || bytes[1] != 0x52 || bytes[2] != 0x52)
                throw new Exception("Invalid raw data file. Filetype doesn't match \"QRR\".");

            //Set QR code version
            var sideLen = (int)bytes[4];
            bytes.RemoveRange(0, 5);
            Version = (sideLen - 21 - 8) / 4 + 1;

            //Unpack
            var modules = new Queue<bool>(8 * bytes.Count);
            foreach (var b in bytes)
            {
                _ = new BitArray(new byte[] { b });
                for (int i = 7; i >= 0; i--)
                    modules.Enqueue((b & 1 << i) != 0);
            }

            //Build module matrix
            ModuleMatrix = new List<BitArray>(sideLen);
            for (int y = 0; y < sideLen; y++)
            {
                ModuleMatrix.Add(new BitArray(sideLen));
                for (int x = 0; x < sideLen; x++)
                    ModuleMatrix[y][x] = modules.Dequeue();
            }
        }

        /// <summary>
        /// Returns the data content according the compression mode.
        /// </summary>
        /// <param name="compressMode"></param>
        public byte[] GetRawData(CompressionMode compressMode)
        {
            var bytes = new List<byte>();

            //Add header - signature ("QRR")
            bytes.AddRange(new byte[] { 0x51, 0x52, 0x52, 0x00 });

            //Add header - rowsize
            bytes.Add((byte)ModuleMatrix.Count);

            //Build data queue
            var dataQueue = new Queue<int>();
            foreach (var row in ModuleMatrix)
            {
                foreach (var module in row)
                    dataQueue.Enqueue((bool)module! ? 1 : 0);
            }

            for (int i = 0; i < 8 - ModuleMatrix.Count * ModuleMatrix.Count % 8; i++)
                dataQueue.Enqueue(0);

            //Process queue
            while (dataQueue.Count > 0)
            {
                byte b = 0;
                for (int i = 7; i >= 0; i--)
                    b += (byte)(dataQueue.Dequeue() << i);

                bytes.Add(b);
            }

            var rawData = bytes.ToArray();

            //Compress stream (optional)
            if (compressMode == CompressionMode.Deflate || compressMode == CompressionMode.GZip)
            {
                using var output = new MemoryStream();
                using (var stream = compressMode == CompressionMode.Deflate
                    ? new DeflateStream(output, System.IO.Compression.CompressionMode.Compress)
                    : (Stream)new GZipStream(output, System.IO.Compression.CompressionMode.Compress, true))
                {
                    stream.Write(rawData, 0, rawData.Length);
                }

                rawData = output.ToArray();
            }

            return rawData;
        }

        private static int ModulesPerSideFromVersion(int version) => 21 + (version - 1) * 4;

        private bool _isDisposed;
        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        /// <remarks>
        /// <list type="bulle ted">
        /// <see cref="Dispose(System.Boolean)" /> executes in two distinct scenarios.
        /// <item>If <paramref name="disposing" /> equals <c>true</c>, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources can be disposed.</item>
        /// <item>If <paramref name="disposing" /> equals <c>false</c>, the method has been called
        /// by the runtime from inside the finalizer and you should not reference other objects.
        /// Only unmanaged resources can be disposed.</item></list>
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                Version = 0;
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Compression mode.
        /// </summary>
        public enum CompressionMode
        {
            /// <summary>
            /// UnCompress
            /// </summary>
            UnCompressed,

            /// <summary>
            /// Deflate
            /// </summary>
            Deflate,

            /// <summary>
            /// GZip.
            /// </summary>
            GZip
        }
    }
}

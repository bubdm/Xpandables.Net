
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
using System.ComponentModel.DataAnnotations;

using Xpandables.Net5.Properties;

namespace Xpandables.Net5.Entities
{
    /// <summary>
    /// Defines the <see cref="Picture"/> class that holds properties for an image.
    /// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed record Picture(string Title, byte[] Content, ushort Height, ushort Width, string Extension)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Creates a <see cref="Picture"/> with the default image content.
        /// </summary>
        /// <returns>A new instance of <see cref="Picture"/> with default image content.</returns>
        public static Picture Default() => new Picture("Default", Resources.Default, 1500, 1500, "Png");

        /// <summary>
        /// Returns the UTF8 encoded string of the image.
        /// </summary>
        /// <returns>An UTF8 string.</returns>
        public override string ToString() => System.Text.Encoding.UTF8.GetString(Content, 0, Content.Length);
    }
}

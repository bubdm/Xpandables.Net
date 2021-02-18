
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
using System.ComponentModel.DataAnnotations;

using Xpandables.Net.Properties;

namespace Xpandables.Net
{
    /// <summary>
    /// Defines the <see cref="ValuePicture"/> class that holds properties for an image.
    /// <para>Returns a new instance <see cref="ValuePicture"/> with all properties.</para>
    /// </summary>
    /// <param name="Title">The picture title.</param>
    /// <param name="Content">The picture byte content.</param>
    /// <param name="Height">The picture height, in pixels, of this picture..</param>
    /// <param name="Width">The picture width, in pixels, of this picture.</param>
    /// <param name="Extension">The picture file format of this picture.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="Title"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Content"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Height"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Width"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Extension"/> is null.</exception>
    public sealed record ValuePicture([Required] string Title, byte[] Content, int Height, int Width, [Required] string Extension)
    {
        /// <summary>
        /// Creates a <see cref="ValuePicture"/> with the default image content.
        /// </summary>
        /// <returns>A new instance of <see cref="ValuePicture"/> with default image content.</returns>
        public static ValuePicture Default() => new("Default", Resources.Default, 1500, 1500, "Png");

        /// <summary>
        /// Creates a new picture from another.
        /// </summary>
        /// <param name="source">the picture source.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public ValuePicture(ValuePicture source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            Title = source.Title;
            Content = new byte[source.Content.Length];
            source.Content.CopyTo(Content, 0);
            Height = source.Height;
            Width = source.Width;
            Extension = source.Extension;
        }

        /// <summary>
        /// Clears the content of the picture.
        /// </summary>
        /// <returns>The current instance without content.</returns>
        public void Clear() => Array.Clear(Content, 0, Content.Length);

        /// <summary>
        /// Returns the UTF8 encoded string of the image.
        /// </summary>
        /// <returns>An UTF8 string.</returns>
        public override string ToString() => System.Text.Encoding.UTF8.GetString(Content, 0, Content.Length);
    }
}

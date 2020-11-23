
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

using Xpandables.Net.Properties;

namespace Xpandables.Net
{
    /// <summary>
    /// Defines the <see cref="ValuePicture"/> class that holds properties for an image.
    /// </summary>
    public sealed record ValuePicture
    {
        /// <summary>
        /// Initializes a new instance <see cref="ValuePicture"/> with all properties.
        /// </summary>
        /// <param name="title">The picture title.</param>
        /// <param name="content">The picture content.</param>
        /// <param name="height">The picture height.</param>
        /// <param name="width">The picture width.</param>
        /// <param name="extension">The picture extension.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="height"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="width"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="extension"/> is null.</exception>
        public ValuePicture(string title, byte[] content, int height, int width, string extension)
        {
            _ = content ?? throw new ArgumentNullException(nameof(content));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Height = height > 0 ? height : throw new ArgumentNullException(nameof(height));
            Width = width > 0 ? width : throw new ArgumentNullException(nameof(width));
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));

            Content = new byte[content.Length];
            content.CopyTo(Content, 0);
        }

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

        /// <summary>
        /// Gets the picture title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the picture byte content.
        /// </summary>
        public byte[] Content { get; }

        /// <summary>
        /// Gets the height, in pixels, of this picture.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the width, in pixels, of this picture.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the file format of this picture.
        /// </summary>
        public string Extension { get; }
    }
}

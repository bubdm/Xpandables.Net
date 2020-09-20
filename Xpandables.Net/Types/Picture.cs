
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
using System.ComponentModel.DataAnnotations;

using Xpandables.Net.Properties;

namespace Xpandables.Net.Types
{
    /// <summary>
    /// Defines the <see cref="Picture"/> class that holds properties for an image.
    /// </summary>
    public sealed class Picture : ValueObject
    {
        /// <summary>
        /// Initializes a new instance <see cref="Picture"/> with all properties.
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
        private Picture(string title, byte[] content, int height, int width, string extension)
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
        /// Creates a <see cref="Picture"/> with the default image content.
        /// </summary>
        /// <returns>A new instance of <see cref="Picture"/> with default image content.</returns>
        public static Picture Default() => new Picture("Default", Resources.Default, 1500, 1500, "Png");

        /// <summary>
        /// Creates a new picture with values.
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
        /// <returns>A new instance of <see cref="Picture"/>.</returns>
        public static Picture Create(string title, byte[] content, int height, int width, string extension)
            => new Picture(title, content, height, width, extension);

        /// <summary>
        /// Creates a new picture from another.
        /// </summary>
        /// <param name="source">the picture source.</param>
        /// <returns>A new instance of <see cref="Picture"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static Picture Create(Picture source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return new Picture(source.Title, source.Content, source.Height, source.Width, source.Extension);
        }

        /// <summary>
        /// Clears the content of the picture.
        /// </summary>
        /// <returns>The current instance without content.</returns>
        public Picture Clear()
        {
            Array.Clear(Content, 0, Content.Length);
            return Default();
        }

        /// <summary>
        /// Provides the list of components that comprise that class.
        /// </summary>
        /// <returns>An enumerable components of the derived class.</returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Title;
            yield return Content;
            yield return Height;
            yield return Width;
            yield return Extension;
        }

        /// <summary>
        /// Returns the UTF8 encoded string of the image.
        /// </summary>
        /// <returns>An UTF8 string.</returns>
        public override string ToString() => System.Text.Encoding.UTF8.GetString(Content, 0, Content.Length);

        /// <summary>
        /// Gets the picture title.
        /// </summary>
        [Required]
        public string Title { get; }

        /// <summary>
        /// Gets the picture byte content.
        /// </summary>
        [Required]
        public byte[] Content { get; }

        /// <summary>
        /// Gets the height, in pixels, of this picture.
        /// </summary>
        [Required]
        public int Height { get; }

        /// <summary>
        /// Gets the width, in pixels, of this picture.
        /// </summary>
        [Required]
        public int Width { get; }

        /// <summary>
        /// Gets the file format of this picture.
        /// </summary>
        [Required]
        public string Extension { get; }
    }
}

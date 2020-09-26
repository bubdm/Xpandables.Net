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
using System.Linq;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.String"/> type.
    /// </summary>
    public interface IStringRequest
    {
        /// <summary>
        /// Returns the body content that will be serialized.
        /// </summary>
        object? GetStringContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.ByteArray"/> type.
    /// </summary>
    public interface IByteArrayRequest
    {
        /// <summary>
        /// Returns the body content.
        /// </summary>
        byte[]? GetByteContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.Multipart"/> type.
    /// </summary>
    public interface IMultipartRequest : IByteArrayRequest, IStringRequest { }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.Stream"/> type.
    /// </summary>
    public interface IStreamRequest
    {
        /// <summary>
        /// Returns the body content.
        /// </summary>
        object GetStreamContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="ParameterLocation.Cookie"/>.
    /// </summary>
    public interface ICookieRequest
    {
        /// <summary>
        /// Returns the cookie content.
        /// </summary>
        object? GetCookieContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="ParameterLocation.Header"/>.
    /// </summary>
    public interface IHeaderRequest
    {
        /// <summary>
        /// Returns the header content.
        /// </summary>
        string? GetHeaderContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.FormUrlEncoded"/> type.
    /// </summary>
    public interface IFormUrlEncodedRequest
    {
        /// <summary>
        /// Returns the body content.
        /// </summary>
        IDictionary<string?, string?> GetFormContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the query string content for query string Uri when using <see cref="ParameterLocation.Query"/> or <see cref="ParameterLocation.Path"/>.
    /// This can be combined with other <see cref="IPathStringRequest"/>.
    /// </summary>
    public interface IQueryStringRequest
    {
        /// <summary>
        /// Returns the keys and values for the query string Uri.
        /// </summary>
        IDictionary<string, string?>? GetQueryStringSource();
    }

    /// <summary>
    /// Provides with a method to retrieve the path string content for query string Uri when using <see cref="ParameterLocation.Query"/> or <see cref="ParameterLocation.Path"/>.
    /// This can be combined with <see cref="IQueryStringRequest"/>.
    /// </summary>
    public interface IPathStringRequest
    {
        /// <summary>
        /// Returns the keys and values for the path string Uri.
        /// </summary>
        IDictionary<string, string> GetPathStringSource();
    }
}

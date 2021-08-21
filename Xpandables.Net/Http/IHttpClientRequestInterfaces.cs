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

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.String"/> type.
    /// </summary>
    public interface IHttpClientStringRequest
    {
        /// <summary>
        /// Returns the body content that will be serialized.
        /// </summary>
        object GetStringContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request patch content for <see cref="BodyFormat.String"/> type.
    /// You may use the generic interface that provides with the Json Document type and make use of <see cref="ContentType.JsonPatch"/> as content type.
    /// </summary>
    public interface IHttpClientPatchRequest
    {
        /// <summary>
        /// Returns the patch document.
        /// </summary>
        object GetPatchDocument();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.ByteArray"/> type.
    /// </summary>
    public interface IHttpClientByteArrayRequest
    {
        /// <summary>
        /// Returns the body content.
        /// </summary>
        byte[]? GetByteContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.Multipart"/> type.
    /// </summary>
    public interface IHttpClientMultipartRequest : IHttpClientByteArrayRequest, IHttpClientStringRequest { }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.Stream"/> type.
    /// </summary>
    public interface IHttpClientStreamRequest
    {
        /// <summary>
        /// Returns the body content.
        /// </summary>
        object GetStreamContent();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="BodyFormat.FormUrlEncoded"/> type.
    /// </summary>
    public interface IHttpClientFormUrlEncodedRequest
    {
        /// <summary>
        /// Returns the body content.
        /// </summary>
        IDictionary<string?, string?> GetFormSource();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="ParameterLocation.Cookie"/>.
    /// </summary>
    public interface IHttpClientCookieLocationRequest
    {
        /// <summary>
        /// Returns the keys and values for the cookie content.
        /// If a key is already present, its value will be replaced with the new one.
        /// </summary>
        IDictionary<string, object?> GetCookieSource();
    }

    /// <summary>
    /// Provides with a method to retrieve the request content for <see cref="ParameterLocation.Header"/>.
    /// </summary>
    public interface IHttpClientHeaderLocationRequest
    {
        /// <summary>
        /// Returns the keys and values for the header content.
        /// If a key is already present, its value will be replaced with the new one.
        /// </summary>
        IDictionary<string, string?> GetHeaderSource();

        /// <summary>
        /// Returns the keys and values for the header content.
        /// If a key is already present, its value will be replaced with the new one.
        /// </summary>
        IDictionary<string, IEnumerable<string?>> GetHeadersSource() => GetHeaderSource().ToDictionary(d => d.Key, d => (IEnumerable<string?>)new[] { d.Value });
    }

    /// <summary>
    /// Provides with a method to retrieve the query string content for query string Uri when using <see cref="ParameterLocation.Query"/>.
    /// This can be combined with other locations.
    /// </summary>
    public interface IHttpClientQueryStringLocationRequest
    {
        /// <summary>
        /// Returns the keys and values for the query string Uri.
        /// </summary>
        IDictionary<string, string?>? GetQueryStringSource();
    }

    /// <summary>
    /// Provides with a method to retrieve the path string content for query string Uri when using <see cref="ParameterLocation.Path"/>.
    /// This can be combined with other locations.
    /// </summary>
    public interface IHttpClientPathStringLocationRequest
    {
        /// <summary>
        /// Returns the keys and values for the path string Uri.
        /// </summary>
        IDictionary<string, string> GetPathStringSource();
    }
}

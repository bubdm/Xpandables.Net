
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
using System.Net.Http;
using System.Net.Http.Headers;

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.HttpRestClient
{
    /// <summary>
    /// Describes the parameters for a request used with <see cref="IHttpRestClientHandler"/>.
    /// The attribute should decorate implementations of <see cref="IAsyncQuery{TResult}"/> or <see cref="IAsyncCommand"/>
    /// in order to be used with <see cref="IHttpRestClientHandler"/>.
    /// Your class can implement the <see cref="IHttpRestClientAttributeProvider"/> to dynamically return a <see cref="HttpRestClientAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HttpRestClientAttribute : Attribute
    {
        /// <summary>
        /// Initializes the default instance of <see cref="HttpRestClientAttribute"/>.
        /// </summary>
        public HttpRestClientAttribute() { }

        /// <summary>
        /// Gets or sets the Uri path.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the location of data.
        /// The default value is <see cref="ParameterLocation.Body"/>.
        /// </summary>
        public ParameterLocation In { get; set; } = ParameterLocation.Body;

        /// <summary>
        /// Gets or sets the header / cookie name for <see cref="In"/> = <see cref="ParameterLocation.Cookie"/> or <see cref="ParameterLocation.Header"/>.
        /// </summary>
        public string HeaderCookieName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the method name.
        /// The default value is <see langword="HttpMethod.Post.Method"/>
        /// </summary>
        public string Method { get; set; } = HttpMethod.Post.Method;

        /// <summary>
        /// Gets or sets the format of the data.
        /// The default value is <see cref="DataFormat.Json"/>.
        /// </summary>
        public DataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets or sets the body format for data.
        /// The default value is <see cref="BodyFormat.String"/>.
        /// </summary>
        public BodyFormat BodyFormat { get; set; }

        /// <summary>
        /// Gets or sets the content type.
        /// The default value is <see cref="ContentType.Json"/>.
        /// </summary>
        public string ContentType { get; set; } = HttpRestClient.ContentType.Json;

        /// <summary>
        /// Gets or sets the accept content.
        /// The default value is 'application/json'.
        /// </summary>
        public string Accept { get; set; } = "application/json";

        /// <summary>
        /// Gets the value whether or not the request needs authorization.
        /// The default value is <see langword="true"/>.
        /// In this case, an <see cref="AuthenticationHeaderValue"/> with the <see cref="Scheme"/> value will be initialized and filled
        /// using one of the <see langword="ConfigureXPrimaryAuthorizationTokenHandler(IHttpClientBuilder)"/> methods extension.
        /// </summary>
        public bool IsSecured { get; set; } = true;

        /// <summary>
        /// Gets the value whether or not the query/command should be added to the request content.
        /// If <see langword="true"/> the query/command will not be added.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the authorization scheme.
        /// The default value is "Bearer".
        /// </summary>
        public string Scheme { get; set; } = "Bearer";
    }

    /// <summary>
    /// The location of the parameter.
    /// </summary>
    public enum ParameterLocation
    {
        /// <summary>
        /// Parameters that are appended to the URL.
        /// </summary>
        Query,

        /// <summary>
        /// Custom headers that are expected as part of the request.
        /// </summary>
        Header,

        /// <summary>
        /// Used together with Path Templating, where the parameter value is actually part of the operation's URL
        /// </summary>
        Path,

        /// <summary>
        /// Used to pass a specific cookie value to the API.
        /// </summary>
        Cookie,

        /// <summary>
        /// Used in the content of the request.
        /// </summary>
        Body
    }

    /// <summary>
    /// Determines the body format of the request.
    /// </summary>
    public enum BodyFormat
    {
        /// <summary>
        /// Body content matching the <see cref="StringContent"/>.
        /// The target class should implement <see cref="IStringRequest"/>, otherwise the hole class will be serialized.
        /// </summary>
        String,

        /// <summary>
        /// Body content matching the <see cref="ByteArrayContent"/>.
        /// The target class should implement <see cref="IByteArrayRequest"/>.
        /// </summary>
        ByteArray,

        /// <summary>
        /// Body content matching the <see cref="MultipartFormDataContent"/>.
        /// The target class should implement <see cref="IMultipartRequest"/>.
        /// </summary>
        Multipart,

        /// <summary>
        /// Body content matching the <see cref="StreamContent"/>.
        /// The target class should implement <see cref="IStreamRequest"/>.
        /// </summary>
        Stream,

        /// <summary>
        /// Body content matching the <see cref="FormUrlEncodedContent"/>.
        /// The target class should implement <see cref="IFormUrlEncodedRequest"/>.
        /// </summary>
        FormUrlEncoded
    }

    /// <summary>
    /// Provides with the content type.
    /// </summary>
    public static class ContentType
    {
        /// <summary>
        /// Returns the application json content type.
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        /// Returns the application XML content type.
        /// </summary>
        public const string Xml = "application/xml";

        /// <summary>
        /// Returns the application pdf content type.
        /// </summary>
        public const string Pdf = "application/pdf";

        /// <summary>
        /// Returns the image jpeg content type.
        /// </summary>
        public const string Jpeg = "image/jpeg";

        /// <summary>
        /// Returns the image png content type.
        /// </summary>
        public const string Png = "image/png";

        /// <summary>
        /// Returns the multi part form data content type.
        /// </summary>
        public const string Multipart = "multipart/form-data";

        /// <summary>
        /// Returns the text plain content type.
        /// </summary>
        public const string Text = "text/plain";

        /// <summary>
        /// Collections of content type from data format.
        /// </summary>
        public static readonly Dictionary<DataFormat, string> FromDataFormat = new Dictionary<DataFormat, string>
        {
            { DataFormat.Xml, Xml },
            { DataFormat.Json, Json },
            { DataFormat.Jpeg, Jpeg },
            { DataFormat.Multipart, Multipart },
            { DataFormat.Pdf, Pdf },
            { DataFormat.Png, Png },
            { DataFormat.Text, Text }
        };

        /// <summary>
        /// Returns the json accept header.
        /// </summary>
        public static readonly string[] JsonAccept = new string[5]
        {
            "application/json",
            "text/json",
            "text/x-json",
            "text/javascript",
            "*+json"
        };

        /// <summary>
        /// Returns the XML accept header.
        /// </summary>
        public static readonly string[] XmlAccept = new string[4]
        {
            "application/xml",
            "text/xml",
            "*+xml",
            "*"
        };
    }

    /// <summary>
    /// Determines the format of the target data.
    /// </summary>
    public enum DataFormat
    {
        /// <summary>
        /// Uses for the JSON format.
        /// </summary>
        Json,

        /// <summary>
        /// uses for XML format.
        /// </summary>
        Xml,

        /// <summary>
        /// uses for Pdf format.
        /// </summary>
        Pdf,

        /// <summary>
        /// uses for Jpeg format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// uses for Png format.
        /// </summary>
        Png,

        /// <summary>
        /// uses for Text format.
        /// </summary>
        Text,

        /// <summary>
        /// uses for Multi part format.
        /// </summary>
        Multipart,

        /// <summary>
        /// No specified format.
        /// </summary>
        None
    }
}

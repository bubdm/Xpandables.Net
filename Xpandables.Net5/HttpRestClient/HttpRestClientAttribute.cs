
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
using System.Design;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace System
{
    /// <summary>
    /// Describes the parameters for a request used with <see cref="IHttpRestClientHandler"/>.
    /// The attribute should decorate implementations of <see cref="IQuery{TResult}"/> or <see cref="ICommand"/>
    /// in order to be used with <see cref="IHttpRestClientHandler"/>.
    /// Your class can implement the <see cref="IHttpRestClientAttributeProvider"/> to dynamically return a <see cref="HttpRestClientAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HttpRestClientAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the Uri path.
        /// </summary>
        public string? Path { get; set; }

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
        public string ContentType { get; set; } = System.ContentType.Json;

        /// <summary>
        /// Gets or sets the accept content.
        /// The default value is 'application/json'.
        /// </summary>
        public string Accept { get; set; } = "application/json";

        /// <summary>
        /// Determines whether or not the request needs authorization.
        /// The default value is <see langword="true"/>.
        /// In this case, an <see cref="AuthenticationHeaderValue"/> with the <see cref="Scheme"/> value will be initialized and filled
        /// using one of the <see langword="ConfigureXPrimaryAuthorizationTokenHandler(IHttpClientBuilder)"/> methods extension.
        /// </summary>
        public bool IsSecured { get; set; } = true;

        /// <summary>
        /// Determines whether or not the query/command should be added to the request content.
        /// If <see langword="true"/> the query/command will not be added.
        /// The default value is <see langword="false"/>.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the authorization scheme.
        /// The default value is "Bearer".
        /// </summary>
        public string Scheme { get; set; } = "Bearer";

        /// <summary>
        /// Returns the <see cref="HttpRequestMessage"/> from the attribute.
        /// </summary>
        /// <typeparam name="TSource">the type of the source.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public HttpRequestMessage GetHttpRequestMessage<TSource>(TSource source)
            where TSource : class
        {
            Path ??= "/";
            Uri uri;
            if (source is IQueryStringRequest queryStringRequest)
            {
                var queryString = queryStringRequest.GetQueryString();
                var queryStringUri = Path.AddQueryString(queryString);
                uri = new Uri(queryStringUri, UriKind.Relative);
            }
            else
            {
                uri = new Uri(Path, UriKind.Relative);
            }

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(Method), uri);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Accept));
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(Threading.Thread.CurrentThread.CurrentCulture.Name));

            if (!IsNullable)
            {
                switch (BodyFormat)
                {
                    case BodyFormat.ByteArray:
                        ValidateImplementation<IByteArrayRequest>(source);
                        if ((source as IByteArrayRequest)!.GetByteContent() is { } byteContent)
                            httpRequestMessage.Content = new ByteArrayContent(byteContent);
                        break;
                    case BodyFormat.FormUrlEncoded:
                        ValidateImplementation<IFormUrlEncodedRequest>(source);
                        if ((source as IFormUrlEncodedRequest)!.GetFormContent() is { } formContent)
                            httpRequestMessage.Content = new FormUrlEncodedContent(formContent);
                        break;
                    case BodyFormat.Multipart:
                        ValidateImplementation<IMultipartRequest>(source);
                        var multipartContent = new MultipartFormDataContent();
                        var multipartSource = source as IMultipartRequest;

                        if (multipartSource!.GetByteContent() is { } mbyteContent)
                            multipartContent.Add(new ByteArrayContent(mbyteContent));

                        if (multipartSource.GetStringContent() is { } mstringContent)
                            multipartContent.Add(new StringContent(JsonSerializer.Serialize(mstringContent), Encoding.UTF8, ContentType));

                        httpRequestMessage.Content = multipartContent;
                        break;
                    case BodyFormat.Stream:
                        ValidateImplementation<IStreamRequest>(source);
                        var memoryStream = new MemoryStream();
                        using (var jsonWriter = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true }))
                        {
                            JsonSerializer.Serialize(jsonWriter, source);
                            jsonWriter.Flush();
                        }
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        httpRequestMessage.Content = new StreamContent(memoryStream);
                        break;
                    case BodyFormat.String:
                        ValidateImplementation<IStringRequest>(source, true);
                        httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, ContentType);
                        break;
                }

                httpRequestMessage!.Content!.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            if (IsSecured)
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Scheme);

            return httpRequestMessage;
        }

        private void ValidateImplementation<TInterface>(object source, bool optional = false)
        {
            if (!typeof(TInterface).IsAssignableFrom(source.GetType()) && !optional)
                throw new ArgumentException($"{source.GetType().Name} must implement {typeof(TInterface).Name} interface");
        }
    }

    /// <summary>
    /// Determines the body format of the request.
    /// </summary>
    public enum BodyFormat
    {
        /// <summary>
        /// Body content macthing the <see cref="StringContent"/>.
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

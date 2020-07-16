
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;

using Xpandables.Net5.Helpers;

[assembly: InternalsVisibleTo("Xpandables.Net5.HttpRestClient")]

namespace Xpandables.Net5.HttpRestClient
{
    /// <summary>
    /// Represents an HTTP Rest client response.
    /// </summary>
    public class HttpRestClientResponse
    {
        /// <summary>
        /// Returns a success HTTP status response.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        public static HttpRestClientResponse Success(HttpStatusCode statusCode = HttpStatusCode.OK)
            => new HttpRestClientResponse(statusCode);

        /// <summary>
        /// Returns a failure HTTP status response.
        /// </summary>
        /// <param name="exception">The handled exception of the response.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public static HttpRestClientResponse Failure(Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new HttpRestClientResponse(exception, statusCode);

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse"/> class with the status code.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        protected HttpRestClientResponse(HttpStatusCode statusCode) => StatusCode = statusCode;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse"/> class with exception and status code.
        /// </summary>
        /// <param name="exception">The handled exception of the response.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        protected HttpRestClientResponse(HttpRestClientException exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse"/> class with exception and status code.
        /// </summary>
        /// <param name="exception">The handled exception of the response.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        protected HttpRestClientResponse(Exception exception, HttpStatusCode statusCode)
        {
            Exception = new HttpRestClientException(exception.Message, exception);
            StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the <see cref="HttpRestClientException"/> that holds the handled exception.
        /// </summary>
        public HttpRestClientException? Exception { get; protected set; }

        /// <summary>
        /// Gets the HTTP response version.
        /// </summary>
        public Version? Version { get; protected set; }

        /// <summary>
        /// Gets  the reason phrase which typically is sent by servers together with the status code.
        /// </summary>
        public string? ReasonPhrase { get; protected set; }

        /// <summary>
        /// Gets all headers of the HTTP response.
        /// </summary>
#pragma warning disable CA2227 // Collection properties should be read only
        public NameValueCollection? Headers { get; protected set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets the response status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; protected set; } = HttpStatusCode.OK;

        /// <summary>
        /// Determines whether or not the response status is valid.
        /// Returns <see langword="true"/> if so, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsValid() => StatusCode == HttpStatusCode.OK || StatusCode == HttpStatusCode.Created || StatusCode == HttpStatusCode.Accepted;

        /// <summary>
        /// Adds the reason phrase.
        /// </summary>
        /// <param name="reason">the reason phrase to be used.</param>
        public HttpRestClientResponse AddReasonPhrase(string? reason) => this.With(h => h.ReasonPhrase = reason);

        /// <summary>
        /// Adds the response headers.
        /// </summary>
        /// <param name="headers">the headers to be used.</param>
        public HttpRestClientResponse AddHeaders(NameValueCollection headers)
            => this.With(h => h.Headers = headers ?? throw new ArgumentNullException(nameof(headers)));

        /// <summary>
        /// Adds the version.
        /// </summary>
        /// <param name="version">the version to be used.</param>
        public HttpRestClientResponse AddVersion(Version version) => this.With(h => h.Version = version);
    }

    /// <summary>
    ///  Represents an HTTP Rest client response of a specific type result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class HttpRestClientResponse<TResult> : HttpRestClientResponse
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        ///  Returns a success HTTP status response.
        /// </summary>
        /// <param name="result">The result instance.</param>
        /// <param name="statusCode">The status response code.</param>
        public static HttpRestClientResponse<TResult> Success(TResult result, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new HttpRestClientResponse<TResult>(result, statusCode);

        /// <summary>
        ///  Returns a success HTTP status response.
        /// </summary>
        /// <param name="statusCode">The status response code.</param>
        public static new HttpRestClientResponse<TResult> Success(HttpStatusCode statusCode = HttpStatusCode.OK)
            => new HttpRestClientResponse<TResult>(statusCode);

        /// <summary>
        /// Returns a failure HTTP status response.
        /// </summary>
        /// <param name="exception">The handled exception of the response.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public static new HttpRestClientResponse<TResult> Failure(
            Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new HttpRestClientResponse<TResult>(exception, statusCode);

        /// <summary>
        ///  Returns a failure HTTP status response.
        /// </summary>
        /// <param name="exception">The handled exception.</param>
        /// <param name="statusCode">The status response code.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public static HttpRestClientResponse<TResult> Failure(
            HttpRestClientException exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new HttpRestClientResponse<TResult>(exception, statusCode);

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse{TResult}"/> class with the status code.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        protected HttpRestClientResponse(HttpStatusCode statusCode)
            : base(statusCode) => Result = default;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse{TResult}"/> class with exception and status code.
        /// </summary>
        /// <param name="exception">The handled exception of the response.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        protected HttpRestClientResponse(HttpRestClientException exception, HttpStatusCode statusCode)
            : base(exception, statusCode) => Result = default;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse{TResult}"/> class with exception and status code.
        /// </summary>
        /// <param name="exception">The handled exception of the response.</param>
        /// <param name="statusCode">The status code of the response.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        protected HttpRestClientResponse(Exception exception, HttpStatusCode statusCode)
            : base(exception, statusCode) => Result = default;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpRestClientResponse"/> class with the status code.
        /// </summary>
        /// <param name="result">The result instance.</param>
        /// <param name="statusCode">The status code of the response.</param>
        protected HttpRestClientResponse(TResult result, HttpStatusCode statusCode)
            : base(statusCode) => Result = result;

        /// <summary>
        /// Gets the HTTP response content.
        /// </summary>
        [AllowNull]
        public TResult Result { get; }

        /// <summary>
        /// Adds the reason phrase.
        /// </summary>
        /// <param name="reason">the reason phrase to be used.</param>
        public new HttpRestClientResponse<TResult> AddReasonPhrase(string? reason) => this.With(h => h.ReasonPhrase = reason);

        /// <summary>
        /// Adds the response headers.
        /// </summary>
        /// <param name="headers">the headers to be used.</param>
        public new HttpRestClientResponse<TResult> AddHeaders(NameValueCollection headers)
            => this.With(h => h.Headers = headers ?? throw new ArgumentNullException(nameof(headers)));

        /// <summary>
        /// Adds the version.
        /// </summary>
        /// <param name="version">the version to be used.</param>
        public new HttpRestClientResponse<TResult> AddVersion(Version version) => this.With(h => h.Version = version);
    }
}

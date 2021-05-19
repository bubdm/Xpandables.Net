
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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace Xpandables.Net.Http
{
    /// <summary>
    /// Provides with extension method for <see cref="IHttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientExtensions
    {
        /// <summary>
        /// Determines whether the current exception message is <see cref="HttpRestClientValidation"/>.
        /// The method will try to parse the property named 'errors' from the exception message to <see cref="HttpRestClientValidation"/>.
        /// </summary>
        /// <param name="httpRestClientException">The target exception.</param>
        /// <param name="clientValidation">The <see cref="HttpRestClientValidation"/> instance if true.</param>
        /// <param name="exception">The handled exception during process.</param>
        /// <param name="settings">The optional settings for serializer.</param>
        /// <returns><see langword="true"/> if exception message is <see cref="HttpRestClientValidation"/>, otherwise <see langword="false"/>.</returns>
        public static bool IsHttpRestClientValidation(
            this HttpRestClientException httpRestClientException,
            [MaybeNullWhen(false)] out HttpRestClientValidation clientValidation,
            [MaybeNullWhen(true)] out Exception exception,
            JsonSerializerSettings? settings = default)
        {
            _ = httpRestClientException ?? throw new ArgumentNullException(nameof(httpRestClientException));

            try
            {
                exception = default;
                var anonymousType = new { Errors = default(HttpRestClientValidation) };
                var result = settings switch
                {
                    null => JsonConvert.DeserializeAnonymousType(httpRestClientException.Message, anonymousType),
                    _ => JsonConvert.DeserializeAnonymousType(httpRestClientException.Message, anonymousType, settings)
                };

                clientValidation = result?.Errors;
                return clientValidation is not null;
            }
            catch (Exception ex)
            {
                exception = ex;
                clientValidation = default;
                return false;
            }
        }
    }
}

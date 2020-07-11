﻿
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
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Xpandables.Net5.HttpRest
{
    /// <summary>
    /// Helpers for <see cref="HttpRestClientHandler"/>.
    /// </summary>
    public static class HttpRestClientHelpers
    {
        internal static HttpRequestMessage GetHttpRequestMessage<TSource>(TSource source)
            where TSource : class
        {
            var attribute = GetHttpClientDescriptionAttribute(source);
            return attribute.GetHttpRequestMessage(source);
        }

        internal static HttpRestClientAttribute GetHttpClientDescriptionAttribute<TSource>(TSource source)
              where TSource : class
        {
            if (source is IHttpRestClientAttributeProvider httpRestClientAttributeProvider)
                return httpRestClientAttributeProvider.GetHttpRestClientAttribute();

            return source.GetType().GetCustomAttribute<HttpRestClientAttribute>()
                         ?? throw new ArgumentNullException($"{nameof(HttpRestClientAttribute)} excepted from : {source.GetType().Name}");
        }

        internal static NameValueCollection GetHttpResponseHeaders(HttpResponseMessage httpResponse)
            => Enumerable
                .Empty<(string Name, string Value)>()
                .Concat(
                    httpResponse.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            )
                        )
                .Concat(
                    httpResponse.Content?.Headers
                        .SelectMany(kvp => kvp.Value
                            .Select(v => (Name: kvp.Key, Value: v))
                            ) ?? Enumerable.Empty<(string, string)>()
                        )
                .Aggregate(
                    seed: new NameValueCollection(),
                    func: (nvc, pair) => { nvc.Add(pair.Name, pair.Value); return nvc; },
                    resultSelector: nvc => nvc
                    );

    }
}

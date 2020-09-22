
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
using Newtonsoft.Json;

using Xpandables.Net.HttpRestClient;

namespace Xpandables.Net.Api.Models
{
    public class Response
    {
        public static Response Create() => new Response();
        internal Response AddMessage(string? message) => this.Assign(s => s.Message = message);
        public string? Message { get; set; }
        [JsonIgnore]
        public HttpRestClientValidation? ClientValidation { get; set; }
        public bool IsClientValidation() => ClientValidation is { };
        internal Response AddClientValidation(HttpRestClientValidation clientValidation) => this.Assign(s => s.ClientValidation = clientValidation);
        internal Response AddClientValidation(string errorMessage, params string[] memberNames)
        {
            var clientValidation = new HttpRestClientValidation();
            foreach (var membername in memberNames)
                clientValidation.Add(membername, new[] { errorMessage });

            ClientValidation = clientValidation;
            return this;
        }
    }

    public class Response<TResponse> : Response
        where TResponse : class, new()
    {
        public new static TResponse Create() => new TResponse();
        internal new TResponse AddMessage(string? message) => (this.Assign(s => s.Message = message) as TResponse)!;
        internal new TResponse AddClientValidation(HttpRestClientValidation clientValidation) => (base.AddClientValidation(clientValidation) as TResponse)!;
        internal new TResponse AddClientValidation(string errorMessage, params string[] memberNames) => (base.AddClientValidation(errorMessage, memberNames) as TResponse)!;
    }
}


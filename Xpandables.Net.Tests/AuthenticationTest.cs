
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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Http;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.Strings;

namespace Xpandables.Net.Tests
{
    [TestClass]
    public class AuthenticationTest
    {
        static IHttpRestClientHandler httpClientHandler = null!;
        static string token = null!;
        static readonly HttpTokenAccessorDelegate httpTokenAccessor = key => token;
        static readonly IStringCryptography stringCryptography = new StringCryptography(new StringGenerator());

        [ClassInitialize]
        public static void Initialize(TestContext _)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var factory = new WebApplicationFactory<Api.Program>();
            var client = factory.CreateDefaultClient(new AuthorizationHttpTokenHandler(new HttpTokenAccessorBuilder(httpTokenAccessor)));

            httpClientHandler = new HttpRestClientHandler(client);
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task RequestAuthenticationTokenAsync(string phone, string password)
        {
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{phone}:{password}"));
            httpClientHandler.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), token);

            var request = RequestAuthenToken.Default();
            using var response = await httpClientHandler.HandleAsync(request).ConfigureAwait(false);

            var result = await response.Result.FirstAsync().ConfigureAwait(false);
            Assert.IsNotNull(result.Token);
            Trace.WriteLine(result.Token);
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task AuthenAndEditUserInformationAsync(string phone, string password)
        {
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{phone}:{password}"));
            httpClientHandler.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), token);

            var request = RequestAuthenToken.Default();
            using var response = await httpClientHandler.HandleAsync(request).ConfigureAwait(false);

            var auth = await response.Result.FirstAsync().ConfigureAwait(false);
            token = auth.Token;

            for (int i = 0; i < 2; i++)
            {
                var email = $"{stringCryptography.StringGenerator.Generate(6, "abcdefghijklmonpqrstuvwxyz")}@{stringCryptography.StringGenerator.Generate(10, "abcdefghijklmonpqrstuvwxyz")}.{stringCryptography.StringGenerator.Generate(3, "abcdefghijklmonpqrstuvwxyz")}";

                var editUser = new EditUser(email, default, default);

                using var responseUser = await httpClientHandler.HandleAsync(editUser).ConfigureAwait(false);
                Assert.IsTrue(responseUser.IsValid());
            }
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task AutheAndListEventLogsAsync(string phone, string password)
        {
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{phone}:{password}"));
            httpClientHandler.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), token);

            var request = RequestAuthenToken.Default();
            using var response = await httpClientHandler.HandleAsync(request).ConfigureAwait(false);

            var auth = await response.Result.FirstAsync().ConfigureAwait(false);
            token = auth.Token;
            var eventLogRequest = new EventLogList() { StartOccuredOn = new DateTime(2020, 09, 24, 10, 00, 35) };

            using var listResponse = await httpClientHandler.HandleAsync(eventLogRequest).ConfigureAwait(false);
            if (listResponse.IsValid())
            {
                await foreach (var log in listResponse.Result.ConfigureAwait(false))
                    Trace.WriteLine($"Timer : {DateTime.UtcNow:O}  {log}");
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task GetUserAsync(string phone, string password)
        {
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{phone}:{password}"));
            httpClientHandler.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString(), token);

            var request = RequestAuthenToken.Default();
            using var response = await httpClientHandler.HandleAsync(request).ConfigureAwait(false);

            var auth = await response.Result.FirstAsync().ConfigureAwait(false);
            token = auth.Token;
            var getUser = new GetUser("IKHTJDQMZWSOKSPYIOFUBMRCHPDAJ197");

            using var userResponse = await httpClientHandler.HandleAsync(getUser).ConfigureAwait(false);
            if (userResponse.IsValid())
            {
                await foreach (var user in userResponse.Result.ConfigureAwait(false))
                    Trace.WriteLine($"Id : {user.Id} Phone : {user.Phone} Email : {user.Email} ");
            }
            else
            {
                Trace.WriteLine(userResponse.Exception);
                Assert.Fail();
            }
        }

        [ClassCleanup]
        public static void CleanUp() => httpClientHandler?.Dispose();
    }
}

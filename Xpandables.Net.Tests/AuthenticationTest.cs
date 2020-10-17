
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
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Http;
using Xpandables.Net.HttpRestClient;
using Xpandables.Net.HttpRestClient.Network;
using Xpandables.Net.Optionals;
using Xpandables.Net.Strings;

namespace Xpandables.Net.Tests
{
    [TestClass]
    public class AuthenticationTest
    {
        static IHttpRestClientHandler authenHandler = null!;
        static IHttpRestClientHandler anonymousHandler = null!;
        static string token = null!;
        static readonly HttpTokenAccessorDelegate httpTokenAccessor = key => token;
        static readonly IStringCryptography stringCryptography = new StringCryptography(new StringGenerator());

        [ClassInitialize]
        public static void Initialize(TestContext _)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var factory = new WebApplicationFactory<Api.Program>();
            var authenClient = factory.CreateDefaultClient(new AuthorizationHttpTokenDelegateHandler(new HttpTokenAccessorBuilder(httpTokenAccessor)));
            var client = factory.CreateClient();

            authenHandler = new HttpRestClientHandler(authenClient, new HttpRestClientEngine());
            anonymousHandler = new HttpRestClientHandler(client, new HttpRestClientEngine());
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task RequestAuthenticationTokenAsync(string phone, string password)
        {
            var request = new GetAuthenToken(phone, password);
            using var response = await authenHandler.HandleAsync(request).ConfigureAwait(false);

            var result = response.Result;
            Assert.IsNotNull(result.Token);
            Trace.WriteLine(result.Token);
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task AuthenAndEditUserInformationAsync(string phone, string password)
        {
            var request = new GetAuthenToken(phone, password);
            using var response = await anonymousHandler.HandleAsync(request).ConfigureAwait(false);

            var auth = response.Result;
            token = auth.Token;

            for (int i = 0; i < 2; i++)
            {
                var email = $"{stringCryptography.StringGenerator.Generate(6, "abcdefghijklmonpqrstuvwxyz")}@{stringCryptography.StringGenerator.Generate(10, "abcdefghijklmonpqrstuvwxyz")}.{stringCryptography.StringGenerator.Generate(3, "abcdefghijklmonpqrstuvwxyz")}";

                var editUser = new EditUser(email, default, default);

                using var responseUser = await authenHandler.HandleAsync(editUser).ConfigureAwait(false);
                Assert.IsTrue(responseUser.IsValid());
            }
        }

        [TestMethod]
        [DataRow("+33123456789", "motdepasse")]
        public async Task AutheAndListEventLogsAsync(string phone, string password)
        {
            var request = new GetAuthenToken(phone, password);
            using var response = await anonymousHandler.HandleAsync(request).ConfigureAwait(false);

            var auth = response.Result;
            token = auth.Token;

            var eventLogRequest = new EventLogList() { StartOccuredOn = new DateTime(2020, 8, 11, 10, 00, 35) };

            using var listResponse = await authenHandler.HandleAsync(eventLogRequest).ConfigureAwait(false);
            if (listResponse.IsValid())
            {
                var results = Optional<IAsyncEnumerable<Log>>.Some(listResponse.Result);

                await foreach (var log in results.GetEnumerable().ConfigureAwait(false))
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
            var request = new GetAuthenToken(phone, password);
            using var response = await anonymousHandler.HandleAsync(request).ConfigureAwait(false);

            var auth = response.Result;
            token = auth.Token;
            var getUser = new GetUser(response.Result.Key);

            using var userResponse = await authenHandler.HandleAsync(getUser).ConfigureAwait(false);
            if (userResponse.IsValid())
            {
                var user = userResponse.Result;
                Trace.WriteLine($"Id : {user.Id} Phone : {user.Phone} Email : {user.Email} ");
            }
            else
            {
                Trace.WriteLine(userResponse.Exception);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task GetIpAddressAsync()
        {
            IHttpRestClientIPHandler clientHandler = new HttpRestClientIPHandler(
                new HttpClient(new HttpRestClientIPMessageHandler()) { BaseAddress = new Uri("https://ipinfo.io/ip") }, new HttpRestClientEngine());
            using var response = await clientHandler.ReadIPAddressAsync().ConfigureAwait(false);

            Assert.IsNotNull(response.Result);
            Trace.WriteLine($"Ip ; {response.Result}");
        }

        [ClassCleanup]
        public static void CleanUp() => authenHandler?.Dispose();
    }
}

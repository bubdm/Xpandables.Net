using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api;
using Xpandables.Net.Api.Handlers;
using Xpandables.Net.Api.Services;
using Xpandables.Net.Http;

namespace Xpandables.Net.Tests
{
    [TestClass]
    public class ContactApiTest
    {
        private IHttpRestClientHandler httpRestClientHandler = null!;
        [TestInitialize]
        public void Initialize()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            httpRestClientHandler = new HttpRestClientHandlerUsingNewtonsoft(client);
        }

        [TestCleanup]
        public void CleanUp()
        {
            httpRestClientHandler?.Dispose();
        }

        [TestMethod]
        public async Task SelectAllTest()
        {
            var selectAll = new SelectAll();
            using var response = await httpRestClientHandler.HandleAsync(selectAll).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
            else
            {
                await foreach (var contact in response.Result)
                    Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
            }
        }

        [TestMethod]
        public async Task AddTest()
        {
            var add = new Add("My New Name", "My New City", "My new Address", "My new Country");
            using var response = await httpRestClientHandler.HandleAsync(add).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
            else
            {
                var select = new Select(response.Result);
                using var selectResponse = await httpRestClientHandler.HandleAsync(select).ConfigureAwait(false);
                var contact = selectResponse.Result!;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");

                Assert.AreEqual(response.Result, contact.Id);
            }
        }

        [TestMethod]
        public async Task GetIpTest()
        {
            var getIp = new GetIp("216.58.204.100");
            using var response = await httpRestClientHandler.HandleAsync(getIp).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
            else
            {
                var location = response.Result;
                Trace.WriteLine($"{location.Ip} {location.City} {location.Country_Name} {location.Latitude} {location.Longitude}");
            }
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            var selectAll = new SelectAll();
            using var response = await httpRestClientHandler.HandleAsync(selectAll).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
            else
            {
                var toDelete = await response.Result.FirstAsync().ConfigureAwait(false);
                var delete = new Delete(toDelete.Id);
                using var delResponse = await httpRestClientHandler.HandleAsync(delete).ConfigureAwait(false);
                if (!delResponse.IsValid())
                {
                    Trace.WriteLine($"{delResponse.StatusCode}");
                    return;
                }
                else
                {
                    Trace.WriteLine($"{delResponse.StatusCode}");
                    Assert.IsTrue(true);
                }
            }
        }
    }
}

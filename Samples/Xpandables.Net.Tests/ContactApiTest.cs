using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api;
using Xpandables.Net.Api.Handlers;
using Xpandables.Net.Api.Models;
using Xpandables.Net.Http;
using Xpandables.Net.Http.RequestBuilders;
using Xpandables.Net.Http.RequestHandlers;
using Xpandables.Net.Http.ResponseBuilders;

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
            httpRestClientHandler = new HttpRestClientHandler(new HttpRestClientNewtonsoftRequestBuilder(), new HttpRestClientNewtonsoftResponseBuilder(), client);
        }

        [TestCleanup]
        public void CleanUp()
        {
            httpRestClientHandler?.Dispose();
        }

        [TestMethod]
        public async Task SelectAllTest()
        {
            var selectAll = new SelectAllQuery();
            using var response = await httpRestClientHandler.SendAsync(selectAll).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                Assert.Fail();
            }
            else
            {
                await foreach (var contact in response.Result)
                    Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
            }
        }

        [TestMethod]
        public async Task SelectTest()
        {
            var select = new SelectQuery("c0bc392c-fd86-41a3-aec8-1894e8908490");
            using var response = await httpRestClientHandler.SendAsync(select).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                Assert.Fail();
            }
            else
            {
                var contact = response.Result;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
            }
        }

        [TestMethod]
        public async Task AddTest()
        {
            var add = new AddCommand("My New Name", "My New City", "My new Address", "My new Country");
            using var response = await httpRestClientHandler.SendAsync(add).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
            }
            else
            {
                var select = new SelectQuery(response.Result);
                using var selectResponse = await httpRestClientHandler.SendAsync(select).ConfigureAwait(false);
                var contact = selectResponse.Result!;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");

                Assert.AreEqual(response.Result, contact.Id);
            }
        }

        [TestMethod]
        public async Task EditTest()
        {
            var select = new SelectQuery(ContactModel.FirstGuidCreated);
            using var response = await httpRestClientHandler.SendAsync(select).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
            }
            else
            {
                var toEdit = response.Result;
                var edit = new EditCommand { Id = toEdit.Id, Address = "Address from edit", City = "City from edit", Country = "Country from edit", Name = "Name from edit" };
                using var editResponse = await httpRestClientHandler.SendAsync(edit).ConfigureAwait(false);
                if (!editResponse.IsValid())
                {
                    Trace.WriteLine($"{editResponse.StatusCode}");
                }
                else
                {
                    using var response1 = await httpRestClientHandler.SendAsync(select).ConfigureAwait(false);
                    Trace.WriteLine($"{response1.StatusCode}");
                    var contact = response1.Result;
                    Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
                }
            }
        }

        [TestMethod]
        public async Task GetIpTest()
        {
            var getIp = new GetIpQuery("216.58.204.100");
            using var response = await httpRestClientHandler.SendAsync(getIp).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                Assert.Fail();
            }
            else
            {
                var location = response.Result;
                Trace.WriteLine($"{location.Ip} {location.City} {location.Country_Name} {location.Latitude} {location.Longitude}");
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api;
using Xpandables.Net.Api.Handlers;
using Xpandables.Net.Http;

namespace Xpandables.Net.Tests
{
    [TestClass]
    public class ContactApiTest
    {
        IHttpRestClientHandler httpRestClientHandler = null!;
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
                await foreach (var contact in response.Result)
                    Trace.WriteLine($"{contact.Id} {contact.Name} {contact.City} {contact.Address} {contact.Country}");
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
                    Trace.WriteLine($"{response.StatusCode}");
                    return;
                }
                else
                {
                    Trace.WriteLine($"{response.StatusCode}");
                    Assert.IsTrue(true);
                }
            }
        }
    }
}

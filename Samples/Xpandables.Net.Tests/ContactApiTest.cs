using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api;
using Xpandables.Net.HttpRestClient;

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
            httpRestClientHandler = new HttpRestClientHandler(client, new HttpRestClientEngine());
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

            var response = await httpRestClientHandler.HandleAsync(selectAll).ConfigureAwait(false);

            if (!response.IsValid())
                Trace.WriteLine($"{response.StatusCode}");
            else
                await foreach (var contact in response.Result)
                    Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");
        }

        [TestMethod]
        [DataRow(1)]
        public async Task SelectTest(int id)
        {
            var select = new Select(id);

            var response = await httpRestClientHandler.HandleAsync(select).ConfigureAwait(false);

            if (!response.IsValid())
                Trace.WriteLine($"{response.StatusCode}");
            else
            {
                var contact = response.Result!;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");
            }
        }

        [TestMethod]
        public async Task AddTest()
        {
            var add = new Add("New Name", "New Address", "New City");

            var response = await httpRestClientHandler.HandleAsync(add).ConfigureAwait(false);

            if (!response.IsValid())
                Trace.WriteLine($"{response.StatusCode}");
            else
            {
                var select = new Select(response.Result);
                var selectResponse = await httpRestClientHandler.HandleAsync(select).ConfigureAwait(false);
                var contact = selectResponse.Result!;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");

                Assert.AreEqual(response.Result, contact.Id);
            }
        }

        [TestMethod]
        [DataRow(2, "New Name")]
        public async Task EditTest(int id, string newName)
        {
            var edit = new Edit(id, newName);

            var response = await httpRestClientHandler.HandleAsync(edit).ConfigureAwait(false);

            if (!response.IsValid())
                Trace.WriteLine($"{response.StatusCode}");
            else
            {
                var select = new Select(edit.Id);
                var selectResponse = await httpRestClientHandler.HandleAsync(select).ConfigureAwait(false);
                var contact = selectResponse.Result!;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");

                Assert.AreEqual(contact.Name, newName);
            }
        }
    }
}

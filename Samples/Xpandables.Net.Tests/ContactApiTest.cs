using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xpandables.Net.Api;
using Xpandables.Net.CQRS;
using Xpandables.Net.Http;
using Xpandables.Net.Http.Network;

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
            httpRestClientHandler = new HttpRestClientHandlerCustom(client);
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
                    Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");
        }

        [TestMethod]
        [DataRow(1)]
        public async Task SelectTest(int id)
        {
            var select = new Select(id);

            using var response = await httpRestClientHandler.HandleAsync(select).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
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
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");

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
                    Assert.IsTrue(true);
            }
        }

        public record Name(string Value) : IAsyncCommand;

        [TestMethod]
        [DataRow(2, "New Name")]
        public async Task EditTest(int id, string newName)
        {
            var edit = new Edit(id, newName);

            using var response = await httpRestClientHandler.HandleAsync(edit).ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
            else
            {
                var select = new Select(edit.Id);
                using var selectResponse = await httpRestClientHandler.HandleAsync(select).ConfigureAwait(false);
                var contact = selectResponse.Result!;
                Trace.WriteLine($"{contact.Id} {contact.Name} {contact.Address} {contact.City}");

                Assert.AreEqual(contact.Name, newName);
            }
        }

        [TestMethod]
        public async Task IPLocationTest()
        {
            using IHttpIPAddressAccessor ipHandler = new HttpIPAddressAccessor(httpRestClientHandler);
            var response = await ipHandler.ReadIPAddressAsync().ConfigureAwait(false);

            if (!response.IsValid())
            {
                Trace.WriteLine($"{response.StatusCode}");
                return;
            }
            else
                Trace.WriteLine($"IP Address : {response.Result}");

            using IHttpLocationAccessor locationHandler = new HttpLocationAccessor(httpRestClientHandler);
            var location = await locationHandler.ReadLocationAsync(new LocationRequest(response.Result.ToString(), "enter your api access key"));

            if (!location.IsValid())
            {
                Trace.WriteLine($"{location.StatusCode}");
                return;
            }
            else
                Trace.WriteLine($"IP Address : {location.Result.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
        }
    }
}


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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.CQRS;
using Xpandables.Net.Http;

namespace Xpandables.Net.Api
{
    public sealed record Contact(int Id, string Name, string Address, string City);

    [HttpRestClient(Path = "api/contacts", Method = "Get", IsSecured = true, IsNullable = true, In = ParameterLocation.Query)]
    public sealed record SelectAll : IAsyncQuery<Contact>, IQueryStringLocationRequest
    {
        public IDictionary<string, string?>? GetQueryStringSource() => new Dictionary<string, string?>();
    }

    [HttpRestClient(Path = "api/contacts/{id}", Method = "Get", IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed record Select([Required] int Id) : IQuery<Contact?>, IPathStringLocationRequest
    {
        [return: NotNull]
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), $"{Id}" } };
    }

    [HttpRestClient(Path = "api/contacts", Method = "Post", IsSecured = false)]
    public sealed record Add([Required] string Name, [Required] string Address, [Required] string City) : ICommand<int>;

    [HttpRestClient(Path = "api/contacts/{id}", Method = "Delete", IsSecured = true, IsNullable = true, In = ParameterLocation.Path)]
    public sealed record Delete([Required] int Id) : ICommand, IPathStringLocationRequest
    {
        [return: NotNull]
        public IDictionary<string, string> GetPathStringSource() => new Dictionary<string, string> { { nameof(Id), $"{Id}" } };
    }

    [HttpRestClient(Path = "api/contacts", Method = "Put", IsSecured = true, IsNullable = false, In = ParameterLocation.Body)]
    public sealed record Edit([Required] int Id, string? Name = default, string? Address = default, string? City = default) : ICommand<Contact>;

    sealed class ContactService
    {
        internal static readonly List<Contact> Contacts = new()
        {
            new Contact(1, "Filip W", "Paris 01", "Paris"),
            new Contact(2, "Jean Pierre", "25 Liberty Street", "Lyon"),
            new Contact(3, "Paul Louis", "1 Jean Paul Street", "Paris"),
            new Contact(4, "Alexandre LeGrand", "12 New Street", "Marseille"),
            new Contact(5, "André José", "18 Alexandre Street", "Paris"),
            new Contact(6, "Joseph Paul", "Paris 02 Street", "Metz"),
            new Contact(7, "Louis Jean", "14 Pierre Street", "Nantes")
        };
    }

    public sealed class ContactHandler :
        IAsyncQueryHandler<SelectAll, Contact>, IQueryHandler<Select, Contact?>, ICommandHandler<Add, int>, ICommandHandler<Delete>, ICommandHandler<Edit, Contact>
    {
        public IAsyncEnumerable<Contact> HandleAsync(SelectAll query, CancellationToken cancellationToken = default) => new AsyncEnumerable<Contact>(ContactService.Contacts);
        public async Task<IOperationResult<Contact?>> HandleAsync(Select query, CancellationToken cancellationToken = default)
        {
            var result = ContactService.Contacts.FirstOrDefault(c => c.Id == query.Id);
            return await Task.FromResult(new SuccessOperationResult<Contact?>(result)).ConfigureAwait(false);
        }
        public async Task<IOperationResult<int>> HandleAsync(Add command, CancellationToken cancellationToken = default)
        {
            var newId = (ContactService.Contacts.LastOrDefault()?.Id ?? 0) + 1;
            ContactService.Contacts.Add(new Contact(newId, command.Name, command.Address, command.City));
            return await Task.FromResult(new SuccessOperationResult<int>(newId)).ConfigureAwait(false);
        }
        public async Task<IOperationResult> HandleAsync(Delete command, CancellationToken cancellationToken = default)
        {
            var result = ContactService.Contacts.FirstOrDefault(c => c.Id == command.Id);
            if (result is null) throw new KeyNotFoundException();
            return await Task.FromResult(new SuccessOperationResult()).ConfigureAwait(false);
        }
        public async Task<IOperationResult<Contact>> HandleAsync(Edit command, CancellationToken cancellationToken = default)
        {
            var result = ContactService.Contacts.FirstOrDefault(c => c.Id == command.Id);

            if (result is null) throw new KeyNotFoundException();

            var index = ContactService.Contacts.IndexOf(result);
            if (command.Name is not null) result = result with { Name = command.Name };
            if (command.Address is not null) result = result with { Address = command.Address };
            if (command.City is not null) result = result with { City = command.City };

            ContactService.Contacts[index] = result;

            return await Task.FromResult(new SuccessOperationResult<Contact>(result)).ConfigureAwait(false);
        }
    }
}

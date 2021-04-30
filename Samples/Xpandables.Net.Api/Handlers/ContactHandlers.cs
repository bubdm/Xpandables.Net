
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Commands;
using Xpandables.Net.Database;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class ContactHandlers : OperationResultBase,
         ICommandHandler<AddCommand, string>, IQueryHandler<SelectQuery, Contact>, ICommandHandler<EditCommand, Contact>
    {
        private readonly IAggregateAccessor<ContactModel> _entityAccessor;
        public ContactHandlers(IAggregateAccessor<ContactModel> entityAccessor) => _entityAccessor = entityAccessor ?? throw new ArgumentNullException(nameof(entityAccessor));

        //public IAsyncEnumerable<Contact> HandleAsync(SelectAllQuery query, CancellationToken cancellationToken = default)
        //{

        //    return _entityAccessor.FetchAllAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken: cancellationToken);
        //}

        public async Task<IOperationResult<Contact>> HandleAsync(SelectQuery query, CancellationToken cancellationToken = default)
        {
            var foundResult = await _entityAccessor.ReadAsync(Guid.Parse(query.Id), cancellationToken).ConfigureAwait(false);
            if (foundResult.Failed)
                return NotFoundOperation<Contact>();
            var found = foundResult.Value;
            return OkOperation(new Contact(found.Guid.ToString(), found.Name, found.City, found.Address, found.Country));
        }

        public async Task<IOperationResult<string>> HandleAsync(AddCommand command, CancellationToken cancellationToken = default)
        {
            var newContact = ContactModel.CreateNewContact(command.Name, command.City, command.Address, command.Country);
            await _entityAccessor.AppendAsync(newContact, cancellationToken).ConfigureAwait(false);

            return new SuccessOperationResult<string>(newContact.Guid.ToString());
        }

        //public async Task<IOperationResult> HandleAsync(DeleteCommand command, CancellationToken cancellationToken = default)
        //{
        //    var toDelete = (await _entityAccessor.TryFindAsync(command, cancellationToken).ConfigureAwait(false))!;
        //    toDelete.Deleted();

        //    return new SuccessOperationResult();
        //}

        public async Task<IOperationResult<Contact>> HandleAsync(EditCommand command, CancellationToken cancellationToken = default)
        {
            var foundResult = await _entityAccessor.ReadAsync(Guid.Parse(command.Id), cancellationToken).ConfigureAwait(false);
            if (foundResult.Failed)
                return NotFoundOperation<Contact>();

            var found = foundResult.Value;
            if (command.Address is not null) found.ChangeContactAddress(command.Address);
            if (command.City is not null) found.ChangeContactCity(command.City);
            if (command.Country is not null) found.ChangeContactCountry(command.Country);
            if (command.Name is not null) found.ChangeContactName(command.Name);

            return OkOperation(new Contact(found.Guid.ToString(), found.Name, found.City, found.Address, found.Country));
        }
    }
}

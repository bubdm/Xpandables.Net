
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

using Xpandables.Net.Aggregates;
using Xpandables.Net.Api.Domains;
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class ContactHandlers : OperationResultBase,
         ICommandHandler<AddCommand, string>, IQueryHandler<SelectQuery, Contact>, ICommandHandler<EditCommand, Contact>,
        ICommandHandler<ContactNameChangedFailedCommand>
    {
        private readonly IAggregateAccessor<ContactAggregate> _entityAccessor;
        public ContactHandlers(IAggregateAccessor<ContactAggregate> entityAccessor) => _entityAccessor = entityAccessor ?? throw new ArgumentNullException(nameof(entityAccessor));

        public async Task<IOperationResult<Contact>> HandleAsync(SelectQuery query, CancellationToken cancellationToken = default)
        {
            var found = await _entityAccessor.ReadAsync(Guid.Parse(query.Id), cancellationToken).ConfigureAwait(false);
            if (found is null)
                return NotFoundOperation<Contact>();
            return OkOperation(new Contact(found.Guid.ToString(), found.Name, found.City, found.Address, found.Country));
        }

        public async Task<IOperationResult<string>> HandleAsync(AddCommand command, CancellationToken cancellationToken = default)
        {
            var newContact = ContactAggregate.CreateNewContact(command.Name, command.City, command.Address, command.Country);
            await _entityAccessor.AppendAsync(newContact, cancellationToken).ConfigureAwait(false);

            return OkOperation(newContact.Guid.ToString());
        }

        public async Task<IOperationResult<Contact>> HandleAsync(EditCommand command, CancellationToken cancellationToken = default)
        {
            var found = await _entityAccessor.ReadAsync(Guid.Parse(command.Id), cancellationToken).ConfigureAwait(false);
            if (found is null)
                return NotFoundOperation<Contact>();

            if (command.Address is not null) found.ChangeContactAddress(command.Address);
            if (command.City is not null) found.ChangeContactCity(command.City);
            if (command.Country is not null) found.ChangeContactCountry(command.Country);
            if (command.Name is not null) found.ChangeContactName(command.Name);

            await _entityAccessor.AppendAsync(found, cancellationToken).ConfigureAwait(false);

            return OkOperation(new Contact(found.Guid.ToString(), found.Name, found.City, found.Address, found.Country));
        }

        public async Task<IOperationResult> HandleAsync(ContactNameChangedFailedCommand command, CancellationToken cancellationToken = default)
        {
            var found = await _entityAccessor.ReadAsync(command.AggregateId, cancellationToken).ConfigureAwait(false);
            if (found is null)
                return NotFoundOperation();

            found.CancelNameChange(command.Name);
            await _entityAccessor.AppendAsync(found, cancellationToken).ConfigureAwait(false);
            return OkOperation();
        }
    }
}

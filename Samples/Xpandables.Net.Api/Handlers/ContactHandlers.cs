
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
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Api.Models;
using Xpandables.Net.CQRS;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class ContactHandlers :
        IAsyncQueryHandler<SelectAll, Contact>, IQueryHandler<Select, Contact>, ICommandHandler<Add, string>, ICommandHandler<Delete>, ICommandHandler<Edit, Contact>
    {
        private readonly IEntityAccessor<ContactModel> _entityAccessor;
        public ContactHandlers(IEntityAccessor<ContactModel> entityAccessor) => _entityAccessor = entityAccessor ?? throw new ArgumentNullException(nameof(entityAccessor));

        public IAsyncEnumerable<Contact> HandleAsync(SelectAll query, CancellationToken cancellationToken = default)
            => _entityAccessor.SelectAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken);

        public async Task<IOperationResult<Contact>> HandleAsync(Select query, CancellationToken cancellationToken = default)
        {
            var found = await _entityAccessor.TryFindAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken).ConfigureAwait(false);
            return found is not null ? new SuccessOperationResult<Contact>(found) : new FailureOperationResult<Contact>(System.Net.HttpStatusCode.NotFound);
        }

        public async Task<IOperationResult<string>> HandleAsync(Add command, CancellationToken cancellationToken = default)
        {
            var newContact = new ContactModel(command.Name, command.City, command.Address, command.Country);
            await _entityAccessor.AddAsync(newContact, cancellationToken).ConfigureAwait(false);

            return new SuccessOperationResult<string>(newContact.Id);
        }

        public async Task<IOperationResult> HandleAsync(Delete command, CancellationToken cancellationToken = default)
        {
            var toDelete = (await _entityAccessor.FindAsync(command, cancellationToken).ConfigureAwait(false))!;
            toDelete.Delete();

            await _entityAccessor.UpdateAsync(toDelete, cancellationToken).ConfigureAwait(false);
            return new SuccessOperationResult();
        }

        public async Task<IOperationResult<Contact>> HandleAsync(Edit command, CancellationToken cancellationToken = default)
        {
            var toEdit = (await _entityAccessor.FindAsync(command, cancellationToken).ConfigureAwait(false))!;
            toEdit.Edit(command.Name, command.City, command.Address, command.Country);

            await _entityAccessor.UpdateAsync(toEdit, cancellationToken).ConfigureAwait(false);
            return new SuccessOperationResult<Contact>(new Contact(toEdit.Id, toEdit.Name, toEdit.City, toEdit.Address, toEdit.Country));
        }
    }
}

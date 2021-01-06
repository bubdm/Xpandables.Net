
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
        private readonly IReadEntityAccessor<ContactModel> _readEntityAccessor;
        private readonly IWriteEntityAccessor<ContactModel> _writeEntityAccessor;
        public ContactHandlers(IReadEntityAccessor<ContactModel> readEntityAccessor, IWriteEntityAccessor<ContactModel> writeEntityAccessor)
        {
            _readEntityAccessor = readEntityAccessor ?? throw new ArgumentNullException(nameof(readEntityAccessor));
            _writeEntityAccessor = writeEntityAccessor ?? throw new ArgumentNullException(nameof(writeEntityAccessor));
        }

        public IAsyncEnumerable<Contact> HandleAsync(SelectAll query, CancellationToken cancellationToken = default)
            => _readEntityAccessor.SelectAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken);

        public async Task<IOperationResult<Contact>> HandleAsync(Select query, CancellationToken cancellationToken = default)
            => new SuccessOperationResult<Contact>(
                await _readEntityAccessor.FindAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken));

        public async Task<IOperationResult<string>> HandleAsync(Add command, CancellationToken cancellationToken = default)
        {
            var newContact = new ContactModel(command.Name, command.City, command.Address, command.Country);
            await _writeEntityAccessor.AddAsync(newContact, cancellationToken).ConfigureAwait(false);

            return new SuccessOperationResult<string>(newContact.Id);
        }

        public async Task<IOperationResult> HandleAsync(Delete command, CancellationToken cancellationToken = default)
        {
            var toDelete = (await _readEntityAccessor.FindAsync(command, cancellationToken).ConfigureAwait(false))!;
            toDelete.Delete();

            await _writeEntityAccessor.UpdateAsync(toDelete, cancellationToken).ConfigureAwait(false);
            return new SuccessOperationResult();
        }

        public async Task<IOperationResult<Contact>> HandleAsync(Edit command, CancellationToken cancellationToken = default)
        {
            var toEdit = (await _readEntityAccessor.FindAsync(command, cancellationToken).ConfigureAwait(false))!;
            toEdit.Edit(command.Name, command.City, command.Address, command.Country);

            await _writeEntityAccessor.UpdateAsync(toEdit, cancellationToken).ConfigureAwait(false);
            return new SuccessOperationResult<Contact>(new Contact(toEdit.Id, toEdit.Name, toEdit.City, toEdit.Address, toEdit.Country));
        }
    }
}

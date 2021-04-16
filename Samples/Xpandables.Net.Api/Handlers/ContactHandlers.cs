
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

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Commands;
using Xpandables.Net.Database;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Api.Handlers
{
    public interface IContactEntityAccessor : IEntityAccessor<ContactModel> { }
    public class ContactEntityAccessor : EntityAccessor<ContactModel>, IContactEntityAccessor
    {
        public ContactEntityAccessor(IDataContext<ContactModel> dataContext) : base(dataContext)
        {
        }
    }

    public sealed class ContactHandlers : OperationResultBase,
        IAsyncQueryHandler<SelectAll, Contact>, IQueryHandler<Select, Contact>, ICommandHandler<Add, string>, ICommandHandler<Delete>, ICommandHandler<Edit, Contact>
    {
        private readonly IContactEntityAccessor _entityAccessor;
        public ContactHandlers(IContactEntityAccessor entityAccessor) => _entityAccessor = entityAccessor ?? throw new ArgumentNullException(nameof(entityAccessor));

        public IAsyncEnumerable<Contact> HandleAsync(SelectAll query, CancellationToken cancellationToken = default)
            => _entityAccessor.FetchAllAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken: cancellationToken);

        public async Task<IOperationResult<Contact>> HandleAsync(Select query, CancellationToken cancellationToken = default)
        {
            var found = await _entityAccessor.AsNoTracking().TryFindAsync(query, s => new Contact(s.Id, s.Name, s.City, s.Address, s.Country), cancellationToken).ConfigureAwait(false);
            return found is not null ? OkOperation(found) : NotFoundOperation<Contact>();
        }

        public async Task<IOperationResult<string>> HandleAsync(Add command, CancellationToken cancellationToken = default)
        {
            var newContact = new ContactModel(command.Name, command.City, command.Address, command.Country);
            await _entityAccessor.AddAsync(newContact, cancellationToken).ConfigureAwait(false);

            return new SuccessOperationResult<string>(newContact.Id);
        }

        public async Task<IOperationResult> HandleAsync(Delete command, CancellationToken cancellationToken = default)
        {
            var toDelete = (await _entityAccessor.TryFindAsync(command, cancellationToken).ConfigureAwait(false))!;
            toDelete.Delete();

            return new SuccessOperationResult();
        }

        public async Task<IOperationResult<Contact>> HandleAsync(Edit command, CancellationToken cancellationToken = default)
        {
            var toEdit = (await _entityAccessor.AsTracking().TryFindAsync(command, cancellationToken).ConfigureAwait(false))!;
            toEdit.Edit(command.Name, command.City, command.Address, command.Country);

            return new SuccessOperationResult<Contact>(new Contact(toEdit.Id, toEdit.Name, toEdit.City, toEdit.Address, toEdit.Country));
        }
    }
}

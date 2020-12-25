
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Api.Models;
using Xpandables.Net.CQRS;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class ContactValidators : IValidation<Select>, IValidation<Add>, IValidation<Delete>, IValidation<Edit>
    {
        private readonly IDataContext<ContactModel> _dataContext;
        public ContactValidators(IDataContext<ContactModel> dataContext) => _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

        public async Task<IOperationResult> ValidateAsync(Select argument, CancellationToken cancellationToken = default)
        {
            if (await _dataContext.FindAsync(c => c.AsNoTracking().Where(argument).OrderBy(o => o.Id), cancellationToken).ConfigureAwait(false) is null)
                return new FailedOperationResult(System.Net.HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

            return new SuccessOperationResult();
        }
        public async Task<IOperationResult> ValidateAsync(Add argument, CancellationToken cancellationToken = default)
        {
            if (await _dataContext.FindAsync(c => c.AsNoTracking().Where(argument).OrderBy(o => o.Id), cancellationToken).ConfigureAwait(false) is not null)
                return new FailedOperationResult(System.Net.HttpStatusCode.BadRequest, nameof(argument.Name), "Contact already exist");

            return new SuccessOperationResult();
        }
        public async Task<IOperationResult> ValidateAsync(Delete argument, CancellationToken cancellationToken = default)
        {
            if (await _dataContext.FindAsync(c => c.AsNoTracking().Where(argument).OrderBy(o => o.Id), cancellationToken).ConfigureAwait(false) is null)
                return new FailedOperationResult(System.Net.HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

            return new SuccessOperationResult();
        }
        public async Task<IOperationResult> ValidateAsync(Edit argument, CancellationToken cancellationToken = default)
        {
            if (await _dataContext.FindAsync(c => c.AsNoTracking().Where(argument).OrderBy(o => o.Id), cancellationToken).ConfigureAwait(false) is not { } contact)
                return new FailedOperationResult(System.Net.HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

            argument.Name = contact.Name;
            argument.City = contact.City;
            argument.Address = contact.Address;
            argument.Country = contact.Country;

            return argument.ApplyPatch(argument);
        }
    }
}

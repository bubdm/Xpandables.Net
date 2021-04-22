
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Expressions.Specifications;
using Xpandables.Net.Validators;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class ContactValidators : IValidator<SelectQuery>, IValidator<AddCommand>, IValidator<DeleteCommand>, IValidator<EditCommand>
    {
        private readonly IContactEntityAccessor _entityAccessor;
        public ContactValidators(IContactEntityAccessor entityAcessor) => _entityAccessor = entityAcessor ?? throw new ArgumentNullException(nameof(entityAcessor));

        public async Task<IOperationResult> ValidateAsync(SelectQuery argument, CancellationToken cancellationToken = default)
        {
            var contact = await _entityAccessor.TryFindAsync(argument, cancellationToken).ConfigureAwait(false);
            if (contact is null)
                return new FailureOperationResult(HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

            return new SuccessOperationResult();
        }
        public async Task<IOperationResult> ValidateAsync(AddCommand argument, CancellationToken cancellationToken = default)
        {
            var contact = await _entityAccessor.TryFindAsync(argument, cancellationToken).ConfigureAwait(false);
            if (contact is not null)
                return new FailureOperationResult(HttpStatusCode.BadRequest, nameof(argument.Name), "Contact already exist");

            return new SuccessOperationResult();
        }
        public async Task<IOperationResult> ValidateAsync(DeleteCommand argument, CancellationToken cancellationToken = default)
        {
            var contact = await _entityAccessor.TryFindAsync(argument, cancellationToken).ConfigureAwait(false);
            if (contact is null)
                return new FailureOperationResult(HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

            return new SuccessOperationResult();
        }
        public async Task<IOperationResult> ValidateAsync(EditCommand argument, CancellationToken cancellationToken = default)
        {
            var contact = await _entityAccessor.TryFindAsync(argument, cancellationToken).ConfigureAwait(false);
            if (contact is null)
                return new FailureOperationResult(HttpStatusCode.NotFound, nameof(argument.Id), "Contact not found");

            argument.Name = contact.Name;
            argument.City = contact.City;
            argument.Address = contact.Address;
            argument.Country = contact.Country;

            return argument.ApplyPatch(argument);
        }
    }
}

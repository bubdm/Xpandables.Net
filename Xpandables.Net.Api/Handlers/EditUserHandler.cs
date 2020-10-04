
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

using Xpandables.Net.Api.Contracts;
using Xpandables.Net.Api.Localization;
using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Storage.Services;
using Xpandables.Net.Commands;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Strings;

using static Xpandables.Net.Validations.ValidationAttributeExtensions;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class EditUserHandler : IAsyncCommandHandler<EditUser>
    {
        private readonly IDataContext _dataContext;
        private readonly IStringCryptography _stringCryptography;

        public EditUserHandler(IDataContext dataContext, IStringCryptography stringCryptography)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _stringCryptography = stringCryptography ?? throw new ArgumentNullException(nameof(stringCryptography));
        }

        public async Task HandleAsync(EditUser command, CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.GetUserAsync(command, true, cancellationToken).ConfigureAwait(false)
                ?? throw CreateValidationException(LocalizationService.PHONE_INVALID, command.Phone, new[] { "Phone" });

            if (command.Email is { }) user.ChangeEmail(new EmailAddress(command.Email), $"Change to {command.Email}");
            if (command.Password is { }) user.ChangePassword(await _stringCryptography.EncryptAsync(command.Password).ConfigureAwait(false), $"Just to change to {command.Password}");
            if (command.Phone is { }) user.ChangePhone(new PhoneNumber(command.Phone), $"Changing from {user.Phone} to {command.Phone}");

            await _dataContext.UpdateEntityAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }
}

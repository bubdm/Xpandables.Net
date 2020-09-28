﻿
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

using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.Api.Storage.Services;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Strings;

namespace Xpandables.Net.Api.Storage
{
    public sealed class UserContextInitializer : IDataContextInitializer
    {
        private readonly IStringCryptography _stringCryptography;

        public UserContextInitializer(IStringCryptography stringCryptography) => _stringCryptography = stringCryptography ?? throw new ArgumentNullException(nameof(stringCryptography));

        public async Task InitializeAsync(IDataContext dataContext, CancellationToken cancellationToken = default)
        {
            if (dataContext.SetOf<User>().Any())
                return;

            var user = await dataContext.CreateNewUser("+33123456789", "motdepasse", "email@email.com", _stringCryptography);

            await dataContext.AddEntityAsync(user, cancellationToken).ConfigureAwait(false);
            await dataContext.PersistAsync(cancellationToken).ConfigureAwait(false);

        }
    }
}
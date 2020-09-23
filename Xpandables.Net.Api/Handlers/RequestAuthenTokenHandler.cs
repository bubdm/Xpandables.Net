
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
using Xpandables.Net.Api.Storage.Services;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Http;
using Xpandables.Net.Optionals;
using Xpandables.Net.Queries;

using static Xpandables.Net.Validations.ValidationAttributeExtensions;

namespace Xpandables.Net.Api.Handlers
{
    public sealed class RequestAuthenTokenHandler : IAsyncQueryHandler<RequestAuthenToken, AuthenToken>
    {
        private readonly IDataContext _dataContext;
        private readonly IHttpTokenEngine _tokenService;

        public RequestAuthenTokenHandler(IDataContext dataContext, IHttpTokenEngine tokenService)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Optional<AuthenToken>> HandleAsync(RequestAuthenToken query, CancellationToken cancellationToken = default)
        {
            var user = await _dataContext.GetNoTrackingUserAsync(query, cancellationToken).ConfigureAwait(false)
                ?? throw CreateValidationException(LocalizationService.PHONE_INVALID, query.Phone, new[] { nameof(query.Phone) });

            var tokenClaims = user.CreateTokenClaims();
            var token = _tokenService.WriteToken(tokenClaims);

            return AuthenToken
                .Create()
                .AddExpiry(token.Expiry)
                .AddToken(token.Value)
                .AddType(token.Type);
        }
    }
}

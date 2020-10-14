
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

using Microsoft.Extensions.Caching.Memory;

using Xpandables.Net.Api.Localization;
using Xpandables.Net.Strings;

using static Xpandables.Net.Validations.ValidationAttributeExtensions;

namespace Xpandables.Net.Api.Services.Implementations
{
    public sealed class TwoFactorService : ITwoFactorService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IStringGenerator _stringGenerator;

        public TwoFactorService(IMemoryCache memoryCache, IStringGenerator stringGenerator)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _stringGenerator = stringGenerator ?? throw new ArgumentNullException(nameof(stringGenerator));
        }
        public async Task<string> SendTwoFactorAsync(TwoFactorOptions options, CancellationToken cancellationToken = default)
        {
            var tempCode = _stringGenerator.Generate(6, "0123456789");
            _memoryCache.Set(options.Phone, tempCode, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTime.UtcNow.AddMinutes(2) });
            return await Task.FromResult($"{LocalizationService.TWO_FACTOR_AUTHEN__SENT_OK} ({ tempCode})");
        }

        public async Task ValidateTwoFactorAsync(TwoFactorOptions options, CancellationToken cancellationToken = default)
        {
            var tempCode = _memoryCache.Get<string>(options.Phone);
            if (options.Code != tempCode)
                throw CreateValidationException(LocalizationService.TWO_FACTOR_AUTHEN_CODE_INVALID, options.Code, new[] { nameof(options.Code) });

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}


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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Xpandables.Net.Api.Models;
using Xpandables.Net.Api.Models.Domains;
using Xpandables.Net.EntityFramework;
using Xpandables.Net.Expressions;
using Xpandables.Net.Strings;

namespace Xpandables.Net.Api.Storage.Services
{
    public static class UserService
    {
        public static async Task<User> GetUserAsync(this IDataContext @this, IQueryExpression<User> query, bool asTracking = false, CancellationToken cancellationToken = default)
            => await @this
                .SetOf(query)
                .AsTracking(asTracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstOrDefaultAsync(query.GetExpression(), cancellationToken)
                .ConfigureAwait(false);

        public static async Task<User> CreateNewUser(this IDataContext _, string phone, string password, string email, IStringCryptography stringCryptography)
        {
            var passwordEncrypted = await stringCryptography.EncryptAsync(password).ConfigureAwait(false);
            return User.Create(new PhoneNumber(phone), passwordEncrypted, new EmailAddress(email));
        }

        public static async IAsyncEnumerable<EventLog> GetNoTrackingEventLogAsync(
            this IDataContext @this, IQueryExpression<User> query, IQueryExpression<EventLog, bool> criteria, [EnumeratorCancellation] CancellationToken _ = default)
        {
            await foreach (var eventLog in @this.SetOf(query).Include(i => i.EventLogs).AsNoTracking().Where(query.GetExpression()).SelectMany(user => user.EventLogs).Where(criteria.GetExpression()).ToAsyncEnumerable())
                yield return eventLog;
        }
    }
}

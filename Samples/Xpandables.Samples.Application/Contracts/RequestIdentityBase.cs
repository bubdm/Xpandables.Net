using System;
using System.Linq.Expressions;

using Xpandables.Net5.Identities;
using Xpandables.Samples.Domain;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Application.Contracts
{
    public abstract class RequestIdentityBase : IdentityExpression<TokenClaims, User>
    {
        protected override Expression<Func<User, bool>> BuildExpression()
            => user => user.Email == Identity.Email && user.IsActive && !user.IsDeleted;
    }
}

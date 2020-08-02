using System;
using System.Linq.Expressions;

using Xpandables.Net.Identities;
using Xpandables.Samples.Domain;
using Xpandables.Samples.Domain.Models;

namespace Xpandables.Samples.Business.Contracts
{
    public abstract class RequestIdentityBase : IdentityDataExpression<TokenClaims, User>
    {
        protected override Expression<Func<User, bool>> BuildExpression()
            => user => user.Email == Identity.Email && user.IsActive && !user.IsDeleted;
    }
}

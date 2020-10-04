
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
using System.ComponentModel;
using System.Linq.Expressions;

using Xpandables.Net.Expressions;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// Defines an implementation of <see cref="ITokenClaim"/> with a protected property that holds token claims information
    /// of any type in a security context.
    /// This class is used with <see cref="ITokenClaimDecorator"/> and its decorator class.
    /// </summary>
    public abstract class TokenClaim : ITokenClaim
    {
        object ITokenClaim.Claims => Claims;

        /// <summary>
        /// Contains the protected instance of token claims.
        /// This value is provided by an implementation of <see cref="ITokenClaimProvider" /> using a decorator.
        /// </summary>
        protected object Claims { get; private set; } = default!;

        /// <summary>
        /// Sets the <see cref="ITokenClaim.Claims" /> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="claims">The claims data to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetClaims(object claims) => Claims = claims ?? throw new ArgumentNullException(nameof(claims));
    }

    /// <summary>
    /// Defines an implementation of <see cref="ITokenClaim{TClaims}"/> with a protected property that holds token claims information
    /// of generic type in a security context.
    /// This class is used with <see cref="ITokenClaimDecorator"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TClaims">The type of the token claims.</typeparam>
    public abstract class TokenClaim<TClaims> : TokenClaim, ITokenClaim<TClaims>
        where TClaims : class
    {
        TClaims ITokenClaim<TClaims>.Claims => Claims;

        /// <summary>
        /// Contains the protected instance of claims of <typeparamref name="TClaims"/> type.
        /// This value is provided by an implementation of <see cref="ITokenClaimProvider" /> using a decorator.
        /// </summary>
        protected new TClaims Claims => (TClaims)base.Claims;

        /// <summary>
        /// Sets the <see cref="Claims"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="claims">The claims to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetClaims(TClaims claims) => SetClaims((object)claims);
    }

    /// <summary>
    /// Defines an implementation of <see cref="ITokenClaimExpression{TClaims, TSource}"/> with a protected property that holds token claims information
    /// of generic type in a security context.
    /// This class implements the <see cref="IQueryExpression{TSource}"/> interface and derives from <see cref="TokenClaim{TClaims}"/>.
    /// You must override the <see cref="BuildExpression"/> method in order to provide a custom behavior.
    /// This class is used with <see cref="ITokenClaimDecorator"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TClaims">The type of the claims data.</typeparam>
    /// /// <typeparam name="TSource">The type of the data source</typeparam>
    public abstract class TokenClaimExpression<TClaims, TSource> : TokenClaim<TClaims>, ITokenClaimExpression<TClaims, TSource>
        where TClaims : class
        where TSource : class
    {
        /// <summary>
        /// Gets the expression tree for the underlying instance.
        /// </summary>
        public Expression<Func<TSource, bool>> GetExpression() => BuildExpression();

        /// <summary>
        /// When implemented in derived class, this method will return the expression
        /// to be used for the <see langword="Where"/> clause in a query to a db context.
        /// </summary>
        protected virtual Expression<Func<TSource, bool>> BuildExpression() => _ => true;

#pragma warning disable CS1591
        public static implicit operator Expression<Func<TSource, bool>>(TokenClaimExpression<TClaims, TSource> criteria)
             => criteria?.GetExpression() ?? (_ => true);

        public static implicit operator Func<TSource, bool>(TokenClaimExpression<TClaims, TSource> criteria)
            => criteria?.GetExpression().Compile() ?? (_ => true);
    }
}

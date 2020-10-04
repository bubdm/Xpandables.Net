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

using System.ComponentModel;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// Provides with a protected property that holds token claims information of any type in a security context.
    /// This class is used with <see cref="ITokenClaimDecorator"/> and its decorator class.
    /// </summary>
    public interface ITokenClaim
    {
        /// <summary>
        /// Contains an instance of claims.
        /// This value is provided by an implementation of <see cref="ITokenClaimProvider"/> using a decorator.
        /// </summary>
        protected object Claims { get; }

        /// <summary>
        /// Sets the <see cref="Claims"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="claims">The claims to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetClaims(object claims);
    }

    /// <summary>
    /// Provides with a protected property that holds token claims information of generic type in a security context.
    /// This class is used with <see cref="ITokenClaimDecorator"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TClaims">The type of claims.</typeparam>
    public interface ITokenClaim<TClaims> : ITokenClaim
        where TClaims : class
    {
        /// <summary>
        /// Contains an instance of claims.
        /// This value is provided by an implementation of <see cref="ITokenClaimProvider"/> using a decorator.
        /// </summary>
        protected new TClaims Claims { get; }

        /// <summary>
        /// Sets the <see cref="Claims"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="claims">The claims to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetClaims(TClaims claims);

        /// <summary>
        /// Sets the <see cref="Claims"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="claims">The claims to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void SetClaims(object claims) => SetClaims((TClaims)claims);
    }
}

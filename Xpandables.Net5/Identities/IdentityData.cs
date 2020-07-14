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
using System.ComponentModel;
using System.Linq.Expressions;

using Xpandables.Net5.Expressions;

namespace Xpandables.Net5.Identities
{
    /// <summary>
    /// Defines an implementation of <see cref="IIdentityData"/> with a property that holds identity information
    /// of any type in a security context.
    /// This class is used with <see cref="IBehaviorIdentity"/> and its decorator class.
    /// </summary>
    public abstract class IdentityData : IIdentityData
    {
        /// <summary>
        /// Contains an instance of identity data.
        /// This value is provided by an implementation of <see cref="IIdentityProvider" /> using a decorator.
        /// </summary>
        public object Identity { get; internal set; } = default!;

        /// <summary>
        /// Sets the <see cref="IIdentityData.Identity" /> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="identity">The identity data to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetIdentity(object identity)
        {
            _ = identity ?? throw new ArgumentNullException(nameof(identity));
            Identity = identity;
        }
    }

    /// <summary>
    /// Defines an implementation of <see cref="IIdentityData{TUser}"/> with a property that holds identity information
    /// of generic type in a security context.
    /// This class is used with <see cref="IBehaviorIdentity"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TIdentity">The type of the identity.</typeparam>
    public abstract class IdentityData<TIdentity> : IdentityData, IIdentityData<TIdentity>
        where TIdentity : class
    {
        /// <summary>
        /// Contains an instance of identity data.
        /// This value is provided by an implementation of <see cref="IIdentityProvider" /> using a decorator.
        /// </summary>
        public new TIdentity Identity => (TIdentity)base.Identity;
    }

    /// <summary>
    /// Defines an implementation of <see cref="IIdentityExpression{TUser, TSource}"/> with a property that holds identity information
    /// of generic type in a security context.
    /// This class implements the <see cref="IQueryExpression{TSource}"/> interface.
    /// You must override the <see cref="BuildExpression"/> method in order to provide a custom behavior.
    /// This class is used with <see cref="IBehaviorIdentity"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TIdentity">The type of the identity data.</typeparam>
    /// /// <typeparam name="TSource">The type of the data source</typeparam>
    public abstract class IdentityExpression<TIdentity, TSource> : IdentityData<TIdentity>, IIdentityExpression<TIdentity, TSource>
        where TIdentity : class
        where TSource : class
    {
        /// <summary>
        /// Gets the expression tree for the underlying instance.
        /// </summary>
        public Expression<Func<TSource, bool>> GetExpression() => BuildExpression();

        /// <summary>
        /// When implemented in derived class, this method will return the expression
        /// to be used for the <see langword="Where"/> clause in a query.
        /// </summary>
        protected virtual Expression<Func<TSource, bool>> BuildExpression() => _ => true;

#pragma warning disable CS1591
#pragma warning disable CA2225
        public static implicit operator Expression<Func<TSource, bool>>(IdentityExpression<TIdentity, TSource> criteria)
             => criteria?.GetExpression() ?? (_ => true);

        public static implicit operator Func<TSource, bool>(IdentityExpression<TIdentity, TSource> criteria)
            => criteria?.GetExpression().Compile() ?? (_ => true);
    }
}
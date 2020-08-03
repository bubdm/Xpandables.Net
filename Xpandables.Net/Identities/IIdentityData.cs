
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

using Xpandables.Net.Expressions;

namespace Xpandables.Net.Identities
{
    /// <summary>
    /// Provides with a property that holds identity information of any type in a security context.
    /// This class is used with <see cref="IBehaviorIdentity"/> and its decorator class.
    /// </summary>
    public interface IIdentityData
    {
        /// <summary>
        /// Contains an instance of secured data.
        /// This value is provided by an implementation of <see cref="IIdentityDataProvider"/> using a decorator.
        /// </summary>
        object Identity { get; }

        /// <summary>
        /// Sets the <see cref="Identity"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="identity">The identity data to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetIdentity(object identity);
    }

    /// <summary>
    /// Provides with a property that holds identity information of generic type in a security context.
    /// This class is used with <see cref="IBehaviorIdentity"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public interface IIdentityData<TData> : IIdentityData
        where TData : class
    {
        /// <summary>
        /// Contains an instance of identity data.
        /// This value is provided by an implementation of <see cref="IIdentityDataProvider"/> using a decorator.
        /// </summary>
        new TData Identity { get; }

        /// <summary>
        /// Sets the <see cref="Identity"/> with the specified value.
        /// This method get called by the decorator class.
        /// </summary>
        /// <param name="identity">The identity data to be used.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetIdentity(TData identity)
        {
            _ = identity ?? throw new ArgumentNullException(nameof(identity));
            SetIdentity((object)identity);
        }
    }

    /// <summary>
    /// Provides with a property that holds identity information of generic type in a security context.
    /// This interface derives from <see cref="IQueryExpression{TSource}"/> interface.
    /// This interface is used with <see cref="IBehaviorIdentity"/> and its decorator class.
    /// </summary>
    /// <typeparam name="TIdentity">The type of the identity data.</typeparam>
    /// <typeparam name="TSource">The type of the data source</typeparam>
    public interface IIdentityExpression<TIdentity, TSource> : IIdentityData<TIdentity>, IQueryExpression<TSource>
        where TIdentity : class
        where TSource : class
    { }
}

﻿/************************************************************************************************************
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

using System.Diagnostics.CodeAnalysis;
using System;

namespace Xpandables.Net5.Identities
{
    /// <summary>
    /// Provides with a method to retrieve identity.
    /// You should provide an implementation of this interface that will be used with the <see cref="CommandIdentityBehavior{TCommand}"/>
    /// or <see cref="QueryIdentityBehavior{TQuery, TResult}"/> decorator to fill the target instance with the identity.
    /// </summary>
    public interface IIdentityProvider
    {
        /// <summary>
        /// Returns an instance that contains identity or throw an exception if not found.
        /// </summary>
        /// <exception cref="InvalidOperationException">Failed to find secured data.</exception>
        [return: NotNull]
        object GetIdentity();
    }
}

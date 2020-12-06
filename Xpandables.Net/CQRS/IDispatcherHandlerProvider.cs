
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// Defines set of methods to retrieve handlers of specific type.
    /// </summary>
    public interface IDispatcherHandlerProvider
    {
        /// <summary>
        /// Returns the handler of the <typeparamref name="THandler"/> type.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler to look for.</typeparam>
        /// <returns>A handler of the <typeparamref name="THandler"/> type -or- null if there is no <typeparamref name="THandler"/> type.</returns>
        [return: MaybeNull]
        public sealed THandler GetHandler<THandler>() where THandler : class => GetHandler(typeof(THandler)) as THandler;

        /// <summary>
        /// Returns all the handlers of the <typeparamref name="THandler"/> type.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler to look for.</typeparam>
        /// <returns>A collection of handlers of the <typeparamref name="THandler"/> type -or- empty collection if there is no <typeparamref name="THandler"/> type.</returns>
        [return: MaybeNull]
        public sealed IEnumerable<THandler> GetHandlers<THandler>() where THandler : class => GetHandlers(typeof(THandler)) as IEnumerable<THandler> ?? Enumerable.Empty<THandler>();

        /// <summary>
        /// Returns the handler of the specified type.
        /// </summary>
        /// <param name="handlerType">An object that specifies the type of handler object to get.</param>
        /// <returns>A handler of the <paramref name="handlerType"/> -or- null if there is no handler of <paramref name="handlerType"/>.</returns>
        [return: MaybeNull]
        object GetHandler(Type handlerType);

        /// <summary>
        /// Returns all the handlers of the specified type.
        /// </summary>
        /// <param name="handlerType">An object that specifies the type of handler object to get.</param>
        /// <returns>A collection of handlers of the <paramref name="handlerType"/> -or- empty collection if there is no handler of <paramref name="handlerType"/>.</returns>
        IEnumerable<object> GetHandlers(Type handlerType);
    }
}

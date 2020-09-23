
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

using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers
{
    /// <summary>
    /// The default implementation for <see cref="IDispatcher"/>.
    /// Implements methods to execute the <see cref="IAsyncQueryHandler{TQuery, TResult}"/> and
    /// <see cref="IAsyncCommandHandler{TCommand}"/> process dynamically.
    /// This class can not be inherited.
    /// </summary>
    public sealed class Dispatcher : IDispatcher
    {
        /// <summary>
        /// Gets the handlers service provider.
        /// </summary>
        public IDispatcherHandlerProvider DispatcherHandlerProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="dispatcherHandlerProvider">The handler service provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherHandlerProvider"/> is null.</exception>
        public Dispatcher(IDispatcherHandlerProvider dispatcherHandlerProvider)
            => DispatcherHandlerProvider = dispatcherHandlerProvider ?? throw new ArgumentNullException(nameof(dispatcherHandlerProvider));
    }
}
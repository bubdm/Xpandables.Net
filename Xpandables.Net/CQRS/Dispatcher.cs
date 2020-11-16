
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// The implementation for <see cref="IDispatcher"/>.
    /// Implements methods to execute the <see cref="IAsyncEnumerableQueryHandler{TQuery, TResult}"/>, <see cref="IAsyncQueryHandler{TQuery, TResult}"/> and
    /// <see cref="IAsyncCommandHandler{TCommand}"/> process dynamically.
    /// </summary>
    public class Dispatcher : IDispatcher
    {
        private readonly IDispatcherHandlerProvider _dispatcherHandlerProvider;

        IDispatcherHandlerProvider IDispatcher.DispatcherHandlerProvider => _dispatcherHandlerProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher"/> class with the handlers provider.
        /// </summary>
        /// <param name="dispatcherHandlerProvider">The handlers provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherHandlerProvider"/> is null.</exception>
        public Dispatcher(IDispatcherHandlerProvider dispatcherHandlerProvider)
            => _dispatcherHandlerProvider = dispatcherHandlerProvider ?? throw new ArgumentNullException(nameof(dispatcherHandlerProvider));
    }
}
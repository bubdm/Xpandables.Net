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
using System.Threading.Tasks;

namespace Xpandables.Net.Commands
{
    /// <summary>
    /// Provides with a method to asynchronously enqueue an internal command of specific type that implements <see cref="IInternalCommand"/> interface.
    /// </summary>
    public interface IInternalCommandScheduler
    {
        /// <summary>
        /// Asynchronously enqueues the specified internal command.
        /// </summary>
        /// <param name="command">The internal command instance to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <returns>A task that represents an asynchronous operation.</returns>
        Task EnqueueAsync<TInternalCommand>(TInternalCommand command) where TInternalCommand : class, IInternalCommand;
    }
}

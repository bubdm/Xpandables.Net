
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
using System.Threading;
using System.Threading.Tasks;

namespace Xpandables.Net.Commands
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IInternalCommandHandler{TInternalCommand}"/> interface.
    /// </summary>
    /// <typeparam name="TInternalCommand">Type of internal command to act on.</typeparam>
    public abstract class InternalCommandHandler<TInternalCommand> : OperationResultBase, IInternalCommandHandler<TInternalCommand>
        where TInternalCommand : class, IInternalCommand
    {
        /// <summary>
        /// Asynchronously handles the specified internal command.
        /// </summary>
        /// <param name="command">The internal command instance to act on.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command" /> is null.</exception>
        /// <returns>A task that represents an object of <see cref="IOperationResult"/>.</returns>
        public abstract Task<IOperationResult> HandleAsync(TInternalCommand command, CancellationToken cancellationToken = default);
    }
}

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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Defines a method to publish events.
    /// The implementation must be thread-safe when working in a multi-threaded environment.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Asynchronously publishes the events across all domain/integration handlers.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <remarks>if errors, see Debug or Trace.</remarks>
        Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}

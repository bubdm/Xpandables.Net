
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Represents a set of methods to read/write email event to/from an event store.
    /// </summary>
    /// <typeparam name="TEmailMessage">the type of the message.</typeparam>
    public interface IEmailEventRepository<TEmailMessage> : IEventRepository<EmailEventStoreEntity>
        where TEmailMessage : class
    {
        /// <summary>
        /// Gets or sets the current <see cref="JsonSerializerOptions"/> to be used for serialization.
        /// </summary>
        JsonSerializerOptions? SerializerOptions { get; set; }

        /// <summary>
        /// Gets or sets the current <see cref="JsonDocumentOptions"/> to be used for <see cref="JsonDocument"/> parsing.
        /// </summary>
        JsonDocumentOptions DocumentOptions { get; set; }

        /// <summary>
        /// Asynchronously appends the specified email event.
        /// </summary>
        /// <param name="event">Then target email to be appended.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task AppendEventAsync(IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default);
    }
}

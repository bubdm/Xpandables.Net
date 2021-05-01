
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

using Xpandables.Net.Database;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Events.IntegrationEvents
{
    /// <summary>
    /// The default implementation of <see cref="IIntegrationEventPublisher"/>.
    /// You can derive from this class in order to customize its behaviors.
    /// </summary>
    public class IntegrationEventPublisher : IIntegrationEventPublisher
    {
        private readonly IDataContext _context;

        /// <summary>
        /// Constructs a new instance of <see cref="IntegrationEventPublisher"/>.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public IntegrationEventPublisher(IDataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Persists the target event to the context.
        /// </summary>
        /// <param name="event">The event to be published.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public virtual async Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var messageEntity = new IntegrationEventEntity(@event);
            await _context.InsertAsync(messageEntity, cancellationToken).ConfigureAwait(false);
        }
    }
}

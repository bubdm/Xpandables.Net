
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
using Xpandables.Net.Aggregates;
using Xpandables.Net.Database;

namespace Xpandables.Net.DomainEvents
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as a event domain for <see cref="IAggregate{TAggregateId}"/>.
    /// This kind of events are published before <see cref="IDataContextPersistence.SaveChangesAsync(System.Threading.CancellationToken)"/>.
    /// In case of exception in target event handlers, you can rollback the operation using transaction.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    public interface IDomainEvent<TAggregateId> : IEvent<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the version of the related aggregate being saved.
        /// </summary>
        AggregateVersion Version { get; }

        /// <summary>
        /// Returns a new instance of the domain event with appropriate values.
        /// </summary>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="version">The aggregate version.</param>
        /// <returns>A new instance of the domain event.</returns>
        IDomainEvent<TAggregateId> WithAggregate(TAggregateId aggregateId, AggregateVersion version);
    }
}

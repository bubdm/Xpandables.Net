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

namespace Xpandables.Net.Events.DomainEvents
{
    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="IDomainEvent"/>.
    /// </summary>
    [Serializable]
    public abstract class DomainEvent : EventBase, IDomainEvent, IEquatable<DomainEvent>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="DomainEvent"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="version">The version of the  related aggregate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="aggregateId"/> is null.</exception>
        protected DomainEvent(Guid aggregateId, long version) : base(aggregateId) => Version = version;

        /// <summary>
        /// Constructs a new instance of <see cref="DomainEvent"/>.
        /// </summary>
        /// <param name="version">The version of the related aggregate.</param>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="occurredOn">When the event occurred.</param>
        /// <param name="createdBy">The user name.</param>
        protected DomainEvent(long version, Guid aggregateId, Guid eventId, DateTimeOffset occurredOn, string createdBy)
            : base(aggregateId, eventId, occurredOn, createdBy)
            => Version = version;

        /// <summary>
        /// Gets the version of the related aggregate being saved.
        /// </summary>
        public long Version { get; protected set; }

        /// <summary>
        /// Indicates whether the current event is equal to another event of the same type.
        /// </summary>
        /// <param name="other">A domain event to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(DomainEvent? other) => other is not null && Equals(other);

        /// <summary>
        /// When overridden in derived class, returns a custom instance of the domain event with appropriate values.
        /// </summary>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="version">The aggregate version.</param>
        /// <returns>A new instance of the domain event.</returns>
        public virtual IDomainEvent WithAggregate(Guid aggregateId, long version)
        {
            AggregateId = aggregateId;
            Version = version;
            return this;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is DomainEvent domainEvent && Equals(domainEvent);

        /// <summary>
        /// Serves as the default hash function for domain event.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => HashCode.Combine(Guid, AggregateId, Version);
    }
}

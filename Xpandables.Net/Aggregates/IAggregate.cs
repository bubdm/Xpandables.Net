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

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Defines base properties for an aggregate that is identified by a property of <see cref="IAggregateId"/> type.
    /// Aggregate is a pattern in Domain-Driven Design.
    /// A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    public interface IAggregate : ICommandQueryEvent
    {
        /// <summary>
        /// Gets the unique identifier of the aggregate.
        /// </summary>
        IAggregateId AggregateId { get; }

        /// <summary>
        /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        AggregateVersion Version { get; }

        /// <summary>
        /// Determines whether or not the underlying instance is a empty one.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public virtual bool IsEmpty => AggregateId?.IsEmpty() ?? true;
    }

    /// <summary>
    /// Defines base properties for an aggregate that is identified by a property of <typeparamref name="TAggregateId"/> type.
    /// Aggregate is a pattern in Domain-Driven Design.
    /// A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    /// <typeparam name="TAggregateId">The type the aggregate identity.</typeparam>
    public interface IAggregate<TAggregateId> : IAggregate
        where TAggregateId : class, IAggregateId
    {
        /// <summary>
        /// Gets the unique identifier of the aggregate.
        /// </summary>
        new TAggregateId AggregateId { get; }

        IAggregateId IAggregate.AggregateId => AggregateId;
    }
}
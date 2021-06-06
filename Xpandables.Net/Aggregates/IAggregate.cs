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
    /// Identifies a command that targets an aggregate. In association with <see cref="Decorators.IPersistenceDecorator"/>, allows aggregate events persistence.
    /// </summary>
    public interface IAggregate { }

    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    /// <typeparam name="TAggregateId">The type the aggregate identity.</typeparam>
    public interface IAggregate<TAggregateId> : ICommandQueryEvent
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the unique identifier of the aggregate.
        /// </summary>
        TAggregateId AggregateId { get; }

        /// <summary>
        ///   /// Gets the current version of the instance, the default value is -1.
        /// </summary>
        long Version { get; }

        /// <summary>
        /// Determines whether or not the underlying instance is a empty one.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if so, otherwise <see langword="false"/></returns>
        public bool IsEmpty => AggregateId?.IsEmpty() ?? true;
    }
}
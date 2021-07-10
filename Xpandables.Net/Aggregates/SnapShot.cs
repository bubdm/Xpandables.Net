
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
using System.Text.Json.Serialization;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// Represents a snapshot to be read.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    [Serializable]
    public sealed class SnapShot<TAggregateId> : Event<TAggregateId>, ISnapShot<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Constructs a new instance of <see cref="SnapShot{TAggregateId}"/>.
        /// </summary>
        /// <param name="memento">the expected memento instance.</param>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="version">The version.</param>
        /// <exception cref="ArgumentException">The <paramref name="memento"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="aggregateId"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="version"/> is null.</exception>
        [JsonConstructor]
        public SnapShot(IMemento memento, TAggregateId aggregateId, AggregateVersion version)
            : base(aggregateId)
        {
            Memento = memento ?? throw new ArgumentNullException(nameof(memento));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        ///<inheritdoc/>
        public IMemento Memento { get; }

        ///<inheritdoc/>
        public AggregateVersion Version { get; }
    }
}

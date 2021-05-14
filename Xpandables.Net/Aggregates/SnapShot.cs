
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
    [Serializable]
    public sealed class SnapShot : ISnapShot
    {
        /// <summary>
        /// Constructs a new instance of <see cref="SnapShot"/>.
        /// </summary>
        /// <param name="memento">the expected memento instance.</param>
        /// <param name="aggregateId">The target aggregate identifier.</param>
        /// <param name="version">The version.</param>
        [JsonConstructor]
        public SnapShot(IMemento memento, Guid aggregateId, long version)
        {
            Memento = memento ?? throw new ArgumentNullException(nameof(memento));
            AggregateId = aggregateId;
            Version = version;
        }

        ///<inheritdoc/>
        public IMemento Memento { get; }

        ///<inheritdoc/>
        public Guid AggregateId { get; }

        ///<inheritdoc/>
        public long Version { get; }
    }
}

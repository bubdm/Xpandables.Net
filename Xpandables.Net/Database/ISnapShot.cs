
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

namespace Xpandables.Net.Database
{
    /// <summary>
    /// Defines properties of a snapshot.
    /// </summary>
    public interface ISnapShot
    {
        /// <summary>
        /// Gets the memento linked to the aggregate.
        /// </summary>
        IMemento Memento { get; }

        /// <summary>
        /// Gets the target aggregate identifier.
        /// </summary>
        Guid AggregateId { get; }

        /// <summary>
        /// Gets the version of the snapshot.
        /// </summary>
        long Version { get; }
    }
}

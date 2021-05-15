
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

namespace Xpandables.Net
{
    /// <summary>
    /// Provides with shared implementation members for <see cref="ICommandQueryEvent"/>.
    /// </summary>
    public abstract class CommandQueryEvent : ICommandQueryEvent
    {
        /// <summary>
        /// Constructs a default instance of <see cref="CommandQueryEvent"/> class
        /// that initializes <see cref="Guid"/>, <see cref="OccurredOn"/> and <see cref="CreatedBy"/> properties.
        /// </summary>
        protected CommandQueryEvent()
        {
            Guid = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            CreatedBy = Environment.UserName;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="CommandQueryEvent"/> class with the specified information.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="occurredOn">When the event occurred.</param>
        /// <param name="createdBy">The user name running the instance.</param>
        protected CommandQueryEvent(Guid guid, DateTimeOffset occurredOn, string createdBy)
        {
            Guid = guid;
            OccurredOn = occurredOn;
            CreatedBy = createdBy;
        }

        ///<inheritdoc/>
        public Guid Guid { get; protected set; }

        ///<inheritdoc/>
        public DateTimeOffset OccurredOn { get; protected set; }

        ///<inheritdoc/>
        public string CreatedBy { get; protected set; }
    }
}

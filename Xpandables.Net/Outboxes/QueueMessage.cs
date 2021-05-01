
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

namespace Xpandables.Net.Enqueues
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IQueueMessage"/> interface.
    /// </summary>
    public abstract class QueueMessage : IQueueMessage
    {
        /// <summary>
        /// Constructs a new instance of <see cref="QueueMessage"/> with the target type.
        /// </summary>
        /// <param name="queueType">The target queue message type.</param>
        /// <param name="occurredOn">When the message occurred.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="queueType"/> is null.</exception>
        protected QueueMessage(string queueType, DateTimeOffset occurredOn)
        {
            QueueType = queueType ?? throw new ArgumentNullException(nameof(queueType));
            OccurredOn = occurredOn;
        }

        /// <summary>
        /// Gets the queue message type.
        /// </summary>
        public string QueueType { get; }

        /// <summary>
        /// Gets the message occurred.
        /// </summary>
        public DateTimeOffset OccurredOn { get; } 
    }
}

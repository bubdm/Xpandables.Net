
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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// The queue domain object base implementation of <see cref="IEntityQueue"/>.
    /// This is an <see langword="abstract"/>class.
    /// </summary>
    public abstract class EntityQueue : AggregateRoot, IEntityQueue
    {
        /// <summary>
        /// Gets the queue type.
        /// </summary>
        public string QueueType { get; }

        /// <summary>
        /// Gets the .Net Framework content type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the JSON <see cref="string"/> representation of the type.
        /// </summary>
        public string JsonTypeContent { get; }

        private QueueStatus _status;

        /// <summary>
        /// Constructs a new instance of <see cref="EntityQueue"/> with its properties.
        /// </summary>
        /// <param name="queueType">The queue type.</param>
        /// <param name="type">The .Net assembly content type.</param>
        /// <param name="jsonTypeContent">The JSON string content.</param>
        /// <param name="status">The queue status.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="queueType"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="jsonTypeContent"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        protected EntityQueue(string queueType, string type, string jsonTypeContent, QueueStatus status)
        {
            QueueType = queueType ?? throw new ArgumentNullException(nameof(queueType));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            JsonTypeContent = jsonTypeContent ?? throw new ArgumentNullException(nameof(jsonTypeContent));
            _status = status;
        }

        /// <summary>
        /// Determines the status of the queue.
        /// The default value is <see cref="QueueStatus.NewlyAdded"/>.
        /// </summary>
        public QueueStatus Status => _status;

        /// <summary>
        /// Sets the entity queue status to <see cref="QueueStatus.NewlyAdded"/>.
        /// </summary>
        public virtual void QueueStatusNewlyAdded() => _status = QueueStatus.NewlyAdded;

        /// <summary>
        /// Sets the entity queue status to <see cref="QueueStatus.AlreadyCollected"/>.
        /// </summary>
        public virtual void QueueStatusAlreadyCollected() => _status = QueueStatus.AlreadyCollected;

        /// <summary>
        /// Sets the entity queue status to <see cref="QueueStatus.ProcessFailed"/>.
        /// </summary>
        public virtual void QueueStatusProcessFailed() => _status = QueueStatus.ProcessFailed;
    }
}

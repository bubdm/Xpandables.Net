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

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Provides with queue message status.
    /// </summary>
    public interface IEntityQueueStatus
    {
        /// <summary>
        /// Determines the status of the queue.
        /// The default value is <see cref="QueueStatus.NewlyAdded"/>.
        /// </summary>
        QueueStatus Status { get; }

        /// <summary>
        /// Sets the entity queue status to <see cref="QueueStatus.NewlyAdded"/>.
        /// </summary>
        void QueueStatusNewlyAdded();

        /// <summary>
        /// Sets the entity queue status to <see cref="QueueStatus.AlreadyCollected"/>.
        /// </summary>
        void QueueStatusAlreadyCollected();

        /// <summary>
        /// Sets the entity queue status to <see cref="QueueStatus.ProcessFailed"/>.
        /// </summary>
        void QueueStatusProcessFailed();
    }

    /// <summary>
    /// Defines the different queue message status.
    /// </summary>
    public enum QueueStatus
    {
        /// <summary>
        /// The queue message has been newly added.
        /// </summary>
        NewlyAdded,

        /// <summary>
        /// The queue message has been already collected.
        /// </summary>
        AlreadyCollected,

        /// <summary>
        /// The process failed to process the queue message.
        /// </summary>
        ProcessFailed
    }
}

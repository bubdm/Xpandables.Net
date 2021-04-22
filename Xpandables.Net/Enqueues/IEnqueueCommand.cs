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

namespace Xpandables.Net.Enqueues
{
    /// <summary>
    /// This interface is used as a marker for enqueue commands.
    /// </summary>
    public interface IEnqueueCommand : ICommandQueryEvent
    {
        /// <summary>
        /// Gets the enqueue type.
        /// </summary>
        string EnqueueType { get; }

        /// <summary>
        /// Determines the status of the enqueue.
        /// The default value is <see cref="EnqueueStatus.NewlyAdded"/>.
        /// </summary>
        EnqueueStatus Status { get; protected set; }

        /// <summary>
        /// Sets the enqueue status of the command to <see cref="EnqueueStatus.NewlyAdded"/>.
        /// </summary>
        public virtual void EnqueueStatusNewlyAdded() => Status = EnqueueStatus.NewlyAdded;

        /// <summary>
        /// Sets the enqueue status of the command to <see cref="EnqueueStatus.AlreadyCollected"/>.
        /// </summary>
        public virtual void EnqueueStatusAlreadyCollected() => Status = EnqueueStatus.AlreadyCollected;

        /// <summary>
        /// Sets the enqueue status of the command to <see cref="EnqueueStatus.ProcessFailed"/>.
        /// </summary>
        public virtual void EnqueueStatusProcessFailed() => Status = EnqueueStatus.ProcessFailed;
    }

    /// <summary>
    /// Defines the different enqueue status.
    /// </summary>
    public enum EnqueueStatus
    {
        /// <summary>
        /// The target enqueue command has been newly added.
        /// </summary>
        NewlyAdded,

        /// <summary>
        /// The target enqueue command has been already collected.
        /// </summary>
        AlreadyCollected,

        /// <summary>
        /// The process failed to process the target enqueue command.
        /// </summary>
        ProcessFailed
    }
}

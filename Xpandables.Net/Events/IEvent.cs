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

namespace Xpandables.Net.Events
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as an event : Domain event or Integration event.
    /// The events are raised using the differed approach described by "Jimmy Bogard"
    /// </summary>
    public interface IEvent : ICommandQueryEvent
    {
        /// <summary>
        /// Determines the publishing status of the event.
        /// The default value is <see cref="PublishingStatus.Ready"/>.
        /// </summary>
        PublishingStatus Status { get; protected set; }

        /// <summary>
        /// Gets When the event was updated.
        /// </summary>
        DateTimeOffset UpdatedOn { get; protected set; }

        /// <summary>
        /// Sets the publishing status of the event to <see cref="PublishingStatus.Ready"/>.
        /// </summary>
        public virtual void PublishingStatusReady()
        {
            UpdatedOn = DateTime.UtcNow;
            Status = PublishingStatus.Ready;
        }

        /// <summary>
        /// Sets the publishing status of the event to <see cref="PublishingStatus.Published"/>.
        /// </summary>
        public virtual void PublishingStatusPublished()
        {
            UpdatedOn = DateTime.UtcNow;
            Status = PublishingStatus.Published;
        }

        /// <summary>
        /// Sets the publishing status of the event to <see cref="PublishingStatus.Failed"/>.
        /// </summary>
        public virtual void PublishingStatusFailed()
        {
            UpdatedOn = DateTime.UtcNow;
            Status = PublishingStatus.Failed;
        }
    }

    /// <summary>
    /// Defines the different publishing event status.
    /// </summary>
    public enum PublishingStatus
    {
        /// <summary>
        /// The target event has be created and ready to be published.
        /// </summary>
        Ready,

        /// <summary>
        /// The target event has been successfully published.
        /// </summary>
        Published,

        /// <summary>
        /// The publish event action in the publisher has failed.
        /// </summary>
        Failed
    }
}

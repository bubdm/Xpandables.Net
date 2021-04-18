
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
    /// Provides with extension methods to access <see cref="IEvent"/> members.
    /// </summary>
    public static class IEventExtensions
    {
        /// <summary>
        /// Returns the status of the current event.
        /// </summary>
        /// <param name="this">The target event.</param>
        /// <returns>The publishing status value.</returns>
        public static PublishingStatus Status(this IEvent @this) => @this.Status;

        /// <summary>
        /// Returns the updated date of the current event.
        /// </summary>
        /// <param name="this">The target event.</param>
        /// <returns>The updated date value.</returns>
        public static DateTimeOffset UpdatedOn(this IEvent @this) => @this.UpdatedOn;

        /// <summary>
        /// Sets the publishing status of the event to <see cref="PublishingStatus.Ready"/>.
        /// </summary>
        /// <param name="this">The target event.</param>
        public static void SetPublishingStatusReady(this IEvent @this) => @this.PublishingStatusReady();

        /// <summary>
        /// Sets the publishing status of the event to <see cref="PublishingStatus.Published"/>.
        /// </summary>
        /// <param name="this">The target event.</param>
        public static void SetPublishingStatusPublished(this IEvent @this) => @this.PublishingStatusPublished();

        /// <summary>
        /// Sets the publishing status of the event to <see cref="PublishingStatus.Failed"/>.
        /// </summary>
        /// <param name="this">The target event.</param>
        public static void SetPublishingStatusFailed(this IEvent @this) => @this.PublishingStatusFailed();
    }
}

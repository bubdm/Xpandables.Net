﻿
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

using Microsoft.AspNetCore.Components;

using System.ComponentModel.DataAnnotations;

using Xpandables.Net.Notifications;

namespace Xpandables.Net.Components
{
    /// <summary>
    /// The notification item component.
    /// </summary>
    public partial class NotificationItem
    {
        [CascadingParameter]
        private NotificationCollection NotificationComponents { get; set; } = default!;

        /// <summary>
        /// Gets or sets the current notification.
        /// </summary>
        [Parameter, Required]
        public Notification Notification { get; set; } = default!;

        /// <summary>
        /// Remove the alert.
        /// </summary>
        protected void RemoveNotificationAsync() => NotificationComponents.RemoveNotificationAsync(Notification);
    }
}

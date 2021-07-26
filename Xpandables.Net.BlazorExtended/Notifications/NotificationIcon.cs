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

namespace Xpandables.Net.Notifications
{
    /// <summary>
    /// Defines the base interface for an notification icon.
    /// </summary>
    public interface INotificationIcon
    {
        /// <summary>
        /// Gets the css class for an notification icon.
        /// </summary>
        string IconClass { get; }
    }

    /// <summary>
    /// Provides with the notification icon.
    /// </summary>
    public readonly struct NotificationIcon
    {
        /// <summary>
        /// Gets the <see cref="NotificationIconDark"/>.
        /// </summary>
        public static INotificationIcon Dark => new NotificationIconDark();

        /// <summary>
        /// Gets the <see cref="NotificationIconError"/>.
        /// </summary>
        public static INotificationIcon Error => new NotificationIconError();

        /// <summary>
        /// Gets the <see cref="NotificationIconInformation"/>.
        /// </summary>
        public static INotificationIcon Info => new NotificationIconInformation();

        /// <summary>
        /// Gets the <see cref="NotificationIconLight"/>.
        /// </summary>
        public static INotificationIcon Light => new NotificationIconLight();

        /// <summary>
        /// Gets the <see cref="NotificationIconPrimary"/>.
        /// </summary>
        public static INotificationIcon Primary => new NotificationIconPrimary();

        /// <summary>
        /// Gets the <see cref="NotificationIconSecondary"/>.
        /// </summary>
        public static INotificationIcon Secondary => new NotificationIconSecondary();

        /// <summary>
        /// Gets the <see cref="NotificationIconSuccess"/>.
        /// </summary>
        public static INotificationIcon Success => new NotificationIconSuccess();

        /// <summary>
        /// Gets the <see cref="NotificationIconWarning"/>.
        /// </summary>
        public static INotificationIcon Warning => new NotificationIconWarning();
    }

    /// <summary>
    /// The Warning notification icon.
    /// </summary>
    public class NotificationIconWarning : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-exclamation-circle";
    }

    /// <summary>
    /// The Error notification icon.
    /// </summary>
    public class NotificationIconError : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-meh";
    }

    /// <summary>
    /// The Information notification icon.
    /// </summary>
    public class NotificationIconInformation : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-info-circle";
    }

    /// <summary>
    /// The Primary notification icon.
    /// </summary>
    public class NotificationIconPrimary : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-info-circle";
    }

    /// <summary>
    /// The Secondary notification icon.
    /// </summary>
    public class NotificationIconSecondary : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-info-circle";
    }

    /// <summary>
    /// The Success notification icon.
    /// </summary>
    public class NotificationIconSuccess : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-check-circle";
    }

    /// <summary>
    /// The Dark notification icon.
    /// </summary>
    public class NotificationIconDark : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-moon";
    }

    /// <summary>
    /// The Light notification icon.
    /// </summary>
    public class NotificationIconLight : INotificationIcon
    {
        ///<inheritdoc/>
        public string IconClass => "fas fa-sun";
    }
}

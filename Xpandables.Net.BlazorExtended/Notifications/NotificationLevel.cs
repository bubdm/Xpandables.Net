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
    /// Defines the base interface for an notification level.
    /// </summary>
    public interface INotificationLevel
    {
        /// <summary>
        /// Gets the css class fo the notification.
        /// </summary>
        string LevelClass { get; }
    }

    /// <summary>
    /// Provides with the notification level.
    /// </summary>
    public readonly struct NotificationLevel
    {
        /// <summary>
        /// Gets the <see cref="NotificationLevelInformation"/>.
        /// </summary>
        public static INotificationLevel Info => new NotificationLevelInformation();

        /// <summary>
        /// Gets the <see cref="NotificationLevelDark"/>.
        /// </summary>
        public static INotificationLevel Dark => new NotificationLevelDark();

        /// <summary>
        /// Gets the <see cref="NotificationLevelLight"/>.
        /// </summary>
        public static INotificationLevel Light => new NotificationLevelLight();

        /// <summary>
        /// Gets the <see cref="NotificationLevelError"/>.
        /// </summary>
        public static INotificationLevel Error => new NotificationLevelError();

        /// <summary>
        /// Gets the <see cref="NotificationLevelPrimay"/>.
        /// </summary>
        public static INotificationLevel Primary => new NotificationLevelPrimay();

        /// <summary>
        /// Gets the <see cref="NotificationLevelSecondary"/>.
        /// </summary>
        public static INotificationLevel Secondary => new NotificationLevelSecondary();

        /// <summary>
        /// Gets the <see cref="NotificationLevelSuccess"/>.
        /// </summary>
        public static INotificationLevel Success => new NotificationLevelSuccess();

        /// <summary>
        /// Gets the <see cref="NotificationLevelWarning"/>.
        /// </summary>
        public static INotificationLevel Warning => new NotificationLevelWarning();
    }

    /// <summary>
    /// The Information notification level.
    /// </summary>
    public class NotificationLevelInformation : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-info alert-dismissable";
    }

    /// <summary>
    /// The Error notification level.
    /// </summary>
    public class NotificationLevelError : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-danger alert-dismissable";
    }

    /// <summary>
    /// The Warning notification level.
    /// </summary>
    public class NotificationLevelWarning : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-warning alert-dismissable";
    }

    /// <summary>
    /// The Success notification level.
    /// </summary>
    public class NotificationLevelSuccess : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-success alert-dismissable";
    }

    /// <summary>
    /// The Secondary notification level.
    /// </summary>
    public class NotificationLevelSecondary : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-secondary alert-dismissable";
    }

    /// <summary>
    /// The Primay notification level.
    /// </summary>
    public class NotificationLevelPrimay : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-primary alert-dismissable";
    }

    /// <summary>
    /// The light notification level.
    /// </summary>
    public class NotificationLevelLight : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-light alert-dismissable";
    }

    /// <summary>
    /// The Dark notification level.
    /// </summary>
    public class NotificationLevelDark : INotificationLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-dark alert-dismissable";
    }
}
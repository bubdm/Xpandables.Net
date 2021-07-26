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
    /// Defines the base interface for an notification position.
    /// </summary>
    public interface INotificationPosition
    {
        /// <summary>
        /// Gets the css class for the notification.
        /// </summary>
        string PositionClass { get; }
    }

    /// <summary>
    ///  Provides with the notification position.
    /// </summary>
    public readonly struct NotificationPosition
    {
        /// <summary>
        /// Gets the <see cref="NotificationPositionTopRight"/>.
        /// </summary>
        public static INotificationPosition TopRight => new NotificationPositionTopRight();

        /// <summary>
        /// Gets the <see cref="NotificationPositionTopRight"/>.
        /// </summary>
        public static INotificationPosition TopLeft => new NotificationPositionTopLeft();

        /// <summary>
        /// Gets the <see cref="NotificationPositionTopRight"/>.
        /// </summary>
        public static INotificationPosition TopCenter => new NotificationPositionTopCenter();

        /// <summary>
        /// Gets the <see cref="NotificationPositionTopRight"/>.
        /// </summary>
        public static INotificationPosition BottomRight => new NotificationPositionBottomRight();

        /// <summary>
        /// Gets the <see cref="NotificationPositionTopRight"/>.
        /// </summary>
        public static INotificationPosition BottomLeft => new NotificationPositionBottomLeft();

        /// <summary>
        /// Gets the <see cref="NotificationPositionTopRight"/>.
        /// </summary>
        public static INotificationPosition BottomCenter => new NotificationPositionBottomCenter();
    }

    /// <summary>
    /// The top right position.
    /// </summary>
    public struct NotificationPositionTopRight : INotificationPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-topright";
    }

    /// <summary>
    /// The top left position.
    /// </summary>
    public struct NotificationPositionTopLeft : INotificationPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-topleft";
    }

    /// <summary>
    /// The top center position.
    /// </summary>
    public struct NotificationPositionTopCenter : INotificationPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-topcenter";
    }

    /// <summary>
    /// The bottom right position.
    /// </summary>
    public struct NotificationPositionBottomRight : INotificationPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-bottomright";
    }

    /// <summary>
    /// The bottom left position.
    /// </summary>
    public struct NotificationPositionBottomLeft : INotificationPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-bottomleft";
    }

    /// <summary>
    /// The bottom center position.
    /// </summary>
    public struct NotificationPositionBottomCenter : INotificationPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-bottomcenter";
    }
}

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

namespace Xpandables.Net.Alerts
{
    /// <summary>
    /// Defines the base interface for an alert position.
    /// </summary>
    public interface IAlertPosition
    {
        /// <summary>
        /// Gets the css class for the alert.
        /// </summary>
        string PositionClass { get; }
    }

    /// <summary>
    ///  Provides with the alert position.
    /// </summary>
    public readonly struct AlertPosition
    {
        /// <summary>
        /// Gets the <see cref="AlertPositionTopRight"/>.
        /// </summary>
        public static IAlertPosition TopRight => new AlertPositionTopRight();

        /// <summary>
        /// Gets the <see cref="AlertPositionTopRight"/>.
        /// </summary>
        public static IAlertPosition TopLeft => new AlertPositionTopLeft();

        /// <summary>
        /// Gets the <see cref="AlertPositionTopRight"/>.
        /// </summary>
        public static IAlertPosition TopCenter => new AlertPositionTopCenter();

        /// <summary>
        /// Gets the <see cref="AlertPositionTopRight"/>.
        /// </summary>
        public static IAlertPosition BottomRight => new AlertPositionBottomRight();

        /// <summary>
        /// Gets the <see cref="AlertPositionTopRight"/>.
        /// </summary>
        public static IAlertPosition BottomLeft => new AlertPositionBottomLeft();

        /// <summary>
        /// Gets the <see cref="AlertPositionTopRight"/>.
        /// </summary>
        public static IAlertPosition BottomCenter => new AlertPositionBottomCenter();
    }

    /// <summary>
    /// The top right position.
    /// </summary>
    public struct AlertPositionTopRight : IAlertPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-topright";
    }

    /// <summary>
    /// The top left position.
    /// </summary>
    public struct AlertPositionTopLeft : IAlertPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-topleft";
    }

    /// <summary>
    /// The top center position.
    /// </summary>
    public struct AlertPositionTopCenter : IAlertPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-topcenter";
    }

    /// <summary>
    /// The bottom right position.
    /// </summary>
    public struct AlertPositionBottomRight : IAlertPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-bottomright";
    }

    /// <summary>
    /// The bottom left position.
    /// </summary>
    public struct AlertPositionBottomLeft : IAlertPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-bottomleft";
    }

    /// <summary>
    /// The bottom center position.
    /// </summary>
    public struct AlertPositionBottomCenter : IAlertPosition
    {
        ///<inheritdoc/>
        public string PositionClass => "position-bottomcenter";
    }
}

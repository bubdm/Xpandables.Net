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
    /// Defines the base interface for an alert level.
    /// </summary>
    public interface IAlertLevel
    {
        /// <summary>
        /// Gets the css class fo the alert.
        /// </summary>
        string LevelClass { get; }
    }

    /// <summary>
    /// Provides with the alert level.
    /// </summary>
    public readonly struct AlertLevel
    {
        /// <summary>
        /// Gets the <see cref="AlertLevelInformation"/>.
        /// </summary>
        public static IAlertLevel Info => new AlertLevelInformation();

        /// <summary>
        /// Gets the <see cref="AlertLevelDark"/>.
        /// </summary>
        public static IAlertLevel Dark => new AlertLevelDark();

        /// <summary>
        /// Gets the <see cref="AlertLevelLight"/>.
        /// </summary>
        public static IAlertLevel Light => new AlertLevelLight();

        /// <summary>
        /// Gets the <see cref="AlertLevelError"/>.
        /// </summary>
        public static IAlertLevel Error => new AlertLevelError();

        /// <summary>
        /// Gets the <see cref="AlertLevelPrimay"/>.
        /// </summary>
        public static IAlertLevel Primary => new AlertLevelPrimay();

        /// <summary>
        /// Gets the <see cref="AlertLevelSecondary"/>.
        /// </summary>
        public static IAlertLevel Secondary => new AlertLevelSecondary();

        /// <summary>
        /// Gets the <see cref="AlertLevelSuccess"/>.
        /// </summary>
        public static IAlertLevel Success => new AlertLevelSuccess();

        /// <summary>
        /// Gets the <see cref="AlertLevelWarning"/>.
        /// </summary>
        public static IAlertLevel Warning => new AlertLevelWarning();
    }

    /// <summary>
    /// The Information alert level.
    /// </summary>
    public class AlertLevelInformation : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-info alert-dismissable";
    }

    /// <summary>
    /// The Error alert level.
    /// </summary>
    public class AlertLevelError : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-danger alert-dismissable";
    }

    /// <summary>
    /// The Warning alert level.
    /// </summary>
    public class AlertLevelWarning : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-warning alert-dismissable";
    }

    /// <summary>
    /// The Success alert level.
    /// </summary>
    public class AlertLevelSuccess : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-success alert-dismissable";
    }

    /// <summary>
    /// The Secondary alert level.
    /// </summary>
    public class AlertLevelSecondary : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-secondary alert-dismissable";
    }

    /// <summary>
    /// The Primay alert level.
    /// </summary>
    public class AlertLevelPrimay : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-primary alert-dismissable";
    }

    /// <summary>
    /// The light alert level.
    /// </summary>
    public class AlertLevelLight : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-light alert-dismissable";
    }

    /// <summary>
    /// The Dark alert level.
    /// </summary>
    public class AlertLevelDark : IAlertLevel
    {
        ///<inheritdoc/>
        public string LevelClass => "alert alert-dark alert-dismissable";
    }
}
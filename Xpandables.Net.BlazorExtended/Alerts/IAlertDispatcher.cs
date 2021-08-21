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

namespace Xpandables.Net.Alerts;

/// <summary>
/// Provides the base alert interface for client displaying alerts.
/// </summary>
public interface IAlertDispatcher
{
    /// <summary>
    /// Defines the default alert identifier.
    /// </summary>
    public const string DefaultId = "default-alert";

    /// <summary>
    /// Gets or sets the current component idenfifier.
    /// </summary>
    string ComponentId { get; set; }

    /// <summary>
    /// Defines the event raised when receive an alert.
    /// </summary>
    event Action<Alert>? OnAlert;

    /// <summary>
    /// Raizes a new alert using the provided argument.
    /// </summary>
    /// <param name="title">The title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="level">The level of the alert.</param>
    /// <param name="icon">The icon of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="title"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
    void Notify(
        string title,
        string message,
        IAlertLevel level,
        IAlertIcon icon,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true);

    /// <summary>
    /// Raised the specified alert.
    /// </summary>
    /// <param name="alert">The alert to be raised.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="alert"/> is null.</exception>
    void RaizeAlert(Alert alert);

    /// <summary>
    /// Clears the alert matching the specified identifier.
    /// </summary>
    /// <param name="id">The target alert identifier.</param>
    void Clear(string id = DefaultId);
}

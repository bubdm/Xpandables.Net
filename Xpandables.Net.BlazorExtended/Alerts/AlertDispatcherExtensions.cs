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
/// Provides with extension methods for <see cref="IAlertDispatcher"/>.
/// </summary>
public static class AlertDispatcherExtensions
{
    /// <summary>
    /// Raizes a success alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Success(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Success, AlertIcon.Success, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes a warning alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Warning(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Warning, AlertIcon.Warning, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes an error alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Error(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Error, AlertIcon.Error, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes an info alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Info(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Info, AlertIcon.Info, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes a dark alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Dark(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Dark, AlertIcon.Dark, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes a light alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Light(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Light, AlertIcon.Light, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes a primary alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Primary(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Primary, AlertIcon.Primary, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);

    /// <summary>
    /// Raizes a secondary alert with specified information.
    /// </summary>
    /// <param name="dispatcher">The taergt dispatcher.</param>
    /// <param name="title">the title of the alert.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="header">The header message of the alert.</param>
    /// <param name="helper">The helper message of the alert.</param>
    /// <param name="isAutoClose">is alert auto closed.</param>
    /// <param name="isKeepAfterRouteChange">is alert kept after route change.</param>
    /// <param name="isFade">is alert fade.</param>
    public static void Secondary(
        this IAlertDispatcher dispatcher,
        string title,
        string message,
        string? header = default,
        string? helper = default,
        bool isAutoClose = true,
        bool isKeepAfterRouteChange = false,
        bool isFade = true)
        => dispatcher.Notify(title, message, AlertLevel.Secondary, AlertIcon.Secondary, header, helper, isAutoClose, isKeepAfterRouteChange, isFade);
}

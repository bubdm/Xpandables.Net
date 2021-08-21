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
/// Defines the base interface for an alert icon.
/// </summary>
public interface IAlertIcon
{
    /// <summary>
    /// Gets the css class for an alert icon.
    /// </summary>
    string IconClass { get; }
}

/// <summary>
/// Provides with the alert icon.
/// </summary>
public readonly struct AlertIcon
{
    /// <summary>
    /// Gets the <see cref="AlertIconDark"/>.
    /// </summary>
    public static IAlertIcon Dark => new AlertIconDark();

    /// <summary>
    /// Gets the <see cref="AlertIconError"/>.
    /// </summary>
    public static IAlertIcon Error => new AlertIconError();

    /// <summary>
    /// Gets the <see cref="AlertIconInformation"/>.
    /// </summary>
    public static IAlertIcon Info => new AlertIconInformation();

    /// <summary>
    /// Gets the <see cref="AlertIconLight"/>.
    /// </summary>
    public static IAlertIcon Light => new AlertIconLight();

    /// <summary>
    /// Gets the <see cref="AlertIconPrimary"/>.
    /// </summary>
    public static IAlertIcon Primary => new AlertIconPrimary();

    /// <summary>
    /// Gets the <see cref="AlertIconSecondary"/>.
    /// </summary>
    public static IAlertIcon Secondary => new AlertIconSecondary();

    /// <summary>
    /// Gets the <see cref="AlertIconSuccess"/>.
    /// </summary>
    public static IAlertIcon Success => new AlertIconSuccess();

    /// <summary>
    /// Gets the <see cref="AlertIconWarning"/>.
    /// </summary>
    public static IAlertIcon Warning => new AlertIconWarning();
}

/// <summary>
/// The Warning alert icon.
/// </summary>
public class AlertIconWarning : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-exclamation-circle";
}

/// <summary>
/// The Error alert icon.
/// </summary>
public class AlertIconError : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-meh";
}

/// <summary>
/// The Information alert icon.
/// </summary>
public class AlertIconInformation : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-info-circle";
}

/// <summary>
/// The Primary alert icon.
/// </summary>
public class AlertIconPrimary : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-info-circle";
}

/// <summary>
/// The Secondary alert icon.
/// </summary>
public class AlertIconSecondary : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-info-circle";
}

/// <summary>
/// The Success alert icon.
/// </summary>
public class AlertIconSuccess : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-check-circle";
}

/// <summary>
/// The Dark alert icon.
/// </summary>
public class AlertIconDark : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-moon";
}

/// <summary>
/// The Light alert icon.
/// </summary>
public class AlertIconLight : IAlertIcon
{
    ///<inheritdoc/>
    public string IconClass => "fas fa-sun";
}

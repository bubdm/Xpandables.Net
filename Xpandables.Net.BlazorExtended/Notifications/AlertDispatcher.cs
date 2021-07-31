
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

namespace Xpandables.Net.Alerts
{
    /// <summary>
    /// The default implementation of <see cref="IAlertDispatcher"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class AlertDispatcher : IAlertDispatcher
    {
        ///<inheritdoc/>
        public event Action<Alert>? OnAlert;

        ///<inheritdoc/>
        public string ComponentId { get; set; } = default!;

        ///<inheritdoc/>
        public void Notify(
                  string title,
                  string message,
                  IAlertLevel level,
                  IAlertIcon icon,
                  string? header = default,
                  string? helper = default,
                  bool isAutoClose = true,
                  bool isKeepAfterRouteChange = false,
                  bool isFade = true)
        {
            _ = title ?? throw new ArgumentNullException(nameof(title));
            _ = message ?? throw new ArgumentNullException(nameof(message));

            var alert = Alert.With(title, message)
                .WithIcon(icon)
                .WithLevel(level)
                .AutoClose(isAutoClose)
                .KeepAfterRouteChange(isKeepAfterRouteChange)
                .Fade(isFade);

            if (header is not null)
                alert = alert.WithHeader(header);

            if (helper is not null)
                alert = alert.WithHelper(helper);

            RaizeAlert(alert);
        }

        ///<inheritdoc/>
        public virtual void Clear(string id = IAlertDispatcher.DefaultId) => OnAlert?.Invoke(Alert.None(id));

        ///<inheritdoc/>
        public virtual void RaizeAlert(Alert alert)
        {
            if (alert.Id == IAlertDispatcher.DefaultId)
                alert.Id = ComponentId;

            OnAlert?.Invoke(alert);
        }
    }
}

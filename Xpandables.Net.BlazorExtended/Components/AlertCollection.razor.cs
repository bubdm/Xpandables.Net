
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
using Microsoft.AspNetCore.Components.Routing;

using System;
using System.Threading.Tasks;

using Xpandables.Net.Alerts;

namespace Xpandables.Net.Components
{
    /// <summary>
    /// The collection of alert components.
    /// </summary>
    public partial class AlertCollection : IDisposable
    {
        [Inject]
        private IAlertProvider AlertProvider { get; set; } = default!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        ///<inheritdoc/>
        public void Dispose()
        {
            // unsubscribe from alert and location change events
            AlertProvider.OnAlert -= OnAlert;
            NavigationManager.LocationChanged -= OnLocationChange;

            GC.SuppressFinalize(this);
        }

        ///<inheritdoc/>
        protected override void OnInitialized()
        {
            // subscribe to new notifications and location change events
            AlertProvider.OnAlert += OnAlert;
            NavigationManager.LocationChanged += OnLocationChange;
        }

        /// <summary>
        /// Displays the specified alert.
        /// </summary>
        /// <param name="alert">The alert to act on.</param>
        protected virtual async void OnAlert(Alert alert)
        {
            // clear notifications when an empty alert is received
            if (alert.Message == null)
            {
                // remove alert without the 'KeepAfterRouteChange' flag set to true
                AlertProvider.Alerts.RemoveAll(x => !x.KeepAfterRouteChange);

                // set the 'KeepAfterRouteChange' flag to false for the 
                // remaining alert so they are removed on the next clear
                AlertProvider.Alerts.ForEach(x => x = x with { KeepAfterRouteChange = false });
            }
            else
            {
                // add alert to array
                AlertProvider.Alerts.Add(alert);

                await InvokeAsync(StateHasChanged);

                // auto close alert if required
                if (alert.AutoClose)
                {
                    await Task.Delay(AlertProvider.AlertOptions.Delay);
                    RemoveNotificationAsync(alert);
                }
            }

            await InvokeAsync(StateHasChanged);
        }


        /// <summary>
        /// Clears the visible alert on navigation change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnLocationChange(object? sender, LocationChangedEventArgs e)
        {
            foreach (var alert in AlertProvider.Alerts)
                AlertProvider.Clear(alert.Id);
        }

        /// <summary>
        /// Removes the specified alert.
        /// </summary>
        /// <param name="alert">The alert to be removed.</param>
        public virtual async void RemoveNotificationAsync(Alert alert)
        {
            // check if already removed to prevent error on auto close
            if (!AlertProvider.Alerts.Contains(alert)) return;

            if (AlertProvider.AlertOptions.FadeOut)
            {
                // fade out alert
                alert = alert.FadeOut();

                // remove alert after faded out
                await Task.Delay(AlertProvider.AlertOptions.FadeOutDelay);
                AlertProvider.Alerts.Remove(alert);
            }
            else
            {
                // remove alert
                AlertProvider.Alerts.Remove(alert);
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}

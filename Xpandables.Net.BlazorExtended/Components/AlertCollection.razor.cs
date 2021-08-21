
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

using System.ComponentModel;

using Xpandables.Net.Alerts;

namespace Xpandables.Net.Components;

/// <summary>
/// The collection of alert components.
/// </summary>
public partial class AlertCollection : IDisposable
{
    [Inject]
    private IAlertDispatcher AlertDispatcher { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the alerts collection parameters.
    /// </summary>
    [Parameter]
    public AlertOptions Options { get; set; } = new();

    /// <summary>
    /// Gets or sets the unique collection identifier.
    /// The default value is <see cref="Guid.NewGuid"/>.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets the collection of registered alerts.
    /// </summary>
    protected BindingList<Alert> AlertList { get; } = new BindingList<Alert>
    {
        AllowEdit = true,
        AllowNew = true,
        AllowRemove = true
    };

    ///<inheritdoc/>
    public void Dispose()
    {
        // unsubscribe from alert and location change events

        AlertDispatcher.OnAlert -= OnAlert;
        NavigationManager.LocationChanged -= OnLocationChange;

        AlertList.ListChanged -= Notifications_ListChanged;
        AlertList.Clear();

        GC.SuppressFinalize(this);
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        // subscribe to new alerts and location change events
        AlertDispatcher.OnAlert += OnAlert;
        NavigationManager.LocationChanged += OnLocationChange;

        AlertList.ListChanged += Notifications_ListChanged;
        AlertDispatcher.ComponentId = Id;
    }

    private void Notifications_ListChanged(object? sender, ListChangedEventArgs e)
    {
        if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor is not null && e.PropertyDescriptor.Name != nameof(Alert.IsKeepAfterRouteChange))
            InvokeAsync(StateHasChanged);

        if (e.ListChangedType == ListChangedType.ItemDeleted)
            InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Displays the specified alert.
    /// </summary>
    /// <param name="alert">The alert to act on.</param>
    protected virtual async void OnAlert(Alert alert)
    {
        // ignoe alert sent to other components
        if (alert.Id != Id)
            return;

        // clear alerts when an empty alert is received
        if (alert.Message == "NONE")
        {
            // remove alert without the 'KeepAfterRouteChange' flag set to true
            var alertsToBeRemoved = AlertList.Where(x => !x.IsKeepAfterRouteChange).ToList();
            foreach (var item in alertsToBeRemoved)
                AlertList.Remove(item);

            // set the 'KeepAfterRouteChange' flag to false for the 
            // remaining alert so they are removed on the next clear
            foreach (var item in AlertList)
                item.KeepAfterRouteChange(false);
        }
        else
        {
            // add alert to array
            AlertList.Add(alert);

            await InvokeAsync(StateHasChanged);

            // auto close alert if required
            if (alert.IsAutoClose)
            {
                await Task.Delay(Options.Delay);
                RemoveAlertAsync(alert);
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
        foreach (var alert in AlertList)
            AlertDispatcher.Clear(alert.Id);
    }

    /// <summary>
    /// Removes the specified alert.
    /// </summary>
    /// <param name="alert">The alert to be removed.</param>
    public virtual async void RemoveAlertAsync(Alert alert)
    {
        // check if already removed to prevent error on auto close
        if (!AlertList.Contains(alert)) return;

        if (Options.FadeOut)
        {
            // fade out alert
            alert.Fade();

            // remove alert after faded out
            await Task.Delay(Options.FadeOutDelay);
            AlertList.Remove(alert);
        }
        else
        {
            // remove alert
            AlertList.Remove(alert);
        }

        await InvokeAsync(StateHasChanged);
    }
}

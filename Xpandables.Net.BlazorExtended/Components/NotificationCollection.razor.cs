
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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Xpandables.Net.Notifications;

namespace Xpandables.Net.Components
{
    /// <summary>
    /// The collection of notification components.
    /// </summary>
    public partial class NotificationCollection : IDisposable
    {
        [Inject]
        private INotificationDispatcher NotificationDispatcher { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Gets or sets the notifcations collection parameters.
        /// </summary>
        [Parameter]
        public NotificationOptions Options { get; set; } = new();

        /// <summary>
        /// Gets or sets the unique collection identifier.
        /// The default value is <see cref="Guid.NewGuid"/>.
        /// </summary>
        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the collection of registered notifications.
        /// </summary>
        protected BindingList<Notification> Notifications { get; } = new BindingList<Notification>
        {
            AllowEdit = true,
            AllowNew = true,
            AllowRemove = true
        };

        ///<inheritdoc/>
        public void Dispose()
        {
            // unsubscribe from notification and location change events

            NotificationDispatcher.OnNotification -= OnNotification;
            NavigationManager.LocationChanged -= OnLocationChange;

            Notifications.ListChanged -= Notifications_ListChanged;
            Notifications.Clear();

            GC.SuppressFinalize(this);
        }

        ///<inheritdoc/>
        protected override void OnInitialized()
        {
            // subscribe to new notifications and location change events
            NotificationDispatcher.OnNotification += OnNotification;
            NavigationManager.LocationChanged += OnLocationChange;

            Notifications.ListChanged += Notifications_ListChanged;
            NotificationDispatcher.ComponentId = Id;
        }

        private void Notifications_ListChanged(object sender, ListChangedEventArgs e)
        {
            //if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor.Name != nameof(Notification.IsKeepAfterRouteChange))
            //    InvokeAsync(StateHasChanged);

            //if (e.ListChangedType == ListChangedType.ItemDeleted)
            //    InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Displays the specified notification.
        /// </summary>
        /// <param name="notification">The notification to act on.</param>
        protected virtual async void OnNotification(Notification notification)
        {
            // ignoe notification sent to other components
            if (notification.Id != Id)
                return;

            // clear notifications when an empty notification is received
            if (notification.Message == "NONE")
            {
                // remove notification without the 'KeepAfterRouteChange' flag set to true
                var notificationsToBeRemoved = Notifications.Where(x => !x.IsKeepAfterRouteChange).ToList();
                foreach (var item in notificationsToBeRemoved)
                    Notifications.Remove(item);

                // set the 'KeepAfterRouteChange' flag to false for the 
                // remaining notification so they are removed on the next clear
                foreach (var item in Notifications)
                    item.KeepAfterRouteChange(false);
            }
            else
            {
                // add notification to array
                Notifications.Add(notification);

                await InvokeAsync(StateHasChanged);

                // auto close notification if required
                if (notification.IsAutoClose)
                {
                    await Task.Delay(Options.Delay);
                    RemoveNotificationAsync(notification);
                }
            }

            await InvokeAsync(StateHasChanged);
        }


        /// <summary>
        /// Clears the visible notification on navigation change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnLocationChange(object? sender, LocationChangedEventArgs e)
        {
            foreach (var notification in Notifications)
                NotificationDispatcher.Clear(notification.Id);
        }

        /// <summary>
        /// Removes the specified notification.
        /// </summary>
        /// <param name="notification">The notification to be removed.</param>
        public virtual async void RemoveNotificationAsync(Notification notification)
        {
            // check if already removed to prevent error on auto close
            if (!Notifications.Contains(notification)) return;

            if (Options.FadeOut)
            {
                // fade out notification
                notification.Fade();

                // remove notification after faded out
                await Task.Delay(Options.FadeOutDelay);
                Notifications.Remove(notification);
            }
            else
            {
                // remove notification
                Notifications.Remove(notification);
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}

using System;
using System.Collections.Generic;

namespace Servitia.Net.Notifications
{
    /// <summary>
    /// Provides the base authentication interface for Web client displaying notification.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Defines the default message identifier.
        /// </summary>
        public const string DefaultId = "default-notification";

        /// <summary>
        /// Defines the event raised when receive a notification.
        /// </summary>
        event Action<Notification> OnNotification;

        /// <summary>
        /// Gets the collection of registered notifications.
        /// </summary>
        List<Notification> Notifications { get; }

        /// <summary>
        /// Raised a <see cref="NotificationLevel.Success"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Success(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="NotificationLevel.Error"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Error(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="NotificationLevel.Info"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Information(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised a <see cref="NotificationLevel.Warning"/> notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        void Warning(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false);

        /// <summary>
        /// Raised the specified notification.
        /// </summary>
        /// <param name="notification">The notification to be raised.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notification"/> is null.</exception>
        void Notify(Notification notification);

        /// <summary>
        /// Clears the notification matching the specified identifier.
        /// </summary>
        /// <param name="id">The target notification identifier.</param>
        void Clear(string id = DefaultId);
    }
}


using System;
using System.Collections.Generic;

namespace Servitia.Net.Notifications
{
    /// <summary>
    /// The default implementation of <see cref="INotificationService"/>.
    /// You can derive from this class to customize its behaviors.
    /// </summary>
    public class NotificationService : INotificationService
    {
        ///<inheritdoc/>
        public event Action<Notification> OnNotification;

        ///<inheritdoc/>
        public List<Notification> Notifications { get; } = new List<Notification>();

        ///<inheritdoc/>
        public virtual void Clear(string id = "default-notification") => OnNotification(new Notification { Id = id });

        ///<inheritdoc/>
        public virtual void Error(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
                title,
                header,
                message,
                NotificationLevel.Error,
                NotificationIcon.Error,
                autoClose,
                keepAfterRouteChange);

            Notify(notification);
        }

        ///<inheritdoc/>
        public virtual void Information(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
                title,
                header,
                message,
                NotificationLevel.Info,
                NotificationIcon.Info,
                autoClose,
                keepAfterRouteChange);

            Notify(notification);
        }

        ///<inheritdoc/>
        public virtual void Notify(Notification notification)
        {
            _ = notification ?? throw new ArgumentNullException(nameof(notification));

            notification.Id ??= INotificationService.DefaultId;
            OnNotification?.Invoke(notification);
        }

        ///<inheritdoc/>
        public virtual void Success(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
            title,
            header,
            message,
            NotificationLevel.Success,
            NotificationIcon.Success,
            autoClose,
            keepAfterRouteChange);

            Notify(notification);
        }

        ///<inheritdoc/>
        public virtual void Warning(string title, string header, string message, bool autoClose = true, bool keepAfterRouteChange = false)
        {
            var notification = CreateNotification(
              title,
              header,
              message,
              NotificationLevel.Warning,
              NotificationIcon.Warning,
              autoClose,
              keepAfterRouteChange);

            Notify(notification);
        }

        /// <summary>
        /// Creates a notification with arguments.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="header">The notification header.</param>
        /// <param name="message">The message of the notification.</param>
        /// <param name="level">The notification level.</param>
        /// <param name="icon">The notification icon.</param>
        /// <param name="autoClose">if the notification auto-closes itself.</param>
        /// <param name="keepAfterRouteChange">If the notification keeps after route change.</param>
        /// <returns>A new instance of <see cref="Notification"/>.</returns>
        protected static Notification CreateNotification(
            string title,
            string header,
            string message,
            NotificationLevel level,
            NotificationIcon icon,
            bool autoClose = true,
            bool keepAfterRouteChange = false)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Header = header,
                Level = level,
                Icon = icon,
                Message = message,
                AutoClose = autoClose,
                KeepAfterRouteChange = keepAfterRouteChange
            };

            return notification;

        }
    }
}


using System;

namespace Servitia.Net.Notifications
{
    /// <summary>
    /// Determines the notification level.
    /// </summary>
    public enum NotificationLevel
    {
        /// <summary>
        /// Success level.
        /// </summary>
        Success,

        /// <summary>
        /// Error level.
        /// </summary>
        Error,

        /// <summary>
        /// Information level.
        /// </summary>
        Info,

        /// <summary>
        /// Warning level.
        /// </summary>
        Warning
    }

    /// <summary>
    /// Determines the notification position.
    /// </summary>
    public enum NotificationPosition
    {
        /// <summary>
        /// Top Left.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Top right.
        /// </summary>
        TopRight,

        /// <summary>
        /// Top center.
        /// </summary>
        TopCenter,

        /// <summary>
        /// Bottom Left.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Bottom right.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Bottom center.
        /// </summary>
        BottomCenter
    }

    /// <summary>
    /// Determines the notification icon.
    /// </summary>
    public enum NotificationIcon
    {
        /// <summary>
        /// Success level.
        /// </summary>
        Success,

        /// <summary>
        /// Error level.
        /// </summary>
        Error,

        /// <summary>
        /// Information level.
        /// </summary>
        Info,

        /// <summary>
        /// Warning level.
        /// </summary>
        Warning
    }

    /// <summary>
    /// Contains a <see cref="Notification"/> to be display.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="Notification"/> class.
        /// </summary>
        public Notification() { }

        /// <summary>
        /// Gets or sets the unique identifier of the notification.
        /// </summary>
        public string Id { get; internal set; } = INotificationService.DefaultId;

        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the notification header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the created date, used for ordering notification.
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the notification level.
        /// </summary>
        public NotificationLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the notification icon.
        /// </summary>
        public NotificationIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets the message content of th notification.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Determines whether the notification auto-close itself.
        /// The default value is true.
        /// </summary>
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// Determines whether the notification keep after route change.
        /// The default value is false.
        /// </summary>
        public bool KeepAfterRouteChange { get; set; } = false;

        /// <summary>
        /// Determines whether the notification fade out or not.
        /// The default value is false.
        /// </summary>
        public bool Fade { get; private set; } = false;

        /// <summary>
        /// Activate the Fade out of the notification.
        /// </summary>
        public void FadeOut() => Fade = true;
    }
}


<div class="@GetCssClass() notification-item">
    <div>
        <button type="button" class="close"
                @onclick="@(() => RemoveNotification())">&times;</button>
        <h4 class="alert-heading"><i class="@GetIconClass()"></i> @Notification.Title</h4>
    </div>
    <p>@Notification.Header</p>
    <hr />
    <p>@Notification.Message</p>
    <p>If you need any help just place the mouse pointer above info icon next to the form field.</p>
</div>

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Servitia.Net.Notifications;

namespace Servitia.Net.RazorComponents.Components
{
    /// <summary>
    /// The notification component.
    /// </summary>
    public partial class NotificationComponent
    {
        [CascadingParameter] private NotificationComponents NotificationComponents { get; set; }

        /// <summary>
        /// Gets or sets the current notification.
        /// </summary>
        [Parameter] public Notification Notification { get; set; }

        /// <summary>
        /// Gets the collection of notification classes matching the level.
        /// </summary>
        protected virtual IDictionary<NotificationLevel, string> NotificationLevelClasses
            => new Dictionary<NotificationLevel, string>
            {
                [NotificationLevel.Success] = "alert-success",
                [NotificationLevel.Error] = "alert-danger",
                [NotificationLevel.Info] = "alert-info",
                [NotificationLevel.Warning] = "alert-warning"
            };

        /// <summary>
        /// Gets the collection of notification icon classes matching the level.
        /// </summary>
        protected virtual IDictionary<NotificationIcon, string> NotificationIconClasses
            => new Dictionary<NotificationIcon, string>
            {
                [NotificationIcon.Success] = "fa fa-check-circle",
                [NotificationIcon.Error] = "fa fa-meh-o",
                [NotificationIcon.Info] = "fa fa-info-circle",
                [NotificationIcon.Warning] = "fa fa-exclamation-circle"
            };

        /// <summary>
        /// Remove the notification.
        /// </summary>
        protected void RemoveNotification()
            => NotificationComponents.RemoveNotificationAsync(Notification);

        /// <summary>
        /// Returns the CssClass for the class notification.
        /// </summary>
        /// <returns>A string representing the CssClass.</returns>
        protected virtual string GetCssClass()
        {
            if (Notification is null) return null;

            var classes = new List<string> { "alert", "alert-dismissable"};

            classes.Add(NotificationLevelClasses[Notification.Level]);

            if (Notification.Fade)
                classes.Add("fade");

            return string.Join(' ', classes);
        }

        /// <summary>
        /// Returns the CssClass for the icon notification.
        /// </summary>
        /// <returns>A string representing the CssClass.</returns>
        protected virtual string GetIconClass()
        {
            if (Notification is null) return null;
            return NotificationIconClasses[Notification.Icon].ToString();
        }
    }
}

@if (NotificationService.Notifications.Any())
{
    <div class="notification-list @PositionClass">
        <CascadingValue Value="this">
            @foreach (var notification in NotificationService.Notifications.OrderBy(o => o.CreatedOn))
            {
                <NotificationComponent Notification="notification" />
            }
        </CascadingValue>
    </div>
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Servitia.Net.Notifications;

namespace Servitia.Net.RazorComponents.Components
{
    /// <summary>
    /// The collection of notification components.
    /// </summary>
    public partial class NotificationComponents : IDisposable
    {
        [Inject] private INotificationService NotificationService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the notification position. The default value is <see cref="NotificationPosition.TopRight"/>.
        /// </summary>
        [Parameter] public NotificationPosition Position { get; set; } = NotificationPosition.TopRight;

        /// <summary>
        /// Gets or sets the notification delay. The default value is 5000sec.
        /// </summary>
        [Parameter] public int Delay { get; set; } = 5000;

        /// <summary>
        /// Determines whether Fade is out or not.
        /// The default value is true.
        /// </summary>
        [Parameter] public bool Fade { get; set; } = true;

        /// <summary>
        /// Gets or sets the notification delay after fade out. The default value is 250sec.
        /// </summary>
        [Parameter] public int DelayFadeOut { get; set; } = 250;

        private string PositionClass => $"position-{Position.ToString().ToLower()}";

        //internal readonly List<Notification> Notifications = new List<Notification>();

        ///<inheritdoc/>
        public void Dispose()
        {
            // unsubscribe from notification and location change events
            NotificationService.OnNotification -= OnNotification;
            NavigationManager.LocationChanged -= OnLocationChange;

            GC.SuppressFinalize(this);
        }

        ///<inheritdoc/>
        protected override void OnInitialized()
        {
            // subscribe to new notifications and location change events
            NotificationService.OnNotification += OnNotification;
            NavigationManager.LocationChanged += OnLocationChange;
        }

        /// <summary>
        /// Displays the specified notification.
        /// </summary>
        /// <param name="notification">The notification to act on.</param>
        protected virtual async void OnNotification(Notification notification)
        {
            // clear notifications when an empty notification is received
            if (notification.Message == null)
            {
                // remove notification without the 'KeepAfterRouteChange' flag set to true
                NotificationService.Notifications.RemoveAll(x => !x.KeepAfterRouteChange);

                // set the 'KeepAfterRouteChange' flag to false for the 
                // remaining notification so they are removed on the next clear
                NotificationService.Notifications.ForEach(x => x.KeepAfterRouteChange = false);
            }
            else
            {
                // add notification to array
                NotificationService.Notifications.Add(notification);

                await InvokeAsync(StateHasChanged);

                // auto close notification if required
                if (notification.AutoClose)
                {
                    await Task.Delay(Delay);
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
        protected virtual void OnLocationChange(object sender, LocationChangedEventArgs e)
        {
            foreach (var notification in NotificationService.Notifications)
                NotificationService.Clear(notification.Id);
        }

        /// <summary>
        /// Removes the specified notification.
        /// </summary>
        /// <param name="notification">The notification to be removed.</param>
        public virtual async void RemoveNotificationAsync(Notification notification)
        {
            // check if already removed to prevent error on auto close
            if (!NotificationService.Notifications.Contains(notification)) return;

            if (Fade)
            {
                // fade out notification
                notification.FadeOut();

                // remove notification after faded out
                await Task.Delay(DelayFadeOut);
                NotificationService.Notifications.Remove(notification);
            }
            else
            {
                // remove notification
                NotificationService.Notifications.Remove(notification);
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}

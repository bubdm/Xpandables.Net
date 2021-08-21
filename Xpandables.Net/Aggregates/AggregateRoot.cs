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

using System.Diagnostics;
using System.Reflection;

using Xpandables.Net.Aggregates.Events;

namespace Xpandables.Net.Aggregates;

/// <summary>
/// Represents a helper class that allows implementation of <see cref="IAggregateRoot{TAggregateId}"/>.
/// It contains a collection of <see cref="IDomainEvent"/>, <see cref="INotification"/>,
/// and a dictionary of domain event handlers (each handler name must start with "On" to be automatically registered). You can register event handlers using 
/// the <see cref="RegisterEventHandler{TDomainEvent}(Delegate)"/>
/// method when overriding the <see cref="RegisterEventHandlers"/> method. 
/// You can add a notification using the <see cref="AddNotification(INotification)"/>
/// method and you may use the <see cref="RaiseEvent{TDomainEvent}(TDomainEvent)"/> method to raise the specified event.
/// When creating an event (<see cref="IDomainEvent"/>), you may use of <see cref="GetNewVersion()"/> 
/// function to get the new version number according to the event creation.
/// </summary>
/// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
[Serializable]
[DebuggerDisplay("Guid = {" + nameof(AggregateId) + "} Version = {" + nameof(Version) + "}")]
public abstract class AggregateRoot<TAggregateId> : IAggregateRoot<TAggregateId>, IDomainEventSourcing, INotificationSourcing
    where TAggregateId : notnull, AggregateId
{
    private static readonly IInstanceCreator _instanceCreator = new InstanceCreator();

    private readonly ICollection<IDomainEvent> _events = new LinkedList<IDomainEvent>();
    private readonly ICollection<INotification> _notifications = new LinkedList<INotification>();
    private readonly IDictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();

    /// <summary>
    /// Gets the current version of the instance, the default value is -1.
    /// </summary>
    public AggregateVersion Version { get; protected set; } = -1;

    ///<inheritdoc/>
    public TAggregateId AggregateId { get; protected set; } = (TAggregateId)_instanceCreator.Create(typeof(TAggregateId), Guid.Empty)!;

    /// <summary>
    /// Constructs the default instance of an aggregate root.
    /// </summary>
    protected AggregateRoot()
    {
        RegisterEventHandlers();
    }

    void INotificationSourcing.MarkNotificationsAsCommitted() => _notifications.Clear();

    IOrderedEnumerable<INotification> INotificationSourcing.GetNotifications()
        => _notifications.OrderBy(o => o.OccurredOn);

    void IDomainEventSourcing.MarkEventsAsCommitted() => _events.Clear();

    IOrderedEnumerable<IDomainEvent> IDomainEventSourcing.GetUncommittedEvents()
        => _events.OrderBy(o => o.Version);

    void IDomainEventSourcing.LoadFromHistory(IOrderedEnumerable<IDomainEvent> events)
    {
        _ = events ?? throw new ArgumentNullException(nameof(events));

        foreach (var @event in events)
        {
            ((IDomainEventSourcing)this).Mutate(@event);
            Version = @event.Version;
        }
    }

    void IDomainEventSourcing.LoadFromHistory(IDomainEvent @event)
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));

        ((IDomainEventSourcing)this).Mutate(@event);
        Version = @event.Version;
    }

    void IDomainEventSourcing.Apply(IDomainEvent @event)
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));

        if (!_events.Any(e => Equals(e.Guid, @event.Guid)))
            ((IDomainEventSourcing)this).Mutate(@event);
        else
            return;

        _events.Add(@event);
    }

    void IDomainEventSourcing.Mutate(IDomainEvent @event)
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));

        if (!_eventHandlers.TryGetValue(@event.GetType(), out var eventHandler))
            throw new InvalidOperationException($"The {@event.GetType().Name} requested handler doest not exist or is not registered.");

        AggregateId = (TAggregateId)_instanceCreator.Create(typeof(TAggregateId), @event.AggregateId.Value)!;
        eventHandler.DynamicInvoke(@event);
    }

    /// <summary>
    /// Adds the specified notification to the entity collection of notifications.
    /// This notification will be published using the out-box pattern.
    /// </summary>
    /// <param name="notification">The notification to be added.</param>
    /// <exception cref=" ArgumentNullException">The <paramref name="notification"/> is null. </exception>
    /// <exception cref="InvalidOperationException">The target notification already exist in the collection.</exception>
    protected void AddNotification(INotification notification)
    {
        _ = notification ?? throw new ArgumentNullException(nameof(notification));

        if (_notifications.Any(e => e.Guid == notification.Guid))
            throw new InvalidOperationException($"The {notification.Guid} already exist in the collection");

        _notifications.Add(notification);
    }

    /// <summary>
    /// Raises the handler that matches the specified event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <param name="event">The event instance to act on.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
    protected void RaiseEvent<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : class, IDomainEvent
    {
        _ = @event ?? throw new ArgumentNullException(nameof(@event));

        ((IDomainEventSourcing)this).Apply(@event);
    }

    private readonly static MethodInfo _createDelegateMethod = typeof(GenericDelegateFactory).GetMethod("CreateDelegate")!;

    /// <summary>
    /// Registers all required event handlers for the underlying aggregate which name start with "On" and
    /// contains one parameters of <see cref="IDomainEvent"/> type.
    /// This method get called by the constructor.
    /// You can also use the <see cref="RegisterEventHandler{TDomainEvent}(Delegate)"/> method to register handler.
    /// </summary>
    protected virtual void RegisterEventHandlers()
    {
        foreach (var handler in GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.Name.StartsWith("On")
                        && m.ReturnType == typeof(void)
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType.IsAssignableTo(typeof(IDomainEvent))))
        {
            var createDelegate = _createDelegateMethod.MakeGenericMethod(handler.GetParameters()[0].ParameterType);
            var handlerDelegate = createDelegate.Invoke(null, new object[] { this, handler })!;

            RegisterEventHandler(handler.GetParameters()[0].ParameterType, (Delegate)handlerDelegate);
        }
    }

    /// <summary>
    /// Registers an handler for the <typeparamref name="TDomainEvent"/> event type.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <param name="eventHandler">the target handler to register.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="eventHandler"/> is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exist in the collection.</exception>
    protected void RegisterEventHandler<TDomainEvent>(Delegate eventHandler)
        where TDomainEvent : class, IDomainEvent
    {
        _ = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        RegisterEventHandler(typeof(TDomainEvent), eventHandler);
    }

    /// <summary>
    /// Registers an handler for the specified event type.
    /// </summary>
    /// <param name="eventType">The domain event type, must implement <see cref="IDomainEvent"/></param>
    /// <param name="eventHandler">the target handler to register.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="eventHandler"/> is null.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exist in the collection.</exception>
    /// <exception cref="ArgumentException">The <paramref name="eventType"/> does not implement <see cref="IDomainEvent"/> interface.</exception>
    protected void RegisterEventHandler(Type eventType, Delegate eventHandler)
    {
        _ = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _ = eventType ?? throw new ArgumentNullException(nameof(eventHandler));

        if (!eventType.IsAssignableTo(typeof(IDomainEvent)))
            throw new ArgumentException($"The '{eventType.GetNameWithoutGenericArity()}' must implement '{nameof(IDomainEvent)}' interface.");

        if (!_eventHandlers.TryAdd(eventType, eventHandler))
            throw new ArgumentException($"An element with the same key '{eventType.GetNameWithoutGenericArity()}' " +
                $"already exists in the collection");
    }

    /// <summary>
    /// Returns the new version of the instance when creating an <see cref="IDomainEvent"/>.
    /// </summary>
    /// <returns>A <see cref="AggregateVersion"/> value that represents the new version of the instance</returns>
    protected AggregateVersion GetNewVersion() => ++Version;
}

/// <summary>
/// Contains the generic delegate for domain event handlers.
/// </summary>
public static class GenericDelegateFactory
{
    /// <summary>
    /// Creates a delegate handler for the specified method.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="target">The target instance.</param>
    /// <param name="method">The target method.</param>
    /// <returns>A delegate like <see cref="Action{TDomainEvent}"/>.</returns>
    public static Action<TDomainEvent> CreateDelegate<TDomainEvent>(object target, MethodInfo method)
        where TDomainEvent : class, IDomainEvent
        => (Action<TDomainEvent>)Delegate.CreateDelegate(typeof(Action<TDomainEvent>), target, method);
}

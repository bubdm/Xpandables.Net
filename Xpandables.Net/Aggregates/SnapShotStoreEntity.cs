using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xpandables.Net.DomainEvents;
using Xpandables.Net.Entities;

namespace Xpandables.Net.Aggregates
{
    /// <summary>
    /// 
    /// </summary>
    public class SnapShotStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// Creates a new instance of <see cref="DomainEventStoreEntity"/> from the event of aggregate.
        /// The method will serialize the specified event to <see cref="JsonDocument"/>.
        /// </summary>
        /// <typeparam name="TAggregateId">The type of the target aggregate id.</typeparam>
        /// <typeparam name="TAggregate">The type of the target aggregate.</typeparam>
        /// <param name="event">The event to act on.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="documentOptions">Options to control the reader behavior during parsing.</param>
        /// <returns>A new instance of <see cref="DomainEventStoreEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        /// <exception cref="ArgumentException">The type of <paramref name="event"/> is not compatible with its value 
        /// or JsonDocument options contains unsupported options.</exception>
        /// <exception cref="NotSupportedException">There is no compatible JsonConverter for inputType or its serializable members.</exception>
        /// <exception cref="JsonException">The json does not represent a valid single JSON value.</exception>
        public static DomainEventStoreEntity CreateFrom<TAggregateId, TAggregate>(
            IEvent @event,
            JsonSerializerOptions? serializerOptions = default,
            JsonDocumentOptions documentOptions = default)
            where TAggregate : notnull, IAggregate<TAggregateId>
            where TAggregateId : notnull, IAggregateId
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            var aggregateId = @event.AggregateId.AsString();
            var aggregateTypeName = typeof(TAggregate).GetNameWithoutGenericArity();
            var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
            var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

            var eventString = JsonSerializer.Serialize(@event, @event.GetType(), serializerOptions);
            var eventData = JsonDocument.Parse(eventString, documentOptions);

            return new(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="aggregateTypeName"></param>
        /// <param name="eventTypeFullName"></param>
        /// <param name="eventTypeName"></param>
        /// <param name="eventData"></param>
        [JsonConstructor]
        public SnapShotStoreEntity(
            string aggregateId,
            string aggregateTypeName,
            string eventTypeFullName,
            string eventTypeName,
            JsonDocument eventData)
            : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData)
        {
        }
    }
}

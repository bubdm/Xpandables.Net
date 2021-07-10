using System.Text.Json;
using System.Text.Json.Serialization;

using Xpandables.Net.Entities;

namespace Xpandables.Net.DomainEvents
{
    /// <summary>
    /// Represents the domain event store entity.
    /// </summary>
    public class DomainEventStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="aggregateTypeName"></param>
        /// <param name="eventTypeFullName"></param>
        /// <param name="eventTypeName"></param>
        /// <param name="eventData"></param>
        [JsonConstructor]
        public DomainEventStoreEntity(
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

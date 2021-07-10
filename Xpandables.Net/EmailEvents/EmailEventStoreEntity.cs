using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xpandables.Net.Entities;

namespace Xpandables.Net.EmailEvents
{
    /// <summary>
    /// Represents the email event store entity.
    /// </summary>
    public class EmailEventStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="aggregateTypeName"></param>
        /// <param name="eventTypeFullName"></param>
        /// <param name="eventTypeName"></param>
        /// <param name="eventData"></param>
        /// <param name="state"></param>
        /// <param name="exceptionTypeFullName"></param>
        /// <param name="exception"></param>
        [JsonConstructor]
        public EmailEventStoreEntity(
            string aggregateId,
            string aggregateTypeName,
            string eventTypeFullName,
            string eventTypeName,
            JsonDocument eventData,
            string state,
            string? exceptionTypeFullName = default,
            JsonDocument? exception = default)
            : base(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            Exception = exception;
            ExceptionTypeFullName = exceptionTypeFullName;
        }

        /// <summary>
        /// Gets the state of the event emeil.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Gets the exception during sent.
        /// </summary>
        public JsonDocument? Exception { get; private set; }

        /// <summary>
        /// Gets the exception full type name.
        /// </summary>
        public string? ExceptionTypeFullName { get; private set; }

        /// <summary>
        /// Remove the underlying exception.
        /// </summary>
        public void RemoveException()
        {
            Exception = default;
            ExceptionTypeFullName = default;
        }

        /// <summary>
        /// Adds the specified exception to the event store.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="documentOptions">Options to control the reader behavior during parsing.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is null.</exception>
        public void AddException(
            Exception exception,
            JsonSerializerOptions? serializerOptions = default,
            JsonDocumentOptions documentOptions = default)
        {
            _ = exception ?? throw new ArgumentNullException(nameof(exception));

            var exceptionString = JsonSerializer.Serialize(exception, exception.GetType(), serializerOptions);
            Exception = JsonDocument.Parse(exceptionString, documentOptions);
            ExceptionTypeFullName = exception.GetType().AssemblyQualifiedName!;
        }

        /// <summary>
        /// Changes the state of the event email.
        /// </summary>
        /// <param name="state">The new state.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="state"/> is null.</exception>
        public void ChangeState(string state) => State = state ?? throw new ArgumentNullException(nameof(state));
    }
}

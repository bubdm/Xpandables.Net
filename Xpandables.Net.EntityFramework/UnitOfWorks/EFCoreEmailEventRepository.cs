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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xpandables.Net.Events;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the EFCore implementation of <see cref="IEmailEventRepository{TEmailMessage}"/>.
    /// </summary>
    /// <typeparam name="TEmailMessage">The type of the message.</typeparam>
    public class EFCoreEmailEventRepository<TEmailMessage> : EFCoreEventRepository<EmailEventStoreEntity>, IEmailEventRepository<TEmailMessage>
        where TEmailMessage : class
    {
        ///<inheritdoc/>
        public JsonSerializerOptions? SerializerOptions { get; set; } = new() { PropertyNameCaseInsensitive = true };

        ///<inheritdoc/>
        public JsonDocumentOptions DocumentOptions { get; set; } = default;

        /// <summary>
        /// Constructs a new instance of <see cref="EFCoreEmailEventRepository{TEmailMessage}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EFCoreEmailEventRepository(EFCoreContext context) : base(context) { }

        ///<inheritdoc/>
        public virtual async Task AppendEventAsync(
            IEmailEvent<TEmailMessage> @event, CancellationToken cancellationToken = default)
        {
            var aggregateId = @event.AggregateId.AsString();
            var aggregateTypeName = @event.AggregateId.GetType().GetNameWithoutGenericArity();
            var eventTypeFullName = @event.GetType().AssemblyQualifiedName!;
            var eventTypeName = @event.GetType().GetNameWithoutGenericArity();

            var eventData = GetJsonDocument(@event, SerializerOptions, DocumentOptions);
            var entity = new EmailEventStoreEntity(aggregateId, aggregateTypeName, eventTypeFullName, eventTypeName, eventData, "Created");

            await Context.Set<EmailEventStoreEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the <see cref="JsonDocument"/> from the specified document.
        /// </summary>
        /// <typeparam name="TDocument">The type of document.</typeparam>
        /// <param name="document">An instance of document to parse.</param>
        /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
        /// <param name="documentOptions">Options to control the reader behavior during parsing.</param>
        /// <returns>An instance of <see cref="JsonDocument"/>.</returns>
        protected virtual JsonDocument GetJsonDocument<TDocument>(
            TDocument document,
            JsonSerializerOptions? serializerOptions,
            JsonDocumentOptions documentOptions)
            where TDocument : notnull
        {
            var eventString = JsonSerializer.Serialize(document, document.GetType(), serializerOptions);
            return JsonDocument.Parse(eventString, documentOptions);
        }
    }
}

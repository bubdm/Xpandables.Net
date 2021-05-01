
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
using System.Text;
using System.Text.Json;

using Xpandables.Net.Events.IntegrationEvents;

namespace Xpandables.Net.Entities
{
    /// <summary>
    /// Represents an out box message to be written.
    /// </summary>
    public class IntegrationEventEntity : Entity
    {
        /// <summary>
        /// Gets the .Net Framework content type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Determines whether or not the data is JSON.
        /// </summary>
        public bool IsJson { get; }

        /// <summary>
        /// Gets the byte representation of the type.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Constructs anew instance of <see cref="IntegrationEventEntity"/> from the specified event.
        /// </summary>
        /// <param name="event">The event to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        public IntegrationEventEntity(IIntegrationEvent @event)
        {
            _ = @event ?? throw new ArgumentNullException(nameof(@event));

            Type = @event.GetType().AssemblyQualifiedName!;
            IsJson = true;
            Data = Serialize(@event);
        }

        /// <summary>
        /// Serializes the message to a JSON string using the <see cref="System.Text.Json"/>.
        /// You can override this method to customize its behavior.
        /// </summary>
        /// <returns>A JSON string.</returns>
        protected virtual byte[] Serialize(object message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, message.GetType()));
        }

        /// <summary>
        /// Deserializes the current message to the expected type or null using the <see cref="System.Text.Json"/>.
        /// </summary>
        /// <returns>An instance of the integration event type type or null.</returns>
        public virtual IIntegrationEvent? Deserialize()
            => JsonSerializer.Deserialize(Encoding.UTF8.GetString(Data), System.Type.GetType(Type)!) as IIntegrationEvent;
    }
}

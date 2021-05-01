
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

using Xpandables.Net.Entities;

namespace Xpandables.Net.Outboxes
{
    /// <summary>
    /// Represents an out box message to be written.
    /// </summary>
    public class MessageEntity : Entity
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
        /// Constructs anew instance of <see cref="MessageEntity"/> from the specified message.
        /// </summary>
        /// <param name="message">The message to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public MessageEntity(object message)
        {
            _ = message ?? throw new ArgumentNullException(nameof(message));

            Type = message.GetType().AssemblyQualifiedName!;
            IsJson = true;
            Data = Serialize(message);
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
        /// <typeparam name="TMessage">The expected type of the message.</typeparam>
        /// <returns>An instance of the type <typeparamref name="TMessage"/> type or null.</returns>
        public virtual TMessage? Deserialize<TMessage>()
        {
            return JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(Data));
        }
    }
}

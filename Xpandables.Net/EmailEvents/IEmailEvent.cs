
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
using System.Text.Json.Serialization;

using Xpandables.Net.Aggregates;

namespace Xpandables.Net.EmailEvents
{
    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as email event holding 
    /// a message type.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    public interface IEmailEvent<TAggregateId> : IEvent<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the target email.
        /// </summary>
        [JsonIgnore]
        object? Email { get; }
    }

    /// <summary>
    /// Defines a marker interface to be used to mark an object to act as email event holding 
    /// a specific <typeparamref name="TEmailMessage"/> type.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TEmailMessage">the type of the email.</typeparam>
    public interface IEmailEvent<TAggregateId, out TEmailMessage> : IEmailEvent<TAggregateId>
        where TAggregateId : notnull, IAggregateId
    {
        /// <summary>
        /// Gets the target email.
        /// </summary>
        [JsonIgnore]
        new TEmailMessage? Email { get; }
    }
}

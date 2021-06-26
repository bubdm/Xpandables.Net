
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
using System.Text.Json.Serialization;

using Xpandables.Net.Aggregates;

namespace Xpandables.Net.EmailEvents
{
    /// <summary>
    /// Represents a helper class that allows implementation of <see cref="IEmailEvent{ TEmail}"/> interface.
    /// </summary>
    /// <typeparam name="TAggregateId">The type of the aggregate identity.</typeparam>
    /// <typeparam name="TEmailMessage">the type of the email.</typeparam>
    public abstract class EmailEvent<TAggregateId, TEmailMessage> : Event<TAggregateId>, IEmailEvent<TEmailMessage>
        where TAggregateId : notnull, AggregateId
        where TEmailMessage : notnull
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="EmailEvent{TAggregateId, TEmail}"/>.
        /// </summary>
        /// <param name="aggregateId">The aggregate id.</param>
        /// <param name="email">The target domain event.</param>
        [JsonConstructor]
        protected EmailEvent(TAggregateId aggregateId, TEmailMessage email)
            : base(aggregateId)
            => EmailMessage = email ?? throw new ArgumentNullException(nameof(email));

        ///<inheritdoc/>
        public TEmailMessage EmailMessage { get; }

        object IEmailEvent.EmailMessage => EmailMessage;
    }
}

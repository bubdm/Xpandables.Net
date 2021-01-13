
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

namespace Xpandables.Net.CQRS
{
    /// <summary>
    /// This is the <see langword="abstract"/> class that implements <see cref="IDomainEventNotification"/> with <see cref="DateTime.UtcNow"/> as default <see cref="OccurredOn"/> value.
    /// </summary>
    public abstract class DomainEvent : IDomainEventNotification
    {
        /// <summary>
        /// Initializes a default instance of the <see cref="DomainEvent"/> class assigning <see cref="DateTime.UtcNow"/> to the <see cref="OccurredOn"/> property.
        /// </summary>
        protected DomainEvent() => OccurredOn = DateTime.UtcNow;

        /// <summary>
        /// Gets when the event occurred.
        /// </summary>
        public DateTime OccurredOn { get; }
    }
}

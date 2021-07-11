
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

using Xpandables.Net.Entities;
using Xpandables.Net.Events;

namespace Xpandables.Net.UnitOfWorks
{
    /// <summary>
    /// Represents the EFCore implementation of <see cref="IEventRepository{TEventStoreEntity}"/>.
    /// </summary>
    /// <typeparam name="TEventStoreEntity">The type of the event.</typeparam>
    public class EventRepository<TEventStoreEntity> : Repository<TEventStoreEntity>, IEventRepository<TEventStoreEntity>
        where TEventStoreEntity : EventStoreEntity
    {
        /// <summary>
        /// Constructs a new instance of <see cref="EventRepository{TEventStoreEntity}"/>.
        /// </summary>
        /// <param name="context">The db context to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is null.</exception>
        public EventRepository(DataContext context) : base(context) { }
    }
}

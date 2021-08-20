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
using Xpandables.Net.Entities;
using Xpandables.Net.UnitOfWorks;

namespace Xpandables.Net.Aggregates.Services
{
    /// <summary>
    /// Provides with base unit of work for aggregates.
    /// </summary>
    public interface IAggregateUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets the repository for <see cref="DomainStoreEntity"/> entities.
        /// </summary>
        IRepository<DomainStoreEntity> Events { get; }

        /// <summary>
        /// Gets the repository for <see cref="NotificationStoreEntity"/> entities.
        /// </summary>
        IRepository<NotificationStoreEntity> Notifications { get; }

        /// <summary>
        /// Gets the repository for <see cref="SnapShotStoreEntity"/> entities.
        /// </summary>
        IRepository<SnapShotStoreEntity> SnapShots { get; }
    }
}

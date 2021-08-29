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

using Xpandables.Net.Aggregates;
using Xpandables.Net.Entities;

namespace Xpandables.Net.UnitOfWorks;

/// <summary>
/// Represents the default implementation of <see cref="IAggregateUnitOfWork"/>.
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
public class AggregateUnitOfWork<TContext> : UnitOfWork<TContext>, IAggregateUnitOfWork
    where TContext : Context
{
    /// <summary>
    /// Constructs a new instance of <see cref="AggregateUnitOfWork{TContext}"/> with the context factory.
    /// </summary>
    /// <param name="unitOfWorkContextFactory">The context factory.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="unitOfWorkContextFactory"/> is null.</exception>
    public AggregateUnitOfWork(IUnitOfWorkContextFactory unitOfWorkContextFactory)
        : base(unitOfWorkContextFactory)
    {
        Events = new Repository<DomainEntity>(Context);
        Notifications = new Repository<NotificationEntity>(Context);
        SnapShots = new Repository<SnapShotEntity>(Context);
    }

    ///<inheritdoc/>
    public IRepository<DomainEntity> Events { get; }

    ///<inheritdoc/>
    public IRepository<NotificationEntity> Notifications { get; }

    ///<inheritdoc/>
    public IRepository<SnapShotEntity> SnapShots { get; }
}

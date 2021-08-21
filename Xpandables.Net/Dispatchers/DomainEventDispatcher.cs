﻿
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
using Xpandables.Net.Aggregates.Events;

namespace Xpandables.Net.Dispatchers;

/// <summary>
/// The default implementation of <see cref="IDomainEventDispatcher"/>.
/// You can derive from this class in order to customize its behaviors.
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IDispatcher _dispatcher;

    /// <summary>
    /// Constructs a new instance of <see cref="DomainEventDispatcher"/>.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="dispatcher"/> is null.</exception>
    public DomainEventDispatcher(IDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    ///<inheritdoc/>
    public virtual async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        => await _dispatcher.PublishAsync(@event, cancellationToken).ConfigureAwait(false);
}

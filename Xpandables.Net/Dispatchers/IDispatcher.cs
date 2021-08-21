
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
using Xpandables.Net.Commands;
using Xpandables.Net.Queries;

namespace Xpandables.Net.Dispatchers;

/// <summary>
/// Defines a set of methods to automatically handle <see cref="ICommand"/>, <see cref="ICommand{TResult}"/>, 
/// <see cref="IQuery{TResult}"/>, <see cref="IAsyncQuery{TResult}"/>, <see cref="IDomainEvent"/> and <see cref="INotification"/>
/// when targeting <see cref="IAsyncQueryHandler{TQuery, TResult}"/>, <see cref="IQueryHandler{TQuery, TResult}"/>, <see cref="ICommandHandler{TCommand}"/>,
/// <see cref="ICommandHandler{TCommand, TResult}"/>, <see cref="IDomainEventHandler{TDomainEvent}"/> or <see cref="INotificationHandler{TNotificationEvent}"/>.
/// The implementation must be thread-safe when working in a multi-threaded environment.
/// </summary>
public interface IDispatcher : ISender, IFetcher, IPublisher { }
